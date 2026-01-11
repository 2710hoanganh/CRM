using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Constants.AppEnum; 
namespace Persistence.Contexts.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans", "dbo");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(l => l.PaybackAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(l => l.Total).HasPrecision(18, 2).IsRequired();
            builder.Property(l => l.Term).HasDefaultValue((int)LoanTerm.TwelveMonths).IsRequired();
            builder.Property(l => l.Rate).HasDefaultValue((int)LoanRate.BaseRate).IsRequired();
            builder.Property(l => l.Status).HasDefaultValue((int)LoanStatus.Pending).IsRequired();
            builder.HasOne(l => l.User).WithMany(u => u.Loans).HasForeignKey(l => l.UserId);
        }
    }
}