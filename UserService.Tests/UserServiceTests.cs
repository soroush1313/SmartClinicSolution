//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UserService.Application.DTOs;
//using UserService.Infrastructure;
//using UserService.Persistence;

//namespace UserService.Tests
//{
//    public class UserServiceTests
//    {
//        private AppDbContext GetDbContext()
//        {
//            var options = new DbContextOptionsBuilder<AppDbContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;
//            return new AppDbContext(options);
//        }

//        private JwtService GetJwtService() =>
//            new JwtService("this_is_a_very_long_test_key_for_jwt_1234567890");


//        [Fact]
//        public async Task RegisterAsync_ShouldRegisterPatientSuccessfully()
//        {
//            // Arrange
//            var dbContext = GetDbContext();
//            var jwtService = GetJwtService();
//            var userService = new UserService.Infrastructure.Services.UserService(dbContext, jwtService);

//            var request = new RegisterRequest
//            {
//                FullName = "Ali Test",
//                Email = "ali@test.com",
//                Password = "123456",
//                Role = "Patient"
//            };

//            // Act
//            var result = await userService.RegisterAsync(request);

//            // Assert
//            Assert.NotEqual(Guid.Empty, result);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
//        {
//            // Arrange
//            var dbContext = GetDbContext();
//            var jwtService = GetJwtService();
//            var userService = new UserService.Infrastructure.Services.UserService(dbContext, jwtService);

//            var request = new RegisterRequest
//            {
//                FullName = "Ali",
//                Email = "duplicate@test.com",
//                Password = "pass",
//                Role = "Doctor",
//                Specialty = "Cardio"
//            };

//            await userService.RegisterAsync(request);

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => userService.RegisterAsync(request));
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
//        {
//            // Arrange
//            var dbContext = GetDbContext();
//            var jwtService = GetJwtService();
//            var userService = new UserService.Infrastructure.Services.UserService(dbContext, jwtService);

//            var request = new RegisterRequest
//            {
//                FullName = "User Test",
//                Email = "login@test.com",
//                Password = "password123",
//                Role = "Patient"
//            };

//            await userService.RegisterAsync(request);

//            // Act
//            var token = await userService.LoginAsync(request.Email, request.Password);

//            // Assert
//            Assert.False(string.IsNullOrEmpty(token));
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
//        {
//            // Arrange
//            var dbContext = GetDbContext();
//            var jwtService = GetJwtService();
//            var userService = new UserService.Infrastructure.Services.UserService(dbContext, jwtService);

//            var request = new RegisterRequest
//            {
//                FullName = "Bad Password",
//                Email = "badpass@test.com",
//                Password = "correctpass",
//                Role = "Patient"
//            };

//            await userService.RegisterAsync(request);

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => userService.LoginAsync(request.Email, "wrongpass"));
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldThrowException_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var dbContext = GetDbContext();
//            var jwtService = GetJwtService();
//            var userService = new UserService.Infrastructure.Services.UserService(dbContext, jwtService);

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => userService.LoginAsync("notfound@test.com", "123"));
//        }
//    }
//}
