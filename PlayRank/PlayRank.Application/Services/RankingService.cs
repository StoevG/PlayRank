using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Rankings.ViewModels;
using PlayRank.Domain.Commont;
using PlayRank.Domain.Constants;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;

namespace PlayRank.Application.Core.Services
{
    public class RankingService : IRankingService
    {
        private readonly IMapper _mapper;
        private readonly IRankingRepository _rankingRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;

        public RankingService(
            IMapper mapper,
            IRankingRepository rankingRepository,
            IMatchRepository matchRepository,
            ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _rankingRepository = rankingRepository;
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetAllAsync()
        {
            var rankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Wins)
                .ToListAsync();

            var result = _mapper.Map<List<RankingViewModel>>(rankings);
            var rankDict = await GetTeamRankPositionsAsync();
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsAsync(int count)
        {
            var rankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Wins)
                .Take(count)
                .ToListAsync();

            var result = _mapper.Map<List<RankingViewModel>>(rankings);
            var rankDict = await GetTeamRankPositionsAsync();
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<TeamStatisticViewModel>> GetTeamStatisticsAsync(int teamId)
        {
            var ranking = await _rankingRepository.Find(r => r.TeamId == teamId)
                .Include(x => x.Team)
                .FirstOrDefaultAsync();
            if (ranking == null)
            {
                return ServiceResult.Failed<TeamStatisticViewModel>(new ServiceError(ErrorMessages.TeamRankingNotFound, 404));

            }
            var result = _mapper.Map<TeamStatisticViewModel>(ranking);
            var rankDict = await GetTeamRankPositionsAsync();
            if (rankDict.TryGetValue(teamId, out int rank))
            {
                result.RankPosition = rank;

            }
            else
            {
                result.RankPosition = 0;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<int>> GetTeamRankPositionAsync(int teamId)
        {
            var rankDict = await GetTeamRankPositionsAsync();
            if (rankDict.TryGetValue(teamId, out int rank))
                return ServiceResult.Success(rank);
            return ServiceResult.Failed<int>(new ServiceError(ErrorMessages.TeamNotFoundInRanking, 404));
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetLeaderboardPagedAsync(int page, int pageSize)
        {
            var allRankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Wins)
                .ToListAsync();

            var pagedRankings = allRankings.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var result = _mapper.Map<List<RankingViewModel>>(pagedRankings);

            var rankDict = allRankings
                .Select((r, index) => new { r.TeamId, Rank = index + 1 })
                .ToDictionary(x => x.TeamId, x => x.Rank);
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByWinsAsync(int count)
        {
            var rankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderByDescending(r => r.Wins)
                .Take(count)
                .ToListAsync();

            var result = _mapper.Map<List<RankingViewModel>>(rankings);
            var rankDict = await GetTeamRankPositionsAsync();
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByDrawsAsync(int count)
        {
            var rankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderByDescending(r => r.Draws)
                .Take(count)
                .ToListAsync();

            var result = _mapper.Map<List<RankingViewModel>>(rankings);
            var rankDict = await GetTeamRankPositionsAsync();
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<List<RankingViewModel>>> GetTopTeamsByLeastLossesAsync(int count)
        {
            var rankings = await _rankingRepository.GetAll()
                .Include(x => x.Team)
                .OrderBy(r => r.Losses)
                .Take(count)
                .ToListAsync();

            var result = _mapper.Map<List<RankingViewModel>>(rankings);
            var rankDict = await GetTeamRankPositionsAsync();
            foreach (var vm in result)
            {
                if (rankDict.TryGetValue(vm.TeamId, out int rank))
                    vm.RankPosition = rank;
            }
            return ServiceResult.Success(result);
        }

        public async Task<ServiceResult<bool>> RecalculateRankingsAsync()
        {
            var teams = await _teamRepository.GetAll().ToListAsync();
            var allRankings = await _rankingRepository.GetAll().ToListAsync();

            foreach (var team in teams)
            {
                if (!allRankings.Any(r => r.TeamId == team.Id))
                {
                    var newRanking = new Ranking
                    {
                        TeamId = team.Id,
                        Wins = 0,
                        Draws = 0,
                        Losses = 0,
                        PlayedGames = 0,
                        Points = 0
                    };
                    await _rankingRepository.AddAsync(newRanking);
                    allRankings.Add(newRanking);
                }
            }

            foreach (var ranking in allRankings)
            {
                ranking.Wins = 0;
                ranking.Draws = 0;
                ranking.Losses = 0;
                ranking.PlayedGames = 0;
                ranking.Points = 0;
            }

            var finishedMatches = await _matchRepository.Find(m => m.IsOver).ToListAsync();
            foreach (var match in finishedMatches)
            {
                var homeRanking = allRankings.FirstOrDefault(r => r.TeamId == match.HomeTeamId);
                if (homeRanking == null)
                {
                    homeRanking = new Ranking { TeamId = match.HomeTeamId, Wins = 0, Draws = 0, Losses = 0, PlayedGames = 0, Points = 0 };
                    await _rankingRepository.AddAsync(homeRanking);
                    allRankings.Add(homeRanking);
                }
                var awayRanking = allRankings.FirstOrDefault(r => r.TeamId == match.AwayTeamId);
                if (awayRanking == null)
                {
                    awayRanking = new Ranking { TeamId = match.AwayTeamId, Wins = 0, Draws = 0, Losses = 0, PlayedGames = 0, Points = 0 };
                    await _rankingRepository.AddAsync(awayRanking);
                    allRankings.Add(awayRanking);
                }

                homeRanking.PlayedGames++;
                awayRanking.PlayedGames++;

                UpdateMatchResults(match, homeRanking, awayRanking);
            }

            await _rankingRepository.SaveChangesAsync();
            return ServiceResult.Success(true);
        }

        public async Task<ServiceResult<bool>> UpdateRankingForMatchAsync(int matchId)
        {
            var match = await _matchRepository.GetAsync(matchId);
            if (match == null)
                return ServiceResult.Failed<bool>(new ServiceError(ErrorMessages.MatchNotFound, 404));

            if (!match.IsOver)
                return ServiceResult.Failed<bool>(new ServiceError(ErrorMessages.MatchNotOver, 400));

            var allRankings = await _rankingRepository.GetAll().ToListAsync();
            var homeRanking = await GetOrCreateRankingAsync(allRankings, match.HomeTeamId);
            var awayRanking = await GetOrCreateRankingAsync(allRankings, match.AwayTeamId);

            homeRanking.PlayedGames++;
            awayRanking.PlayedGames++;

            UpdateMatchResults(match, homeRanking, awayRanking);

            await _rankingRepository.SaveChangesAsync();
            return ServiceResult.Success(true);
        }

        private async Task<Dictionary<int, int>> GetTeamRankPositionsAsync()
        {
            var sortedRankings = await _rankingRepository.GetAll()
                .Include(r => r.Team)
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.Wins)
                .ToListAsync();

            return sortedRankings
                .Select((r, index) => new { r.TeamId, RankPosition = index + 1 })
                .ToDictionary(x => x.TeamId, x => x.RankPosition);
        }

        private async Task<Ranking> GetOrCreateRankingAsync(List<Ranking> allRankings, int teamId)
        {
            var ranking = allRankings.FirstOrDefault(r => r.TeamId == teamId);
            if (ranking == null)
            {
                ranking = new Ranking { TeamId = teamId };
                await _rankingRepository.AddAsync(ranking);
                allRankings.Add(ranking);
            }
            return ranking;
        }

        private void UpdateMatchResults(Match match, Ranking homeRanking, Ranking awayRanking)
        {
            if (match.HomeTeamScore > match.AwayTeamScore)
            {
                homeRanking.Wins++;
                homeRanking.Points += 3;
                awayRanking.Losses++;
            }
            else if (match.HomeTeamScore < match.AwayTeamScore)
            {
                awayRanking.Wins++;
                awayRanking.Points += 3;
                homeRanking.Losses++;
            }
            else
            {
                homeRanking.Draws++;
                awayRanking.Draws++;
                homeRanking.Points += 1;
                awayRanking.Points += 1;
            }
        }
    }
}
