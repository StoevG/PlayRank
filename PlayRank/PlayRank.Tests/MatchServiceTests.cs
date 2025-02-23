using AutoMapper;
using MockQueryable.Moq;
using Moq;
using PlayRank.Application.Core.Services;
using PlayRank.Application.Core.Matches.ViewModels;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;
using PlayRank.Domain.Commont;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Data;
using Match = PlayRank.Domain.Entities.Match;
using PlayRank.Tests.Helpers;

namespace PlayRank.Tests
{
    public class MatchServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<ITeamRepository> _teamRepoMock;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly PlayRankContext _context;
        private readonly MatchService _service;

        public MatchServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Match, MatchViewModel>();
            });
            _mapperMock.SetupGet(m => m.ConfigurationProvider).Returns(mapperConfig);

            _matchRepoMock = new Mock<IMatchRepository>();
            _teamRepoMock = new Mock<ITeamRepository>();
            _rankingServiceMock = new Mock<IRankingService>();

            _context = TestDbContextFactory.CreateContext("PlayRankDb");

            _service = new MatchService(
                _mapperMock.Object,
                _matchRepoMock.Object,
                _teamRepoMock.Object,
                _rankingServiceMock.Object,
                _context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var matchesList = new List<Match>
            {
                new Match { Id = 1 },
                new Match { Id = 2 }
            };
            var mockDbSet = matchesList.AsQueryable().BuildMockDbSet();
            _matchRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var matchViewModels = new List<MatchViewModel>
            {
                new MatchViewModel { Id = 1 },
                new MatchViewModel { Id = 2 }
            };
            _mapperMock.Setup(m => m.Map<List<MatchViewModel>>(It.IsAny<List<Match>>()))
                .Returns(matchViewModels);

            var result = await _service.GetAllAsync();

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetAsync_ReturnsSuccess()
        {
            int id = 1;
            var matchesList = new List<Match>
            {
                new Match { Id = id },
                new Match { Id = 2 }
            };
            var mockDbSet = matchesList.AsQueryable().BuildMockDbSet();
            _matchRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var matchViewModel = new MatchViewModel { Id = id };
            _mapperMock.Setup(m => m.Map<MatchViewModel>(It.IsAny<Match>()))
                .Returns(matchViewModel);

            var result = await _service.GetAsync(id);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_InvalidScores_WhenOver_ReturnsFailed()
        {
            var model = new CreateMatchViewModel { IsOver = true, HomeTeamScore = null, AwayTeamScore = 1 };

            var result = await _service.CreateAsync(model);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
        }

        [Fact]
        public async Task CreateAsync_NegativeScores_ReturnsFailed()
        {
            var model = new CreateMatchViewModel { IsOver = false, HomeTeamScore = -1, AwayTeamScore = 2 };

            var result = await _service.CreateAsync(model);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
        }

        [Fact]
        public async Task CreateAsync_HomeTeamNotFound_ReturnsFailed()
        {
            var model = new CreateMatchViewModel
            {
                IsOver = false,
                HomeTeamScore = 2,
                AwayTeamScore = 1,
                HomeTeamId = 1,
                AwayTeamId = 2
            };
            _teamRepoMock.Setup(t => t.GetAsync(model.HomeTeamId)).ReturnsAsync((Team)null);

            var result = await _service.CreateAsync(model);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
            Assert.Contains("Home Team does not exist", result.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_AwayTeamNotFound_ReturnsFailed()
        {
            var model = new CreateMatchViewModel
            {
                IsOver = false,
                HomeTeamScore = 2,
                AwayTeamScore = 1,
                HomeTeamId = 1,
                AwayTeamId = 2
            };
            _teamRepoMock.Setup(t => t.GetAsync(model.HomeTeamId)).ReturnsAsync(new Team { Id = 1 });
            _teamRepoMock.Setup(t => t.GetAsync(model.AwayTeamId)).ReturnsAsync((Team)null);

            var result = await _service.CreateAsync(model);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
            Assert.Contains("Away Team does not exist", result.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_SuccessfulCreation_MatchNotOver_ReturnsSuccess()
        {
            var model = new CreateMatchViewModel
            {
                IsOver = false,
                HomeTeamScore = 2,
                AwayTeamScore = 1,
                HomeTeamId = 1,
                AwayTeamId = 2
            };
            _teamRepoMock.Setup(t => t.GetAsync(model.HomeTeamId)).ReturnsAsync(new Team { Id = 1 });
            _teamRepoMock.Setup(t => t.GetAsync(model.AwayTeamId)).ReturnsAsync(new Team { Id = 2 });

            var createdMatch = new Match { Id = 10, IsOver = false };
            _matchRepoMock.Setup(m => m.AddAsync(It.IsAny<Match>())).ReturnsAsync(createdMatch);

            var matchViewModel = new MatchViewModel { Id = 10 };
            _mapperMock.Setup(m => m.Map<MatchViewModel>(createdMatch)).Returns(matchViewModel);

            var result = await _service.CreateAsync(model);

            Assert.True(result.Succeeded);
            Assert.Equal(10, result.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_SuccessfulCreation_MatchIsOver_ReturnsSuccess()
        {
            var model = new CreateMatchViewModel
            {
                IsOver = true,
                HomeTeamScore = 3,
                AwayTeamScore = 1,
                HomeTeamId = 1,
                AwayTeamId = 2
            };
            _teamRepoMock.Setup(t => t.GetAsync(model.HomeTeamId)).ReturnsAsync(new Team { Id = 1 });
            _teamRepoMock.Setup(t => t.GetAsync(model.AwayTeamId)).ReturnsAsync(new Team { Id = 2 });

            var createdMatch = new Match { Id = 11, IsOver = true };
            _matchRepoMock.Setup(m => m.AddAsync(It.IsAny<Match>())).ReturnsAsync(createdMatch);
            _rankingServiceMock.Setup(r => r.UpdateRankingForMatchAsync(createdMatch.Id))
                .ReturnsAsync(ServiceResult.Success(true));

            var matchViewModel = new MatchViewModel { Id = 11 };
            _mapperMock.Setup(m => m.Map<MatchViewModel>(createdMatch)).Returns(matchViewModel);

            var result = await _service.CreateAsync(model);

            Assert.True(result.Succeeded);
            Assert.Equal(11, result.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_FailedRankingUpdate_ReturnsFailed()
        {
            var model = new CreateMatchViewModel
            {
                IsOver = true,
                HomeTeamScore = 3,
                AwayTeamScore = 1,
                HomeTeamId = 1,
                AwayTeamId = 2
            };
            _teamRepoMock.Setup(t => t.GetAsync(model.HomeTeamId)).ReturnsAsync(new Team { Id = 1 });
            _teamRepoMock.Setup(t => t.GetAsync(model.AwayTeamId)).ReturnsAsync(new Team { Id = 2 });

            var createdMatch = new Match { Id = 12, IsOver = true };
            _matchRepoMock.Setup(m => m.AddAsync(It.IsAny<Match>())).ReturnsAsync(createdMatch);
            _rankingServiceMock.Setup(r => r.UpdateRankingForMatchAsync(createdMatch.Id))
                .ReturnsAsync(ServiceResult.Failed<bool>(new ServiceError("Ranking update failed", 500)));

            var result = await _service.CreateAsync(model);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
        }

        [Fact]
        public async Task DeleteAsync_MatchNotFound_ReturnsFailed()
        {
            int matchId = 5;
            _matchRepoMock.Setup(m => m.GetAsync(matchId)).ReturnsAsync((Match)null);

            var result = await _service.DeleteAsync(matchId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Match not found", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_DeleteFails_ReturnsFailed()
        {
            int matchId = 6;
            var match = new Match { Id = matchId };
            _matchRepoMock.Setup(m => m.GetAsync(matchId)).ReturnsAsync(match);
            _matchRepoMock.Setup(m => m.DeleteAsync(matchId)).ReturnsAsync(0);

            var result = await _service.DeleteAsync(matchId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Unable to delete match", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_FailedRankingRecalc_ReturnsFailed()
        {
            int matchId = 7;
            var match = new Match { Id = matchId };
            _matchRepoMock.Setup(m => m.GetAsync(matchId)).ReturnsAsync(match);
            _matchRepoMock.Setup(m => m.DeleteAsync(matchId)).ReturnsAsync(1);
            _rankingServiceMock.Setup(r => r.RecalculateRankingsAsync())
                .ReturnsAsync(ServiceResult.Failed<bool>(new ServiceError("Ranking recalc failed", 500)));

            var result = await _service.DeleteAsync(matchId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Match deleted, but failed to update ranking", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_Success_ReturnsSuccess()
        {
            int matchId = 8;
            var match = new Match { Id = matchId };
            _matchRepoMock.Setup(m => m.GetAsync(matchId)).ReturnsAsync(match);
            _matchRepoMock.Setup(m => m.DeleteAsync(matchId)).ReturnsAsync(1);
            _rankingServiceMock.Setup(r => r.RecalculateRankingsAsync())
                .ReturnsAsync(ServiceResult.Success(true));

            var matchViewModel = new MatchViewModel { Id = matchId };
            _mapperMock.Setup(m => m.Map<MatchViewModel>(match)).Returns(matchViewModel);

            var result = await _service.DeleteAsync(matchId);

            Assert.True(result.Succeeded);
            Assert.Equal(matchId, result.Data.Id);
        }
    }
}