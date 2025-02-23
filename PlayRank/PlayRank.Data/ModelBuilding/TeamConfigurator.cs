using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayRank.Domain.Entities;

namespace PlayRank.Data.ModelBuilding
{
    public class TeamConfigurator : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasIndex(t => t.Name)
            .IsUnique();

            builder.Property(t => t.Name)
                .IsRequired()
            .HasMaxLength(100);

            builder.HasOne(t => t.Ranking)
                .WithOne(r => r.Team)
                .HasForeignKey<Ranking>(r => r.TeamId);
        }
    }
}