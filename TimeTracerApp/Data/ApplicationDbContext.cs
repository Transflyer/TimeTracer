using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data.Models;

namespace TimeTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Constuctor

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constuctor

        #region Methods

        protected override void OnModelCreating(ModelBuilder
            modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.RootElements).WithOne(i => i.User);
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Tokens).WithOne(i => i.User);

            modelBuilder.Entity<NodeElement>().ToTable("NodeElements");
            modelBuilder.Entity<NodeElement>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<NodeElement>().HasOne(i => i.User).WithMany(u => u.RootElements);
            modelBuilder.Entity<NodeElement>().HasOne(i => i.ParentNode).WithMany(u => u.NodeElements).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<NodeElement>().Property(p => p.Deleted).HasConversion<int>();
            modelBuilder.Entity<NodeElement>().HasIndex(p => p.UserId);
            modelBuilder.Entity<NodeElement>().HasIndex(p => p.ParentId);

            modelBuilder.Entity<Token>().ToTable("Tokens");
            modelBuilder.Entity<Token>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Token>().HasOne(i => i.User).WithMany(u => u.Tokens);
            modelBuilder.Entity<Token>().HasIndex(p => p.ClientId);
            modelBuilder.Entity<Token>().HasIndex(p => p.UserId);

            modelBuilder.Entity<Interval>().ToTable("Intervals");
            modelBuilder.Entity<Interval>().Property(i => i.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Interval>().HasOne(i => i.NodeElement).WithMany(t => t.Intervals);
            modelBuilder.Entity<Interval>().HasIndex(p => p.UserId);
            modelBuilder.Entity<Interval>().HasIndex(p => p.ElementId);
            modelBuilder.Entity<Interval>().HasIndex(p => p.IsOpen);
            modelBuilder.Entity<Interval>().HasIndex(p => p.Start);
            modelBuilder.Entity<Interval>().HasIndex(p => p.End);


            #region MySQL restrictions on MySQL

            modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Name).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }

            #endregion MySQL restrictions
        }

        #endregion Methods

        #region Properties

        //public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<NodeElement> NodeElements { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<Interval> Intervals { get; set; }

        #endregion Properties
    }
}