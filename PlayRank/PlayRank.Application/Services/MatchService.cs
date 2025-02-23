using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Matches.ViewModels;
using PlayRank.Domain.Commont;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;
using PlayRank.Data;
using PlayRank.Domain.Constants;

namespace PlayRank.Application.Core.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMapper _mapper;
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IRankingService _rankingService;
        private readonly PlayRankContext _context;

        public MatchService(IMapper mapper,
                            IMatchRepository matchRepository,
                            ITeamRepository teamRepository,
                            IRankingService rankingService,
                            PlayRankContext context)
        {
            _mapper = mapper;
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _rankingService = rankingService;
            _context = context;
        }

        public async Task<ServiceResult<IEnumerable<MatchViewModel>>> GetAllAsync()
        {
            var matches = await _matchRepository.GetAll()
                                                .Include(x => x.HomeTeam)
                                                .Include(x => x.AwayTeam)
                                                .ToListAsync();

            var viewModels = _mapper.Map<List<MatchViewModel>>(matches);
            return ServiceResult.Success<IEnumerable<MatchViewModel>>(viewModels);
        }

        public async Task<ServiceResult<MatchViewModel>> GetAsync(int id)
        {
            var match = await _matchRepository.GetAll()
                                              .Include(x => x.HomeTeam)
                                              .Include(x => x.AwayTeam)
                                              .Where(x => x.Id == id)
                                              .FirstOrDefaultAsync();
            var viewModel = _mapper.Map<MatchViewModel>(match);
            return ServiceResult.Success<MatchViewModel>(viewModel);
        }

        public async Task<ServiceResult<MatchViewModel>> CreateAsync(CreateMatchViewModel model)
        {
            var validation = ValidateMatchModel(model);
            if (!validation.Succeeded)
                return ServiceResult.Failed<MatchViewModel>(validation.Error);

            var homeTeam = await _teamRepository.GetAsync(model.HomeTeamId);
            if (homeTeam == null)
                return ServiceResult.Failed<MatchViewModel>(new ServiceError(ErrorMessages.HomeTeamNotFound, 400));
            var awayTeam = await _teamRepository.GetAsync(model.AwayTeamId);
            if (awayTeam == null)
                return ServiceResult.Failed<MatchViewModel>(new ServiceError(ErrorMessages.AwayTeamNotFound, 400));

            return await ExecuteMatchTransactionAsync(async () =>
            {
                var matchEntity = _mapper.Map<Match>(model);
                var createdMatch = await _matchRepository.AddAsync(matchEntity);

                if (createdMatch.IsOver)
                {
                    await ProcessRankingUpdateAsync(createdMatch);
                }
                return _mapper.Map<MatchViewModel>(createdMatch);
            });
        }

        public async Task<ServiceResult<MatchViewModel>> UpdateAsync(int id, CreateMatchViewModel model)
        {
            var validation = ValidateMatchModel(model);
            if (!validation.Succeeded)
                return ServiceResult.Failed<MatchViewModel>(validation.Error);

            var match = await _matchRepository.GetAsync(id);
            if (match == null)
                return ServiceResult.Failed<MatchViewModel>(new ServiceError(ErrorMessages.MatchNotFound, 404));

            var homeTeam = await _teamRepository.GetAsync(model.HomeTeamId);
            if (homeTeam == null)
                return ServiceResult.Failed<MatchViewModel>(new ServiceError(ErrorMessages.HomeTeamNotFound, 400));
            var awayTeam = await _teamRepository.GetAsync(model.AwayTeamId);
            if (awayTeam == null)
                return ServiceResult.Failed<MatchViewModel>(new ServiceError(ErrorMessages.AwayTeamNotFound, 400));

            return await ExecuteMatchTransactionAsync(async () =>
            {
                match.HomeTeamId = model.HomeTeamId;
                match.AwayTeamId = model.AwayTeamId;
                if (model.HomeTeamScore.HasValue)
                    match.HomeTeamScore = model.HomeTeamScore.Value;
                if (model.AwayTeamScore.HasValue)
                    match.AwayTeamScore = model.AwayTeamScore.Value;
                match.IsOver = model.IsOver;

                _matchRepository.Update(match);
                await _matchRepository.SaveChangesAsync();

                if (match.IsOver)
                {
                    await ProcessRankingUpdateAsync(match);
                }
                return _mapper.Map<MatchViewModel>(match);
            });
        }

        public async Task<ServiceResult<MatchViewModel>> DeleteAsync(int id)
        {
            return await ExecuteMatchTransactionAsync(async () =>
            {
                var match = await _matchRepository.GetAsync(id);
                if (match == null)
                    throw new Exception(ErrorMessages.MatchNotFound);

                var deleteCount = await _matchRepository.DeleteAsync(id);
                if (deleteCount <= 0)
                    throw new Exception(ErrorMessages.UnableToDeleteMatch);

                var rankingResult = await _rankingService.RecalculateRankingsAsync();
                if (!rankingResult.Succeeded)
                    throw new Exception(ErrorMessages.FailedToUpdateRankingAfterMatchDeleted);

                return _mapper.Map<MatchViewModel>(match);
            });
        }

        public async Task<ServiceResult<MatchPredictionViewModel>> PredictScoreAsync(int matchId)
        {
            // Retrieve the match.
            var match = await _matchRepository.GetAsync(matchId);
            if (match == null)
                return ServiceResult.Failed<MatchPredictionViewModel>(new ServiceError(ErrorMessages.MatchNotFound, 404));

            if (match.IsOver)
                return ServiceResult.Failed<MatchPredictionViewModel>(new ServiceError(ErrorMessages.MatchAlreadyOver, 400));

            var homeStatsResult = await _rankingService.GetTeamStatisticsAsync(match.HomeTeamId);
            var awayStatsResult = await _rankingService.GetTeamStatisticsAsync(match.AwayTeamId);

            if (!homeStatsResult.Succeeded || !awayStatsResult.Succeeded)
                return ServiceResult.Failed<MatchPredictionViewModel>(new ServiceError(ErrorMessages.UnableToRetrieveTeamRankings, 500));

            int predictedHomeScore, predictedAwayScore;
            if (homeStatsResult.Data.RankPosition < awayStatsResult.Data.RankPosition)
            {
                predictedHomeScore = 2;
                predictedAwayScore = 1;
            }
            else if (homeStatsResult.Data.RankPosition > awayStatsResult.Data.RankPosition)
            {
                predictedHomeScore = 1;
                predictedAwayScore = 2;
            }
            else
            {
                predictedHomeScore = 1;
                predictedAwayScore = 1;
            }

            var prediction = new MatchPredictionViewModel
            {
                MatchId = matchId,
                PredictedHomeScore = predictedHomeScore,
                PredictedAwayScore = predictedAwayScore,
                Prediction = "Prediction based on team rankings."
            };

            return ServiceResult.Success(prediction);
        }

        private ServiceResult<bool> ValidateMatchModel(CreateMatchViewModel model)
        {
            if (model.IsOver && (model.HomeTeamScore == null || model.AwayTeamScore == null))
            {
                return ServiceResult.Failed<bool>(new ServiceError(ErrorMessages.MatchScoreRequired, 400));
            }
            if (model.HomeTeamScore < 0 || model.AwayTeamScore < 0)
            {
                return ServiceResult.Failed<bool>(new ServiceError(ErrorMessages.NegativeScoreNotAllowed, 400));
            }
            return ServiceResult.Success(true);
        }

        private async Task<ServiceResult<MatchViewModel>> ExecuteMatchTransactionAsync(Func<Task<MatchViewModel>> operation)
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
                    return ServiceResult.Failed<MatchViewModel>(new ServiceError(ex.Message, 500));
                }
            }
        }

        private async Task ProcessRankingUpdateAsync(Match match)
        {
            var rankingResult = await _rankingService.UpdateRankingForMatchAsync(match.Id);
            if (!rankingResult.Succeeded)
            {
                throw new Exception(ErrorMessages.FailedToUptateRanking);
            }
        }
    }
}
