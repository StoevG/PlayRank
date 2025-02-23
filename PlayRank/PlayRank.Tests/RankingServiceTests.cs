using AutoMapper;
using MockQueryable.Moq;
using Moq;
using PlayRank.Application.Core.Services;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;
using Match = PlayRank.Domain.Entities.Match;
using PlayRank.Application.Core.Rankings.ViewModels;
using System.Linq.Expressions;

namespace PlayRank.Tests
{
    public class RankingServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRankingRepository> _rankingRepoMock;
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<ITeamRepository> _teamRepoMock;
        private readonly RankingService _service;

        public RankingServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Ranking, RankingViewModel>();
                cfg.CreateMap<Ranking, TeamStatisticViewModel>();
            });
            _mapperMock.SetupGet(m => m.ConfigurationProvider).Returns(mapperConfig);
            _mapperMock.Setup(m => m.Map<List<RankingViewModel>>(It.IsAny<List<Ranking>>()))
                .Returns((List<Ranking> src) =>
                    src.Select(r => new RankingViewModel
                    {
                        TeamId = r.TeamId,
                        Points = r.Points,
                        Wins = r.Wins
                    }).ToList());
            _mapperMock.Setup(m => m.Map<TeamStatisticViewModel>(It.IsAny<Ranking>()))
                .Returns((Ranking r) => new TeamStatisticViewModel
                {
                    TeamId = r.TeamId,
                    Points = r.Points,
                    Wins = r.Wins
                });

            _rankingRepoMock = new Mock<IRankingRepository>();
            _matchRepoMock = new Mock<IMatchRepository>();
            _teamRepoMock = new Mock<ITeamRepository>();

            _rankingRepoMock.Setup(r => r.AddAsync(It.IsAny<Ranking>()))
                .ReturnsAsync((Ranking r) => r);
            _rankingRepoMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _service = new RankingService(
                _mapperMock.Object,
                _rankingRepoMock.Object,
                _matchRepoMock.Object,
                _teamRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 10, Wins = 3 },
                new Ranking { TeamId = 2, Points = 5, Wins = 1 },
                new Ranking { TeamId = 3, Points = 8, Wins = 2 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var result = await _service.GetAllAsync();

            Assert.True(result.Succeeded);
            Assert.Equal(3, result.Data.Count);

            var sorted = rankingsList.OrderByDescending(r => r.Points).ThenByDescending(r => r.Wins).ToList();
            var rankDict = sorted.Select((r, index) => new { r.TeamId, Rank = index + 1 })
                                 .ToDictionary(x => x.TeamId, x => x.Rank);
            foreach (var vm in result.Data)
            {
                Assert.Equal(rankDict[vm.TeamId], vm.RankPosition);
            }
        }

        [Fact]
        public async Task GetTopTeamsAsync_ReturnsSuccess()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 10, Wins = 3 },
                new Ranking { TeamId = 2, Points = 8, Wins = 2 },
                new Ranking { TeamId = 3, Points = 5, Wins = 1 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);
            int count = 2;

            var result = await _service.GetTopTeamsAsync(count);

            Assert.True(result.Succeeded);
            Assert.Equal(count, result.Data.Count);
        }

        [Fact]
        public async Task GetTeamStatisticsAsync_NonExistingTeam_ReturnsFailed()
        {
            _rankingRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Ranking, bool>>>()))
                .Returns(new List<Ranking>().AsQueryable().BuildMockDbSet().Object);

            var result = await _service.GetTeamStatisticsAsync(99);

            Assert.False(result.Succeeded);
            Assert.Equal(404, result.Error.Code);
        }

        [Fact]
        public async Task GetTeamRankPositionAsync_ExistingTeam_ReturnsSuccess()
        {
            int teamId = 2;
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 10, Wins = 3 },
                new Ranking { TeamId = teamId, Points = 8, Wins = 2 },
                new Ranking { TeamId = 3, Points = 5, Wins = 1 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var result = await _service.GetTeamRankPositionAsync(teamId);

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data);
        }

        [Fact]
        public async Task GetTeamRankPositionAsync_NonExistingTeam_ReturnsFailed()
        {
            int teamId = 99;
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 10, Wins = 3 },
                new Ranking { TeamId = 2, Points = 8, Wins = 2 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var result = await _service.GetTeamRankPositionAsync(teamId);

            Assert.False(result.Succeeded);
            Assert.Equal(404, result.Error.Code);
        }

        [Fact]
        public async Task GetLeaderboardPagedAsync_ReturnsCorrectPage()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 10, Wins = 3 },
                new Ranking { TeamId = 2, Points = 8, Wins = 2 },
                new Ranking { TeamId = 3, Points = 5, Wins = 1 },
                new Ranking { TeamId = 4, Points = 4, Wins = 1 },
                new Ranking { TeamId = 5, Points = 2, Wins = 0 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            int page = 2, pageSize = 2;

            var result = await _service.GetLeaderboardPagedAsync(page, pageSize);

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
            var expectedTeamIds = rankingsList
                .OrderByDescending(r => r.Points).ThenByDescending(r => r.Wins)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => r.TeamId)
                .ToList();
            Assert.Equal(expectedTeamIds, result.Data.Select(vm => vm.TeamId).ToList());
        }

        [Fact]
        public async Task GetTopTeamsByWinsAsync_ReturnsSuccess()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Wins = 5, Points = 15 },
                new Ranking { TeamId = 2, Wins = 3, Points = 9 },
                new Ranking { TeamId = 3, Wins = 4, Points = 12 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);
            int count = 2;

            var result = await _service.GetTopTeamsByWinsAsync(count);

            Assert.True(result.Succeeded);
            Assert.Equal(count, result.Data.Count);
        }

        [Fact]
        public async Task GetTopTeamsByDrawsAsync_ReturnsSuccess()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Draws = 4, Points = 8 },
                new Ranking { TeamId = 2, Draws = 2, Points = 4 },
                new Ranking { TeamId = 3, Draws = 3, Points = 6 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);
            int count = 2;

            var result = await _service.GetTopTeamsByDrawsAsync(count);

            Assert.True(result.Succeeded);
            Assert.Equal(count, result.Data.Count);
        }

        [Fact]
        public async Task GetTopTeamsByLeastLossesAsync_ReturnsSuccess()
        {
            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Losses = 2, Points = 6 },
                new Ranking { TeamId = 2, Losses = 1, Points = 3 },
                new Ranking { TeamId = 3, Losses = 3, Points = 2 }
            };
            var mockDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);
            int count = 2;

            var result = await _service.GetTopTeamsByLeastLossesAsync(count);

            Assert.True(result.Succeeded);
            Assert.Equal(count, result.Data.Count);
        }

        [Fact]
        public async Task RecalculateRankingsAsync_UpdatesRankingsSuccessfully()
        {
            var teamsList = new List<Team>
            {
                new Team { Id = 1 },
                new Team { Id = 2 }
            };
            var teamsDbSet = teamsList.AsQueryable().BuildMockDbSet();
            _teamRepoMock.Setup(r => r.GetAll()).Returns(teamsDbSet.Object);

            var rankingsList = new List<Ranking>();
            var rankingsDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(rankingsDbSet.Object);

            var matchesList = new List<Match>
            {
                new Match { Id = 1, IsOver = true, HomeTeamId = 1, AwayTeamId = 2, HomeTeamScore = 2, AwayTeamScore = 1 }
            };
            var matchesDbSet = matchesList.AsQueryable().BuildMockDbSet();
            _matchRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Match, bool>>>()))
                .Returns(matchesList.AsQueryable().BuildMockDbSet().Object);

            var result = await _service.RecalculateRankingsAsync();

            Assert.True(result.Succeeded);
            _rankingRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRankingForMatchAsync_MatchNotFound_ReturnsFailed()
        {
            int matchId = 5;
            _matchRepoMock.Setup(r => r.GetAsync(matchId)).ReturnsAsync((Match)null);

            var result = await _service.UpdateRankingForMatchAsync(matchId);

            Assert.False(result.Succeeded);
            Assert.Equal(404, result.Error.Code);
        }

        [Fact]
        public async Task UpdateRankingForMatchAsync_MatchNotOver_ReturnsFailed()
        {
            int matchId = 6;
            var match = new Match { Id = matchId, IsOver = false };
            _matchRepoMock.Setup(r => r.GetAsync(matchId)).ReturnsAsync(match);

            var result = await _service.UpdateRankingForMatchAsync(matchId);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
            Assert.Contains("Match is not over", result.Error.Message);
        }

        [Fact]
        public async Task UpdateRankingForMatchAsync_Success_ReturnsSuccess()
        {
            int matchId = 7;
            var match = new Match { Id = matchId, IsOver = true, HomeTeamId = 1, AwayTeamId = 2, HomeTeamScore = 3, AwayTeamScore = 1 };
            _matchRepoMock.Setup(r => r.GetAsync(matchId)).ReturnsAsync(match);

            var rankingsList = new List<Ranking>
            {
                new Ranking { TeamId = 1, Points = 0, Wins = 0, Losses = 0, Draws = 0, PlayedGames = 0 },
                new Ranking { TeamId = 2, Points = 0, Wins = 0, Losses = 0, Draws = 0, PlayedGames = 0 }
            };
            var rankingsDbSet = rankingsList.AsQueryable().BuildMockDbSet();
            _rankingRepoMock.Setup(r => r.GetAll()).Returns(rankingsDbSet.Object);

            var result = await _service.UpdateRankingForMatchAsync(matchId);

            Assert.True(result.Succeeded);
        }
    }
}
