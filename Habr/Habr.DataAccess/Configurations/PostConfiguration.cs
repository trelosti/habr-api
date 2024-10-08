using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entityTypeBuilder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entityTypeBuilder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);

            entityTypeBuilder.Property(x => x.Created)
                .IsRequired();

            entityTypeBuilder.Property(x => x.IsPublished)
                .HasDefaultValue(false);

            entityTypeBuilder
                .HasMany(x => x.Comments)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientCascade);

            entityTypeBuilder
                .HasMany(x => x.PostRates)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
