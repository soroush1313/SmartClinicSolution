using AppointmentService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace SmartClinicSolution.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // حذف DbContext اصلی
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppointmentDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // افزودن DbContext تستی با InMemory
                services.AddDbContext<AppointmentDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // اعمال migration اختیاری
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
                db.Database.EnsureCreated();

                // اینجا می‌تونی دیتاهای آزمایشی هم وارد کنی در صورت نیاز
            });
        }
    }
}
