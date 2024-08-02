using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project_MVC.Models
{
    public partial class PRN231_1Context : IdentityDbContext<AspNetUser, AspNetRole, int, AspNetUserClaim, IdentityUserRole<int>, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        public PRN231_1Context(DbContextOptions<PRN231_1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(config.GetConnectionString("value"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base method

            // Configure AspNetRole
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            // Configure AspNetRoleClaim
            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            // Configure AspNetUser
            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail);
                entity.HasIndex(e => e.UserName)
                    .IsUnique()
                    .HasFilter("([UserName] IS NOT NULL)");
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("([Email] IS NOT NULL)");
                entity.HasIndex(e => e.NormalizedUserName)
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            // Configure AspNetUserClaim
            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            // Configure AspNetUserLogin
            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }); // Set composite key
                entity.HasIndex(e => e.UserId);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            // Configure AspNetUserToken
            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }); // Set composite key
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            // Configure UserDetail
            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.UserDetailsId)
                    .HasName("PK__UserDeta__053A9382007C1DA8");

                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserDetai__UserI__440B1D61");
            });
        }
    }
}
