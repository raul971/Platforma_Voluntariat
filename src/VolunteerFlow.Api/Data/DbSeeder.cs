using VolunteerFlow.Api.Models;
using VolunteerFlow.Api.Helpers;

namespace VolunteerFlow.Api.Data;

public static class DbSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Check if database is already seeded
        if (context.Users.Any())
        {
            return; // Database already has data
        }

        // Create Admin user
        var admin = new User
        {
            Email = "admin@example.com",
            PasswordHash = PasswordHasher.HashPassword("Admin123!"),
            Role = "Admin",
            FullName = "Admin User",
            CreatedAt = DateTime.UtcNow
        };

        // Create Volunteer users
        var volunteer1 = new User
        {
            Email = "volunteer1@example.com",
            PasswordHash = PasswordHasher.HashPassword("Volunteer123!"),
            Role = "Volunteer",
            FullName = "Jane Volunteer",
            CreatedAt = DateTime.UtcNow
        };

        var volunteer2 = new User
        {
            Email = "volunteer2@example.com",
            PasswordHash = PasswordHasher.HashPassword("Volunteer123!"),
            Role = "Volunteer",
            FullName = "John Volunteer",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(admin, volunteer1, volunteer2);
        context.SaveChanges();

        Console.WriteLine("✅ Database seeded successfully!");
        Console.WriteLine("📧 Admin: admin@example.com / Admin123!");
        Console.WriteLine("📧 Volunteer1: volunteer1@example.com / Volunteer123!");
        Console.WriteLine("📧 Volunteer2: volunteer2@example.com / Volunteer123!");
    }
}
