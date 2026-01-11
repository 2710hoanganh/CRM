using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Constants.AppEnum;

namespace Persistence.Contexts.Configurations
{
    public class UserReferenceConfiguration : IEntityTypeConfiguration<UserReference>
    {
        public void Configure(EntityTypeBuilder<UserReference> builder)
        {
            builder.ToTable("UserReferences", "dbo");
            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.FullName).HasMaxLength(250).IsRequired();
            builder.Property(ur => ur.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(ur => ur.Relationship).HasDefaultValue((int)ReferenceRelationship.Parent).IsRequired();
            builder.Property(ur => ur.Status).HasDefaultValue((int)Status.Active).IsRequired();
            builder.HasOne(ur => ur.User).WithMany(u => u.UserReferences).HasForeignKey(ur => ur.UserId);
        }
    }
}