using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Configurations
{
    public class UserRepaymentConfiguration : IEntityTypeConfiguration<UserRepayment>
    {
        public void Configure(EntityTypeBuilder<UserRepayment> builder)
        {
            builder.ToTable("UserRepayments", "dbo");
            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.PrincipalAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(ur => ur.InterestAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(ur => ur.PenaltyAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(ur => ur.PaidAmount).HasPrecision(18, 2).IsRequired();
            builder.HasOne(ur => ur.Loan).WithMany(l => l.UserRepayments).HasForeignKey(ur => ur.LoanId);
        }
    }
}
