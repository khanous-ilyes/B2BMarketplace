using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.Domains.AnyAsync())
            {
                var domains = new List<Domain>
                {
                    new()
                    {
                        Name = "Industrie",
                        Slug = "industrie",
                        Categories = new List<Category>
                        {
                            new() { Name = "Machines Industrielles", Slug = "machines-industrielles" },
                            new() { Name = "Outillage", Slug = "outillage" }
                        }
                    },
                    new()
                    {
                        Name = "Construction",
                        Slug = "construction",
                        Categories = new List<Category>
                        {
                            new() { Name = "Matériaux", Slug = "materiaux" },
                            new() { Name = "Équipements Lourds", Slug = "equipements-lourds" }
                        }
                    },
                    new()
                    {
                        Name = "Services",
                        Slug = "services",
                        Categories = new List<Category>
                        {
                            new() { Name = "Ingénierie", Slug = "ingenierie" },
                            new() { Name = "Maintenance", Slug = "maintenance" }
                        }
                    }
                };

                await context.Domains.AddRangeAsync(domains);
                await context.SaveChangesAsync();
            }

            // Seed Admin User
            if (!await context.Users.AnyAsync(u => u.UserType == BaseLibrary.Helpers.UserType.Admin))
            {
                var adminUser = new User
                {
                    Email = "admin@b2b.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    UserType = BaseLibrary.Helpers.UserType.Admin,
                    Status = BaseLibrary.Helpers.UserStatus.Active,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
