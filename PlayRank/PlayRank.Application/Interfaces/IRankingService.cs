using PlayRank.Application.Core.Interfaces.Abstract;
using PlayRank.Application.Core.Rankings.ViewModels;
using PlayRank.Domain.Commont;

namespace PlayRank.Application.Core.Interfaces
{
    public interface IRankingService : IService
    {
        Task<ServiceResult<List<RankingViewModel>>> GetAllAsync();

        Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsAsync(int count);

        Task<ServiceResult<TeamStatisticViewModel>> GetTeamStatisticsAsync(int teamId);

        Task<ServiceResult<int>> GetTeamRankPositionAsync(int teamId);

        Task<ServiceResult<List<RankingViewModel>>> GetLeaderboardPagedAsync(int page, int pageSize);

        Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByWinsAsync(int count);

        Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByDrawsAsync(int count);

        Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByLeastLossesAsync(int count);

        Task<ServiceResult<bool>> RecalculateRankingsAsync();

        Task<ServiceResult<bool>> UpdateRankingForMatchAsync(int matchId);
    }
}
