using PlayRank.Application.Core.ExternalDtos;
using PlayRank.Domain.Commont;

namespace PlayRank.Application.Core.Interfaces.External
{
    public interface IFootballTeamService
    {
        Task<ServiceResult<List<FootballTeamDto>>> GetAllFootballTeamsAsync();

        public Task<ServiceResult<List<FootballTeamDto>>> SearchFootballTeamsAsync(string query);
    }
}
