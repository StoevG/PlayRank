using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PlayRank.Data;

namespace PlayRank.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static PlayRankContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<PlayRankContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new PlayRankContext(options);
        }
    }
}
