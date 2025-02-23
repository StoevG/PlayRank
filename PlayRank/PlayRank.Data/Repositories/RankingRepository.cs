using PlayRank.Data.Repositories.Abstract;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;

namespace PlayRank.Data.Repositories
{
    public class RankingRepository : BaseRepository<Ranking>, IRankingRepository
    {
        public RankingRepository(PlayRankContext context) : base(context) { }
    }
}
