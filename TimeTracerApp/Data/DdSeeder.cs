using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace TimeTracker.Data
{
    public static class DdSeeder
    {
        #region Public Methods
        public static void Seed(ApplicationDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            // Create default Users (if there are none)
            if (!dbContext.Users.ToList().Any()) CreateUsers(dbContext, roleManager, userManager)
                    .GetAwaiter()
                    .GetResult();

            // Create default NodeElements (if there are none)
            if (!dbContext.NodeElements.ToList().Any()) CreateRootNodes(dbContext);
        }


        #endregion

        #region Seed Methods
        private static async Task CreateUsers(
             ApplicationDbContext dbContext,
             RoleManager<IdentityRole> roleManager,
             UserManager<ApplicationUser> userManager)
        {

            // local variables
            DateTime createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;

            string role_Administrator = "Administrator";
            string role_RegisteredUser = "RegisteredUser";

            //Create Roles (if they doesn't exist yet)
            if (!await roleManager.RoleExistsAsync(role_Administrator))
            {
                await roleManager.CreateAsync(new
                IdentityRole(role_Administrator));
            }
            if (!await roleManager.RoleExistsAsync(role_RegisteredUser))
            {
                await roleManager.CreateAsync(new
                IdentityRole(role_RegisteredUser));
            }

            // Create the "Admin" ApplicationUser account (if it doesn't exist already)
            var user_Admin = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            // Insert "Admin" into the Database and assign the "Administrator"     and "RegisteredUser" roles to him.
            if (await userManager.FindByNameAsync(user_Admin.UserName) == null)
            {
                await userManager.CreateAsync(user_Admin, "Pass4Admin");
                await userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await userManager.AddToRoleAsync(user_Admin, role_Administrator);
                // Remove Lockout and E-Mail confirmation.
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;
            }

#if DEBUG
            // Create some sample registered user accounts (if they don't exist already)
            var user_Ryan = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Ryan",
                Email = "ryan@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Solice = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Solice",
                Email = "solice@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Vodan = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Vodan",
                Email = "vodan@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            // Insert sample registered users into the Database and also assign the "Registered" role to him.
            if (await userManager.FindByNameAsync(user_Ryan.UserName) == null)
            {
                await userManager.CreateAsync(user_Ryan, "Pass4Ryan");
                await userManager.AddToRoleAsync(user_Ryan,
                role_RegisteredUser);
                // Remove Lockout and E-Mail confirmation.
                user_Ryan.EmailConfirmed = true;
                user_Ryan.LockoutEnabled = false;
            }
            if (await userManager.FindByNameAsync(user_Solice.UserName) == null)
            {
                await userManager.CreateAsync(user_Solice, "Pass4Solice");
                await userManager.AddToRoleAsync(user_Solice,
                role_RegisteredUser);
                // Remove Lockout and E-Mail confirmation.
                user_Solice.EmailConfirmed = true;
                user_Solice.LockoutEnabled = false;
            }
            if (await userManager.FindByNameAsync(user_Vodan.UserName) == null)
            {
                await userManager.CreateAsync(user_Vodan, "Pass4Vodan");
                await userManager.AddToRoleAsync(user_Vodan,
                role_RegisteredUser);
                // Remove Lockout and E-Mail confirmation.
                user_Vodan.EmailConfirmed = true;
                user_Vodan.LockoutEnabled = false;
            }
#endif
            await dbContext.SaveChangesAsync();
        }

        private static void CreateRootNodes(ApplicationDbContext dbContext)
        {
            // local variables
            DateTime createdDate = new DateTime(2018, 08, 08, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;

            // retrieve the admin user, which we'll use as default author.
            var authorId = dbContext.Users
                .Where(u => u.UserName == "Admin")
                .FirstOrDefault()
                .Id;
#if DEBUG
            dbContext.NodeElements.Add(new NodeElement()
            {
                UserId = authorId,
                Title = "Customer John",
                Description = "My first customer",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });

            dbContext.SaveChanges();

            dbContext.NodeElements.Add(new NodeElement()
            {
                UserId = authorId,
                Title = "Customer Mike",
                Description = "My secont customer",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });

            dbContext.NodeElements.Add(new NodeElement()
            {
                UserId = authorId,
                Title = "Customer Barbara",
                Description = "My best customer",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });

            dbContext.SaveChanges();
#endif

        }


        #endregion
    }
}
