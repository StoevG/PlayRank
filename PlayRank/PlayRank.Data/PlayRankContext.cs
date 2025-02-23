using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Abstract;

namespace PlayRank.Data
{
    public class PlayRankContext(DbContextOptions<PlayRankContext> options) : DbContext(options)
    {
        public DbSet<Match> Matches { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Ranking> Rankings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ApplySoftDeleteFilter(modelBuilder);
        }

        public override int SaveChanges()
        {
            this.UpdateCreateAndModifyDates();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.UpdateCreateAndModifyDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(IEntity).IsAssignableFrom(e.ClrType));

            foreach (var entityType in entityTypes)
            {
                var method = typeof(PlayRankContext)
                    .GetMethod(nameof(SetSoftDeleteQueryFilter), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }

        private static void SetSoftDeleteQueryFilter<T>(ModelBuilder modelBuilder) where T : class, IEntity
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedOn == null);
        }

        private void UpdateCreateAndModifyDates()
        {
            var currentTime = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                var entity = entry.Entity as IEntity;
                if (entity is null)
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added: entity.CreatedDate = currentTime; break;
                    case EntityState.Modified: entity.ModifiedDate = currentTime; break;
                }
            }
        }
    }
}