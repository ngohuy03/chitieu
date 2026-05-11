using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupExpenseManager.Infrastructure.Persistence.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure one-to-many relationship for Contributor
            builder.HasOne(x => x.Contributor)
                .WithMany() // We don't need a collection in User for contributed groups unless we want to
                .HasForeignKey(x => x.ContributorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many relationship for Members (if needed, or let EF handle it)
            // If EF handles it, it creates a join table. Let's make sure it doesn't conflict.
            builder.HasMany(x => x.Members)
                .WithMany(x => x.Groups);
        }
    }
}
