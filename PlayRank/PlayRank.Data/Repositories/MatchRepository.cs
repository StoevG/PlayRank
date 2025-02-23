using PlayRank.Data.Repositories.Abstract;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;

namespace PlayRank.Data.Repositories
{
    public class MatchRepository : BaseRepository<Match>, IMatchRepository
    {
        public MatchRepository(PlayRankContext context) : base(context) { }
    }
}
