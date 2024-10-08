using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Configurations
{
    public class PostRatingConfiguration
    {
        public void Configure(EntityTypeBuilder<PostRate> entityTypeBuilder)
        {
            entityTypeBuilder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            entityTypeBuilder.Property(x => x.CreatedAt)
                .IsRequired();

            entityTypeBuilder.Property(x => x.Value)
                .IsRequired();

            entityTypeBuilder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
