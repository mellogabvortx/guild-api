using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Maps
{
	public class GuildMap : IEntityTypeConfiguration<Guild>
	{
		public void Configure(EntityTypeBuilder<Guild> builder)
		{
			builder.HasKey(x => x.Id);
			builder.HasIndex(u => u.Name)
				.IsUnique();
			builder.HasMany(x => x.Members)
				.WithOne(x => x.Guild)
				.HasForeignKey(x => x.GuildId)
				.OnDelete(DeleteBehavior.Restrict);
			builder.Property(x => x.CreatedDate)
				.ValueGeneratedOnAdd()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");
			builder.Property(x => x.ModifiedDate)
				.ValueGeneratedOnAddOrUpdate()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");
		}
	}
}