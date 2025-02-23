using System.Linq.Expressions;
using AutoMapper;
using Moq;
using MockQueryable.Moq;
using PlayRank.Application.Core.Services;
using PlayRank.Application.Core.Teams.ViewModels;
using PlayRank.Domain.Entities;
using PlayRank.Domain.Interfaces.Repositories;
using PlayRank.Domain.Commont;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Tests.Helpers;
using PlayRank.Data;
using MockQueryable;
using Match = PlayRank.Domain.Entities.Match;

namespace PlayRank.Tests
{
    public class TeamServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITeamRepository> _teamRepoMock;
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly PlayRankContext _context;
        private readonly TeamService _service;

        public TeamServiceTests()
        {
            // Set up a minimal AutoMapper configuration.
            _mapperMock = new Mock<IMapper>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Team, TeamViewModel>();
                cfg.CreateMap<CreateTeamViewModel, Team>();
            });
            _mapperMock.SetupGet(m => m.ConfigurationProvider).Returns(mapperConfig);

            // Also set up simple mapping for Update and Create results.
            _mapperMock.Setup(m => m.Map<TeamViewModel>(It.IsAny<Team>()))
                .Returns((Team t) => new TeamViewModel { Id = t.Id, Name = t.Name });

            _teamRepoMock = new Mock<ITeamRepository>();
            _matchRepoMock = new Mock<IMatchRepository>();
            _rankingServiceMock = new Mock<IRankingService>();

            // Use TestDbContextFactory to create a real in-memory context.
            _context = TestDbContextFactory.CreateContext("TeamServiceTestsDb");

            _service = new TeamService(
                _mapperMock.Object,
                _teamRepoMock.Object,
                _matchRepoMock.Object,
                _rankingServiceMock.Object,
                _context);
        }

        [Fact]
        public async Task CreateAsync_TeamAlreadyExists_ReturnsFailedResult()
        {
            var existingTeam = new Team { Id = 1, Name = "Existing Team" };
            var teamsList = new List<Team> { existingTeam };

            var teamsQueryable = teamsList.AsQueryable().BuildMock();

            _teamRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Team, bool>>>()))
                .Returns((Expression<Func<Team, bool>> predicate) =>
                    teamsQueryable.Where(predicate));

            var createModel = new CreateTeamViewModel { Name = "Existing Team" };

            var result = await _service.CreateAsync(createModel);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
            Assert.Contains("already exists", result.Error.Message);
        }

        [Fact]
        public async Task CreateAsync_Success_ReturnsSuccess()
        {
            var emptyTeams = new List<Team>();
            var teamsDbSetMock = emptyTeams.AsQueryable().BuildMockDbSet();
            _teamRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Team, bool>>>()))
                .Returns(teamsDbSetMock.Object);

            var newTeam = new Team { Id = 2, Name = "New Team" };
            _teamRepoMock.Setup(r => r.AddAsync(It.IsAny<Team>()))
                .ReturnsAsync(newTeam);

            _rankingServiceMock.Setup(r => r.RecalculateRankingsAsync())
                .ReturnsAsync(ServiceResult.Success(true));

            var createModel = new CreateTeamViewModel { Name = "New Team" };

            var result = await _service.CreateAsync(createModel);

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Id);
            Assert.Equal("New Team", result.Data.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccess()
        {
            var teamsList = new List<Team>
            {
                new Team { Id = 1, Name = "Team One" },
                new Team { Id = 2, Name = "Team Two" }
            };
            var mockDbSet = teamsList.AsQueryable().BuildMockDbSet();
            _teamRepoMock.Setup(r => r.GetAll()).Returns(mockDbSet.Object);

            var result = await _service.GetAllAsync();

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetAsync_ExistingTeam_ReturnsSuccess()
        {
            int teamId = 1;
            var team = new Team { Id = teamId, Name = "Team One" };
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync(team);

            var result = await _service.GetAsync(teamId);

            Assert.True(result.Succeeded);
            Assert.Equal(teamId, result.Data.Id);
            Assert.Equal("Team One", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_TeamNotFound_ReturnsFailedResult()
        {
            int teamId = 99;
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync((Team)null);

            var updateModel = new CreateTeamViewModel { Name = "Updated Name" };

            var result = await _service.UpdateAsync(teamId, updateModel);

            Assert.False(result.Succeeded);
            Assert.Equal(400, result.Error.Code);
            Assert.Contains("Team does not exist", result.Error.Message);
        }

        [Fact]
        public async Task UpdateAsync_Success_ReturnsSuccess()
        {
            int teamId = 1;
            var team = new Team { Id = teamId, Name = "Old Name" };
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync(team);

            _teamRepoMock.Setup(r => r.Update(It.IsAny<Team>()));
            _teamRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var updateModel = new CreateTeamViewModel { Name = "New Name" };

            var result = await _service.UpdateAsync(teamId, updateModel);

            Assert.True(result.Succeeded);
            Assert.Equal(teamId, result.Data.Id);
            Assert.Equal("New Name", result.Data.Name);
        }

        [Fact]
        public async Task DeleteAsync_TeamNotFound_ReturnsFailedResult()
        {
            int teamId = 5;
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync((Team)null);

            var result = await _service.DeleteAsync(teamId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Team not found", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_DeleteFails_ReturnsFailedResult()
        {
            int teamId = 6;
            var team = new Team { Id = teamId, Name = "Team Six" };
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync(team);

            var emptyMatches = new List<Match>().AsQueryable().BuildMockDbSet().Object;
            _matchRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Match, bool>>>()))
                .Returns(emptyMatches);

            _teamRepoMock.Setup(r => r.DeleteAsync(teamId)).ReturnsAsync(0);

            var result = await _service.DeleteAsync(teamId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Unable to delete team", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_RankingRecalcFails_ReturnsFailedResult()
        {
            int teamId = 7;
            var team = new Team { Id = teamId, Name = "Team Seven" };
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync(team);

            var matchesList = new List<Match>
            {
                new Match { Id = 101, HomeTeamId = teamId, AwayTeamId = 2 }
            };
            var mockMatches = matchesList.AsQueryable().BuildMockDbSet();
            _matchRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Match, bool>>>()))
                .Returns(mockMatches.Object);

            _matchRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(1);
            _teamRepoMock.Setup(r => r.DeleteAsync(teamId)).ReturnsAsync(1);

            _rankingServiceMock.Setup(r => r.RecalculateRankingsAsync())
                .ReturnsAsync(ServiceResult.Failed<bool>(new ServiceError("Ranking update failed", 500)));

            var result = await _service.DeleteAsync(teamId);

            Assert.False(result.Succeeded);
            Assert.Equal(500, result.Error.Code);
            Assert.Contains("Team deleted, but failed to update ranking", result.Error.Message);
        }

        [Fact]
        public async Task DeleteAsync_Success_ReturnsSuccess()
        {
            int teamId = 8;
            var team = new Team { Id = teamId, Name = "Team Eight" };
            _teamRepoMock.Setup(r => r.GetAsync(teamId)).ReturnsAsync(team);

            var matchesList = new List<Match>
            {
                new Match { Id = 201, HomeTeamId = teamId, AwayTeamId = 2 }
            };
            var mockMatches = matchesList.AsQueryable().BuildMockDbSet();
            _matchRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Match, bool>>>()))
                .Returns(mockMatches.Object);

            _matchRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(1);

            _teamRepoMock.Setup(r => r.DeleteAsync(teamId)).ReturnsAsync(1);

            _rankingServiceMock.Setup(r => r.RecalculateRankingsAsync())
                .ReturnsAsync(ServiceResult.Success(true));

            var result = await _service.DeleteAsync(teamId);

            Assert.True(result.Succeeded);
            Assert.Equal(teamId, result.Data.Id);
        }
    }
}