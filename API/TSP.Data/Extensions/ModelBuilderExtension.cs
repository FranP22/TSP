using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TSP.Common.Dto.Db;

namespace TSP.Data.Extensions
{
    public static class ModelBuilderExtension
    {
        public static ModelBuilder CreateAppModel(this ModelBuilder builder)
        {
            builder.DefineAppUser();
            builder.DefineAppRole();
            builder.DefineAppRoleClaim();
            builder.DefineAppUserSession();
            builder.DefineAppUsedRefreshTokens();
            builder.DefineAppOrder();

            return builder;
        }

        public static ModelBuilder DefineAppUser(this ModelBuilder builder)
        {
            builder.Entity<AppUser>(user =>
            {
                user
                    .HasKey(x => x.Id);

                user
                    .HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                user
                    .Property(x => x.Email)
                    //.IsRequired()
                    .HasMaxLength(255);

                user
                    .Property(x => x.EmailNormalized)
                    //.IsRequired()
                    .HasMaxLength(255);

                user
                    .Property(x => x.PasswordHash);
                    //.IsRequired();

                user
                    .Property(x => x.IsActive)
                    .HasDefaultValue(true);

                user
                    .HasIndex(x => x.EmailNormalized)
                    .IsUnique();
            });

            return builder;
        }

        public static ModelBuilder DefineAppRole(this ModelBuilder builder)
        {
            builder.Entity<AppRole>(role =>
            {
                role
                    .HasKey(x => x.Id);

                role
                    .Property(x => x.Name)
                    //.IsRequired()
                    .HasMaxLength(100);

                role
                    .Property(x => x.NameNormalized)
                    //.IsRequired()
                    .HasMaxLength(100);

                role
                    .Property(x => x.IsDefault)
                    .HasDefaultValue(false);

                role
                    .Property(x => x.IsAdmin)
                    .HasDefaultValue(false);

                role
                    .HasIndex(x => x.NameNormalized)
                    .IsUnique();
            });

            return builder;
        }

        public static ModelBuilder DefineAppRoleClaim(this ModelBuilder builder)
        {
            builder.Entity<AppRoleClaim>(roleClaim =>
            {
                roleClaim
                    .HasKey(x => new
                    {
                        x.RoleId,
                        x.ClaimType,
                        x.ClaimValue
                    });

                roleClaim
                    .HasOne(x => x.Role)
                    .WithMany(x => x.Claims)
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                roleClaim
                    .Property(x => x.RoleId)
                    //.IsRequired()
                    .HasMaxLength(36);

                roleClaim
                    .Property(x => x.ClaimType)
                    //.IsRequired()
                    .HasMaxLength(100);

                roleClaim
                    .Property(x => x.ClaimValue)
                    //.IsRequired()
                    .HasMaxLength(100);
            });

            return builder;
        }

        public static ModelBuilder DefineAppUserSession(this ModelBuilder builder)
        {
            builder.Entity<AppUserSession>(userSession =>
            {
                userSession
                    .HasKey(x => x.Id);

                userSession
                    .HasOne(x => x.User)
                    .WithMany(x => x.Sessions)
                    .HasForeignKey(x => x.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                userSession
                    .Property(x => x.RefreshTokenHash)
                    //.IsRequired()
                    .HasMaxLength(128);

                userSession
                    .Property(x => x.RevokeReason)
                    //.IsRequired()
                    .HasMaxLength(256);
            });

            return builder;
        }

        public static ModelBuilder DefineAppUsedRefreshTokens(this ModelBuilder builder)
        {
            builder.Entity<AppUsedRefreshTokens>(usedToken =>
            {
                usedToken
                    .HasKey(x => x.Id);

                usedToken
                    .HasOne(x => x.UserSession)
                    .WithMany(x => x.UsedRefreshTokens)
                    .HasForeignKey(x => x.SessionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                usedToken
                    .Property(x => x.TokenHash)
                    //.isrequired()
                    .HasMaxLength(128);
            });

            return builder;
        }

        public static ModelBuilder DefineAppOrder(this ModelBuilder builder)
        {
            builder.Entity<AppOrder>(order =>
            {
                order
                    .HasKey(x => x.Id);

                order
                    .HasOne(x => x.User)
                    .WithMany(x => x.Orders)
                    .HasForeignKey(x => x.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                order
                    .Property(x => x.Description)
                    //.isrequired()
                    .HasMaxLength(500);
            });

            return builder;
        }

        public static ModelBuilder DefineAppProducts(this ModelBuilder builder)
        {
            builder.Entity<AppProduct>(product =>
            {
                product
                    .HasKey(x => x.Id);

                product
                    .Property(x => x.Name)
                    //.isrequired()
                    .HasMaxLength(100);

                product
                    .Property(x => x.Description)
                    //.isrequired()
                    .HasMaxLength(500);

                product
                    .Property(x => x.IsActive)
                    .HasDefaultValue(true);
            });

            return builder;
        }
    }
}
