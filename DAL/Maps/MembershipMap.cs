using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Maps
{
	public class MembershipMap : IEntityTypeConfiguration<Membership>
	{
		public void Configure(EntityTypeBuilder<Membership> builder)
		{
			builder.HasKey(x => x.Id);
			builder.HasOne(x => x.Guild)
				.WithMany()
				.HasForeignKey(x => x.GuildId)
				.OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(x => x.Member)
				.WithMany(x => x.Memberships)
				.HasForeignKey(x => x.MemberId)
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