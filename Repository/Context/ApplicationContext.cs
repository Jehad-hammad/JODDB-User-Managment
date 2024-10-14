using Domain.DTO.Settings;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Repository.Context
{
    public partial class ApplicationContext : IdentityDbContext<ApplicationUser, Roles, long, UserClaims, UserRoles, UserLogins, RoleClaims, UserTokens>
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        #region Identity
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<RoleClaims> RoleClaims { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserLogins> UserLogins { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserTokens> UserTokens { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=JehadH;Initial Catalog=JODDB_Managment;Integrated Security=True;TrustServerCertificate=True", builder =>
                {
                    builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null);
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Roles Seed
            var role = new Roles
            {
                Id = 1,
                Name = "superadmin",
                NormalizedName = "SUPERADMIN",

            };
            builder.Entity<Roles>().HasData(role);
            #endregion

            #region User Seed
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            var user = new ApplicationUser
            {
                Id = 1,
                Email = "superadmin@joddp.com",
                EmailConfirmed = true,
                PhoneNumber = "000000000",
                PhoneNumberConfirmed = true,
                UserName = "superadmin",
                SecurityStamp = Guid.NewGuid().ToString(),
                NormalizedEmail = "SUPERADMIN@LORRY.COM",
                NormalizedUserName = "SUPER ADMIN",
                FullName ="Super Admin",
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "P@ssw0rd") // Hash the password
            };

            builder.Entity<ApplicationUser>().HasData(user);

            var userRoles = new UserRoles
            {
                RoleId = 1,
                UserId = 1,
            };
            builder.Entity<UserRoles>().HasData(userRoles);
            #endregion
        }

    }
}
