using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Persistence;

namespace UserService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;
        private readonly RedisCacheService _redis;
        public UserService(AppDbContext dbContext, JwtService jwtService, RedisCacheService redis)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _redis = redis;
        }

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            // 1. چک در Redis برای جلوگیری از کوئری به SQL
            if (await _redis.ExistsAsync($"user:email:{request.Email}"))
            {
                throw new Exception("Email is already registered (cached).");
            }

            // 2. چک در SQL فقط اگر Redis خالی بود
            if (await _dbContext.Patients.AnyAsync(p => p.Email == request.Email) ||
                await _dbContext.Doctors.AnyAsync(d => d.Email == request.Email))
            {
                // ✅ اگر ایمیل جدید توی SQL پیدا شد، پس اون رو داخل Redis هم کش کن:
                await _redis.SetAsync($"user:email:{request.Email}", true, TimeSpan.FromMinutes(30));
                throw new Exception("Email is already registered.");
            }

            // 3. هش کردن رمز عبور
            var passwordHash = HashPassword(request.Password);

            // 4. ساخت یوزر
            if (request.Role?.ToLower() == "doctor")
            {
                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    Specialty = request.Specialty ?? "General"
                };

                _dbContext.Doctors.Add(doctor);
                await _dbContext.SaveChangesAsync();

                // 5. ذخیره ایمیل در Redis بعد از ثبت‌نام موفق
                await _redis.SetAsync($"user:email:{request.Email}", true, TimeSpan.FromMinutes(30));

                return doctor.Id;
            }
            else
            {
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = passwordHash
                };

                _dbContext.Patients.Add(patient);
                await _dbContext.SaveChangesAsync();

                // 5. ذخیره ایمیل در Redis بعد از ثبت‌نام موفق
                await _redis.SetAsync($"user:email:{request.Email}", true, TimeSpan.FromMinutes(30));

                return patient.Id;
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // 1. هش کردن رمز عبور وارد شده
            var passwordHash = HashPassword(password);

            // 2. جستجو در بیماران
            User? user = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.Email == email && p.PasswordHash == passwordHash);

            // 3. اگر در بیماران نبود، در پزشکان جستجو کن
            if (user == null)
            {
                user = await _dbContext.Doctors
                    .FirstOrDefaultAsync(d => d.Email == email && d.PasswordHash == passwordHash);
            }

            // 4. اگر هیچ‌کدام نبود، خطا بده
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }

            // 5. تعیین نقش (پزشک یا بیمار)
            var role = user is Doctor ? "Doctor" : "Patient";

            // 6. تولید توکن JWT
            return _jwtService.GenerateToken(user.Id, role);
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
