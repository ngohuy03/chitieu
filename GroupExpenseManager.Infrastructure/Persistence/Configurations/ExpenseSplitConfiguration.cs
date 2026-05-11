using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroupExpenseManager.Infrastructure.Persistence.Configurations
{
    public class ExpenseSplitConfiguration : IEntityTypeConfiguration<ExpenseSplit>
    {
        public void Configure(EntityTypeBuilder<ExpenseSplit> builder)
        {
            builder.Property(x => x.OwedAmount)
                .HasColumnType("decimal(18,2)");

            // Fix cycle or multiple cascade paths
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
