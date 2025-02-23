using PlayRank.Application.Core.Interfaces.Abstract;
using PlayRank.Application.Core.Teams.ViewModels;
using PlayRank.Domain.Commont;

namespace PlayRank.Application.Core.Interfaces
{
    public interface ITeamService : IService
    {
        public Task<ServiceResult<IEnumerable<TeamViewModel>>> GetAllAsync();

        public Task<ServiceResult<TeamViewModel>> GetAsync(int id);

        public Task<ServiceResult<TeamViewModel>> CreateAsync(CreateTeamViewModel model);

        public Task<ServiceResult<TeamViewModel>> UpdateAsync(int id, CreateTeamViewModel model);

        public Task<ServiceResult<TeamViewModel>> DeleteAsync(int id);
    }
}