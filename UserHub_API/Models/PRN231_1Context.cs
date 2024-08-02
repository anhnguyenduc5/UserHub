using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project_API.Models
{
    public partial class PRN231_1Context : DbContext
    {
        public PRN231_1Context()
        {
        }

        public PRN231_1Context(DbContextOptions<PRN231_1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; } = null!;
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
            base.OnModelCreating(modelBuilder);

            // Cấu hình thực thể AspNetUserRole
            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                      .WithMany(p => p.UserRoles)
                      .HasForeignKey(d => d.RoleId)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.UserRoles)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Cấu hình thực thể AspNetRole
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                      .IsUnique()
                      .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.HasIndex(e => e.Name, "UQ__Roles__8A2B6160F550D508")
                      .IsUnique()
                      .HasFilter("([Name] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            // Cấu hình thực thể AspNetRoleClaim
            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                      .WithMany(p => p.AspNetRoleClaims)
                      .HasForeignKey(d => d.RoleId);
            });

            // Cấu hình thực thể AspNetUser
            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.UserName, "UQ__Users__536C85E45C78FBA3")
                      .IsUnique()
                      .HasFilter("([UserName] IS NOT NULL)");

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CE60D369")
                      .IsUnique()
                      .HasFilter("([Email] IS NOT NULL)");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                      .IsUnique()
                      .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                      .WithMany(p => p.Users)
                      .UsingEntity<AspNetUserRole>(
                          j => j.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId),
                          j => j.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId),
                          j =>
                          {
                              j.HasKey("UserId", "RoleId");
                              j.ToTable("AspNetUserRoles");
                              j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                          });
            });

            // Cấu hình thực thể AspNetUserClaim
            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.AspNetUserClaims)
                      .HasForeignKey(d => d.UserId);
            });

            // Cấu hình thực thể AspNetUserLogin
            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.AspNetUserLogins)
                      .HasForeignKey(d => d.UserId);
            });

            // Cấu hình thực thể AspNetUserToken
            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                      .WithMany(p => p.AspNetUserTokens)
                      .HasForeignKey(d => d.UserId);
            });

            // Cấu hình thực thể UserDetail
            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.UserDetailsId)
                      .HasName("PK__UserDeta__053A9382007C1DA8");

                entity.HasIndex(e => e.UserId, "IX_UserDetails_UserId");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.UserDetails)
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("FK__UserDetai__UserI__440B1D61");
            });

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
