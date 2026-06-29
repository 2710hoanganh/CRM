using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Configurations
{
    public class LoanTransactionConfiguration : IEntityTypeConfiguration<LoanTransaction>
    {
        public void Configure(EntityTypeBuilder<LoanTransaction> builder)
        {
            builder.ToTable("LoanTransactions", "dbo");
            builder.HasKey(lt => lt.Id);
            builder.Property(lt => lt.Amount).HasPrecision(18, 2).IsRequired();
            builder.HasOne(lt => lt.Loan).WithMany(l => l.LoanTransactions).HasForeignKey(lt => lt.LoanId);
        }
    }
}
