using Habr.Common.Enums;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entityTypeBuilder.Property(x => x.Name)
                .HasMaxLength(256);

            entityTypeBuilder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            entityTypeBuilder.HasIndex(x => x.Email).IsUnique();

            entityTypeBuilder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(128);

            entityTypeBuilder.Property(x => x.Created)
                .IsRequired();

            entityTypeBuilder.Property(x => x.RefreshToken)
                .HasMaxLength(64);

            entityTypeBuilder.Property(x => x.UserRole)
                .HasDefaultValue((UserRole)0);

            entityTypeBuilder
                .HasMany(x => x.Posts)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entityTypeBuilder
                .HasMany(x => x.Comments)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
