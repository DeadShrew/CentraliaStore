﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Models;
using CentraliaStore.Areas.Identity;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using System.Collections.Generic;

namespace CentraliaStore.Data
{
    public class StoreContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration Configuration;

        public StoreContext(DbContextOptions<StoreContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        // dynamic seeded data and configuration goes here
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSeeding((context, _) =>
                {
                    // checking for a value in the database
                    var admin = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:AdminEmail"]);

                    var hasher = new PasswordHasher<AppUser>();

                    // if the value doesnt exist, go ahead and add it
                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            // add properties to specify the app user
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        };

                        // add a user
                        context.Set<AppUser>().Add(admin);
                        context.SaveChanges();
                    }

                    // i have an admin

                    var user = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:TestUserEmail"]);

                    if (user == null)
                    {
                        // add a user
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
                            UserName = Configuration["Accounts:TestUserEmail"],
                            NormalizedUserName = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            Email = Configuration["Accounts:TestUserEmail"],
                            NormalizedEmail = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:TestUserPassword"])
                        });
                        context.SaveChanges();
                    }

                    var adminRole = context.Set<IdentityRole>().FirstOrDefault(r => r.Name == "Administrator");

                    if (adminRole == null)
                    {
                        string roleId = Guid.NewGuid().ToString();

                        adminRole = new IdentityRole
                        {
                            Id = roleId,
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                            ConcurrencyStamp = roleId,
                        };

                        context.Set<IdentityRole>().Add(adminRole);
                        context.SaveChanges();
                    }

                    // i have an admin role

                    var roleUser = context.Set<IdentityUserRole<string>>().FirstOrDefault(a => a.RoleId == adminRole.Id);

                    if (roleUser == null)
                    {
                        // add it
                        roleUser = new IdentityUserRole<string>
                        {
                            RoleId = adminRole.Id,
                            UserId = admin.Id
                        };

                        context.Set<IdentityUserRole<string>>().Add(roleUser);
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    // checking for a value in the database
                    var admin = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:AdminEmail"]);

                    var hasher = new PasswordHasher<AppUser>();

                    // if the value doesnt exist, go ahead and add it
                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            // add properties to specify the app user
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        };

                        // add a user
                        context.Set<AppUser>().Add(admin);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // i have an admin

                    var user = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:TestUserEmail"]);

                    if (user == null)
                    {
                        // add a user
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
                            UserName = Configuration["Accounts:TestUserEmail"],
                            NormalizedUserName = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            Email = Configuration["Accounts:TestUserEmail"],
                            NormalizedEmail = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:TestUserPassword"])
                        });
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var adminRole = await context.Set<IdentityRole>().FirstOrDefaultAsync(r => r.Name == "Administrator");

                    if (adminRole == null)
                    {
                        string roleId = Guid.NewGuid().ToString();

                        adminRole = new IdentityRole
                        {
                            Id = roleId,
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                            ConcurrencyStamp = roleId,
                        };

                        context.Set<IdentityRole>().Add(adminRole);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // i have an admin role

                    var roleUser = await context.Set<IdentityUserRole<string>>().FirstOrDefaultAsync(a => a.RoleId == adminRole.Id);

                    if (roleUser == null)
                    {
                        // add it
                        roleUser = new IdentityUserRole<string>
                        {
                            RoleId = adminRole.Id,
                            UserId = admin.Id
                        };

                        context.Set<IdentityUserRole<string>>().Add(roleUser);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });

        // static seeded data and model setup goes in this method
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // add categories
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Sweatshirts" },
                new Category { CategoryId = 2, Name = "Water Bottles" },
                new Category { CategoryId = 3, Name = "Notebooks" },
                new Category { CategoryId = 4, Name = "Textbooks" },
                new Category
                {
                    CategoryId = 5,
                    Name = "Writing Utensils"
                }
            );
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<IdentityRole> Roles { get; set; } = default!;
    }
}
