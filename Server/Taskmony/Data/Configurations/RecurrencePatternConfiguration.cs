using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Tasks;

namespace Taskmony.Data.Configurations;

public class RecurrencePatternConfiguration : IEntityTypeConfiguration<RecurrencePattern>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RecurrencePattern> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasValueGenerator<GuidValueGenerator>();
    }
}