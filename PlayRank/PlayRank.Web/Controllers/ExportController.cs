using Microsoft.AspNetCore.Mvc;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Matches.ViewModels;
using PlayRank.Application.Core.Rankings.ViewModels;
using PlayRank.Application.Core.Teams.ViewModels;
using PlayRank.Application.Core.ExternalDtos;
using System.Text;
using PlayRank.Application.Core.Interfaces.External;
using PlayRank.Domain.Constants;

namespace PlayRank.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for exporting various data sets to CSV format.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMatchService _matchService;
        private readonly IRankingService _rankingService;
        private readonly IFootballTeamService _footballTeamService;
        private readonly IExportService _exportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportController"/> class.
        /// </summary>
        /// <param name="teamService">Team service instance.</param>
        /// <param name="matchService">Match service instance.</param>
        /// <param name="rankingService">Ranking service instance.</param>
        /// <param name="footballTeamService">Football team service instance.</param>
        /// <param name="exportService">Export service instance.</param>
        public ExportController(
            ITeamService teamService,
            IMatchService matchService,
            IRankingService rankingService,
            IFootballTeamService footballTeamService,
            IExportService exportService)
        {
            _teamService = teamService;
            _matchService = matchService;
            _rankingService = rankingService;
            _footballTeamService = footballTeamService;
            _exportService = exportService;
        }

        /// <summary>
        /// Exports all teams data to CSV.
        /// </summary>
        [HttpGet("teams")]
        public async Task<IActionResult> ExportTeamsToCsv()
        {
            var result = await _teamService.GetAllAsync();
            if (!result.Succeeded)
                return BadRequest(result);

            var teams = result.Data;
            var csv = _exportService.BuildCsv(teams, new Dictionary<string, Func<TeamViewModel, string>>
            {
                { ExportConstants.TeamsHeaderId, t => t.Id.ToString() },
                { ExportConstants.TeamsHeaderName, t => t.Name }
            });
            return File(Encoding.UTF8.GetBytes(csv), ExportConstants.CsvContentType, ExportConstants.TeamsFileName);
        }

        /// <summary>
        /// Exports all matches data to CSV.
        /// </summary>
        [HttpGet("matches")]
        public async Task<IActionResult> ExportMatchesToCsv()
        {
            var result = await _matchService.GetAllAsync();
            if (!result.Succeeded)
                return BadRequest(result);

            var matches = result.Data;
            var csv = _exportService.BuildCsv(matches, new Dictionary<string, Func<MatchViewModel, string>>
            {
                { ExportConstants.MatchesHeaderId, m => m.Id.ToString() },
                { ExportConstants.MatchesHeaderHomeTeam, m => m.HomeTeam },
                { ExportConstants.MatchesHeaderAwayTeam, m => m.AwayTeam },
                { ExportConstants.MatchesHeaderHomeTeamScore, m => m.HomeTeamScore.ToString() },
                { ExportConstants.MatchesHeaderAwayTeamScore, m => m.AwayTeamScore.ToString() },
                { ExportConstants.MatchesHeaderIsOver, m => m.IsOver.ToString() }
            });
            return File(Encoding.UTF8.GetBytes(csv), ExportConstants.CsvContentType, ExportConstants.MatchesFileName);
        }

        /// <summary>
        /// Exports all rankings data to CSV.
        /// </summary>
        [HttpGet("rankings")]
        public async Task<IActionResult> ExportRankingsToCsv()
        {
            var result = await _rankingService.GetAllAsync();
            if (!result.Succeeded)
                return BadRequest(result);

            var rankings = result.Data;
            var csv = _exportService.BuildCsv(rankings, new Dictionary<string, Func<RankingViewModel, string>>
            {
                { ExportConstants.RankingsHeaderTeamId, r => r.TeamId.ToString() },
                { ExportConstants.RankingsHeaderTeamName, r => r.TeamName },
                { ExportConstants.RankingsHeaderPoints, r => r.Points.ToString() },
                { ExportConstants.RankingsHeaderPlayedGames, r => r.PlayedGames.ToString() },
                { ExportConstants.RankingsHeaderWins, r => r.Wins.ToString() },
                { ExportConstants.RankingsHeaderDraws, r => r.Draws.ToString() },
                { ExportConstants.RankingsHeaderLosses, r => r.Losses.ToString() },
                { ExportConstants.RankingsHeaderRankPosition, r => r.RankPosition.ToString() }
            });
            return File(Encoding.UTF8.GetBytes(csv), ExportConstants.CsvContentType, ExportConstants.RankingsFileName);
        }

        /// <summary>
        /// Exports all external football teams data to CSV.
        /// </summary>
        [HttpGet("externalteams")]
        public async Task<IActionResult> ExportExternalTeamsToCsv()
        {
            var result = await _footballTeamService.GetAllFootballTeamsAsync();
            if (!result.Succeeded)
                return BadRequest(result);

            var teams = result.Data;
            var csv = _exportService.BuildCsv(teams, new Dictionary<string, Func<FootballTeamDto, string>>
            {
                { ExportConstants.ExternalTeamsHeaderId, t => t.Id.ToString() },
                { ExportConstants.ExternalTeamsHeaderName, t => t.Name },
                { ExportConstants.ExternalTeamsHeaderCode, t => t.Code },
                { ExportConstants.ExternalTeamsHeaderCountry, t => t.Country },
                { ExportConstants.ExternalTeamsHeaderFounded, t => t.Founded?.ToString() ?? "" },
                { ExportConstants.ExternalTeamsHeaderNational, t => t.National.ToString() },
                { ExportConstants.ExternalTeamsHeaderLogo, t => t.Logo }
            });
            return File(Encoding.UTF8.GetBytes(csv), ExportConstants.CsvContentType, ExportConstants.ExternalTeamsFileName);
        }
    }
}
