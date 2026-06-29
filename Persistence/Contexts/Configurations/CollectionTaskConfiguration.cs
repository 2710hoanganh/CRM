using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Configurations
{
    public class CollectionTaskConfiguration : IEntityTypeConfiguration<CollectionTask>
    {
        public void Configure(EntityTypeBuilder<CollectionTask> builder)
        {
            builder.ToTable("CollectionTasks", "dbo");
            builder.HasKey(ct => ct.Id);
            builder.HasOne(ct => ct.Loan).WithMany().HasForeignKey(ct => ct.LoanId);
        }
    }
}
