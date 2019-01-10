using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data.Models;

namespace TimeTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region Constuctor
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder
            modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.RootElements).WithOne(i => i.User);
            modelBuilder.Entity<NodeElement>().ToTable("NodeElements");
            modelBuilder.Entity<NodeElement>().Property(i => i.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<NodeElement>().HasMany(u => u.NodeElements).WithOne(i => i.ParentNode);
            modelBuilder.Entity<NodeElement>().HasOne(i => i.User).WithMany(u => u.RootElements);
            modelBuilder.Entity<NodeElement>().HasOne(i => i.ParentNode).WithMany(u => u.NodeElements).OnDelete(DeleteBehavior.ClientSetNull);
        }
        #endregion

        #region Properties
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<NodeElement> NodeElements { get; set; }
        #endregion
    }
}
