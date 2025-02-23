using PlayRank.Application.Core.Interfaces.Abstract;
using PlayRank.Application.Core.Matches.ViewModels;
using PlayRank.Domain.Commont;

namespace PlayRank.Application.Core.Interfaces
{
    public interface IMatchService : IService
    {
        public Task<ServiceResult<IEnumerable<MatchViewModel>>> GetAllAsync();

        public Task<ServiceResult<MatchViewModel>> GetAsync(int id);

        public Task<ServiceResult<MatchViewModel>> CreateAsync(CreateMatchViewModel model);

        public Task<ServiceResult<MatchViewModel>> UpdateAsync(int id, CreateMatchViewModel model);

        public Task<ServiceResult<MatchViewModel>> DeleteAsync(int id);

        Task<ServiceResult<MatchPredictionViewModel>> PredictScoreAsync(int matchId);
    }
}