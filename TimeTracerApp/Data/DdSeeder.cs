using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data.Models;

namespace TimeTracker.Data
{
    public static class DdSeeder
    {
        #region Public Methods
        public static void Seed (ApplicationDbContext dbContext)
        {
            // Create default Users (if there are none)
            if (!dbContext.Users.ToList().Any()) CreateUsers(dbContext);

            // Create default NodeElements (if there are none)
            if (!dbContext.NodeElements.ToList().Any()) CreateRootNodes(dbContext);
        }

       
        #endregion

        #region Seed Methods
        private static void CreateUsers(ApplicationDbContext dbContext)
        {
            // local variables
            DateTime createdDate = new DateTime(2018, 03, 01, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;

            // Create the "Admin" ApplicationUser account (if it doesn't exist already)
            var user_Admin = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@timetracer.top",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            // Insert the Admin user into the Database
            dbContext.Users.Add(user_Admin);

#if DEBUG
            // Create some sample registered user accounts (if they don't exist already)
            var user_Ryan = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Ryan",
                Email = "ryan@timetracer.top",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Solice = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Solice",
                Email = "solice@timetracer.top",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Vodan = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Vodan",
                Email = "vodan@timetracer.top",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            // Insert sample registered users into the Database
            dbContext.Users.AddRange(user_Ryan, user_Solice, user_Vodan);
#endif

            dbContext.SaveChanges();
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
