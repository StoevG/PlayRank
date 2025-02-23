using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Teams.ViewModels;
using PlayRank.Domain.Commont;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;
using PlayRank.Data;
using AutoMapper.QueryableExtensions;
using PlayRank.Domain.Constants;

namespace PlayRank.Application.Core.Services
{
    public class TeamService : ITeamService
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IRankingService _rankingService;
        private readonly PlayRankContext _context;

        public TeamService(
            IMapper mapper,
            ITeamRepository teamRepository,
            IMatchRepository matchRepository,
            IRankingService rankingService,
            PlayRankContext context)
        {
            _mapper = mapper;
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _rankingService = rankingService;
            _context = context;
        }

        public async Task<ServiceResult<TeamViewModel>> CreateAsync(CreateTeamViewModel model)
        {
            var existingTeam = await _teamRepository.Find(x => x.Name == model.Name).FirstOrDefaultAsync();
            if (existingTeam != null)
            {
                return ServiceResult.Failed<TeamViewModel>(new ServiceError(ErrorMessages.TeamAlreadyExists, 400));
            }

            return await ExecuteTeamTransactionAsync(async () =>
            {
                var team = await _teamRepository.AddAsync(_mapper.Map<Team>(model));

                var rankingResult = await _rankingService.RecalculateRankingsAsync();
                if (!rankingResult.Succeeded)
                {
                    throw new Exception(ErrorMessages.TeamCreatedButFailedRanking);
                }

                return _mapper.Map<TeamViewModel>(team);
            });
        }

        public async Task<ServiceResult<IEnumerable<TeamViewModel>>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAll()
                                               .ProjectTo<TeamViewModel>(_mapper.ConfigurationProvider)
                                               .ToListAsync();

            return ServiceResult.Success<IEnumerable<TeamViewModel>>(teams);
        }

        public async Task<ServiceResult<TeamViewModel>> GetAsync(int id)
        {
            var team = await _teamRepository.GetAsync(id);
            var viewModel = _mapper.Map<TeamViewModel>(team);
            return ServiceResult.Success(viewModel);
        }

        public async Task<ServiceResult<TeamViewModel>> UpdateAsync(int id, CreateTeamViewModel model)
        {
            var team = await _teamRepository.GetAsync(id);
            if (team == null)
            {
                return ServiceResult.Failed<TeamViewModel>(new ServiceError(ErrorMessages.TeamDoesNotExist, 400));
            }

            team.Name = model.Name;
            _teamRepository.Update(team);
            await _teamRepository.SaveChangesAsync();

            return ServiceResult.Success(_mapper.Map<TeamViewModel>(team));
        }

        public async Task<ServiceResult<TeamViewModel>> DeleteAsync(int id)
        {
            return await ExecuteTeamTransactionAsync(async () =>
            {
                var team = await _teamRepository.GetAsync(id);
                if (team == null)
                {
                    throw new Exception(ErrorMessages.TeamNotFound);
                }

                var matchesToDelete = await _matchRepository.Find(m => m.HomeTeamId == id || m.AwayTeamId == id).ToListAsync();
                foreach (var match in matchesToDelete)
                {
                    await _matchRepository.DeleteAsync(match.Id);
                }

                var deleteCount = await _teamRepository.DeleteAsync(id);
                if (deleteCount <= 0)
                {
                    throw new Exception(ErrorMessages.UnableToDeleteTeam);
                }

                var rankingResult = await _rankingService.RecalculateRankingsAsync();
                if (!rankingResult.Succeeded)
                {
                    throw new Exception(ErrorMessages.TeamDeletedButFailedRanking);
                }

                return _mapper.Map<TeamViewModel>(team);
            });
        }

        private async Task<ServiceResult<TeamViewModel>> ExecuteTeamTransactionAsync(Func<Task<TeamViewModel>> operation)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return ServiceResult.Success(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Failed<TeamViewModel>(new ServiceError(ex.Message, 500));
                }
            }
        }
    }
}
