using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Domain.Enums;
using HiredIn.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.API.Services
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var email = configuration["AdminSeed:Email"]?.Trim().ToLowerInvariant();
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var db = services.GetRequiredService<ApplicationDbContext>();

            var adminExists = await db.Users
                .AnyAsync(u => u.Role == UserRole.Admin && u.DeletedAtUtc == null);

            if (adminExists)
                return;

            var now = DateTime.UtcNow;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            var passwordHasher = services.GetRequiredService<IPasswordHasherService>();

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = configuration["AdminSeed:FirstName"]?.Trim() ?? "Admin",
                    LastName = configuration["AdminSeed:LastName"]?.Trim() ?? "User",
                    Email = email,
                    Role = UserRole.Admin,
                    Status = UserStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                user.PasswordHash = passwordHasher.HashPassword(user, password);

                await db.Users.AddAsync(user);
            }
            else
            {
                user.Role = UserRole.Admin;
                user.Status = UserStatus.Active;
                user.DeletedAtUtc = null;
                user.UpdatedAtUtc = now;
                user.PasswordHash = passwordHasher.HashPassword(user, password);
            }

            await db.SaveChangesAsync();
        }
    }
}
