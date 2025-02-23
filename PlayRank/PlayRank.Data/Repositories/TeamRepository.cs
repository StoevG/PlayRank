using PlayRank.Data.Repositories.Abstract;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;

namespace PlayRank.Data.Repositories
{
    public class TeamRepository : BaseRepository<Team>, ITeamRepository 
    {
        public TeamRepository(PlayRankContext context) : base(context) { }
    }
}
