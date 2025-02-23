using Microsoft.AspNetCore.Mvc;
using PlayRank.Application.Core.Interfaces;

namespace PlayRank.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving football team ranking statistics.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingController"/> class.
        /// </summary>
        /// <param name="rankingService">The ranking service instance.</param>
        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        /// <summary>
        /// Retrieves all team rankings.
        /// </summary>
        /// <returns>An IActionResult containing a ServiceResult with a list of ranking view models.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRankings()
        {
            var result = await _rankingService.GetAllAsync();
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves the top 3 ranked teams.
        /// </summary>
        /// <returns>An IActionResult containing a ServiceResult with the top 3 ranking view models.</returns>
        [HttpGet("top3")]
        public async Task<IActionResult> GetTop3Teams()
        {
            var result = await _rankingService.GetTopTeamsAsync(3);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves a paginated leaderboard.
        /// </summary>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The number of entries per page (default is 10).</param>
        /// <returns>An IActionResult containing a ServiceResult with a list of ranking view models.</returns>
        [HttpGet("leaderboard/paged")]
        public async Task<IActionResult> GetLeaderboardPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _rankingService.GetLeaderboardPagedAsync(page, pageSize);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves statistics for a given team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <returns>An IActionResult containing a ServiceResult with the team statistic view model.</returns>
        [HttpGet("statistics/{teamId:int}")]
        public async Task<IActionResult> GetTeamStatistics(int teamId)
        {
            var result = await _rankingService.GetTeamStatisticsAsync(teamId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves the rank position of a specific team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <returns>An IActionResult containing a ServiceResult with the team's rank position.</returns>
        [HttpGet("position/{teamId:int}")]
        public async Task<IActionResult> GetTeamRankPosition(int teamId)
        {
            var result = await _rankingService.GetTeamRankPositionAsync(teamId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves the top teams with the most wins.
        /// </summary>
        /// <param name="count">The number of top teams to retrieve.</param>
        /// <returns>An IActionResult containing a ServiceResult with a list of ranking view models.</returns>
        [HttpGet("topwins/{count:int}")]
        public async Task<IActionResult> GetTopTeamsByWins(int count)
        {
            var result = await _rankingService.GetTopTeamsByWinsAsync(count);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves the top teams with the most draws.
        /// </summary>
        /// <param name="count">The number of top teams to retrieve.</param>
        /// <returns>An IActionResult containing a ServiceResult with a list of ranking view models.</returns>
        [HttpGet("topdraws/{count:int}")]
        public async Task<IActionResult> GetTopTeamsByDraws(int count)
        {
            var result = await _rankingService.GetTopTeamsByDrawsAsync(count);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves the top teams with the least losses.
        /// </summary>
        /// <param name="count">The number of top teams to retrieve.</param>
        /// <returns>An IActionResult containing a ServiceResult with a list of ranking view models.</returns>
        [HttpGet("toplosses/{count:int}")]
        public async Task<IActionResult> GetTopTeamsByLeastLosses(int count)
        {
            var result = await _rankingService.GetTopTeamsByLeastLossesAsync(count);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Updates the ranking for a specific match.
        /// </summary>
        /// <param name="matchId">The unique identifier of the match to update rankings for.</param>
        /// <returns>An IActionResult containing a ServiceResult indicating the result of the operation.</returns>
        [HttpPost("update-match/{matchId:int}")]
        public async Task<IActionResult> UpdateRankingForMatch(int matchId)
        {
            var result = await _rankingService.UpdateRankingForMatchAsync(matchId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Recalculates the rankings for all teams.
        /// </summary>
        /// <returns>An IActionResult containing a ServiceResult indicating the result of the operation.</returns>
        [HttpPost("recalculate")]
        public async Task<IActionResult> RecalculateRankings()
        {
            var result = await _rankingService.RecalculateRankingsAsync();
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
