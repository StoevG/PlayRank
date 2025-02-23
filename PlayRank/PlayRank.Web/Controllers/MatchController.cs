using Microsoft.AspNetCore.Mvc;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Matches.ViewModels;

namespace PlayRank.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing football matches.
    /// </summary
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchController"/> class.
        /// </summary>
        /// <param name="matchService">The match service instance.</param>
        /// <param name="teamService">The team service instance.</param>
        public MatchController(IMatchService matchService, ITeamService teamService)
        {
            this._matchService = matchService;
            this._teamService = teamService;
        }

        /// <summary>
        /// Retrieves all matches.
        /// </summary>
        /// <returns>An IActionResult containing a ServiceResult with a list of match view models.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetMatches()
        {
            var result = await this._matchService.GetAllAsync();

            return result.Succeeded ? this.Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves a match by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the match.</param>
        /// <returns>An IActionResult containing a ServiceResult with the match view model.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatch(int id)
        {
            var result = await this._matchService.GetAsync(id);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // <summary>
        /// Creates a new match.
        /// </summary>
        /// <param name="model">The match details for creation.</param>
        /// <returns>An IActionResult containing a ServiceResult with the created match view model.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateMatchViewModel model)
        {
            var result = await _matchService.CreateAsync(model);

            return result.Succeeded ? this.Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Updates an existing match and recalculates rankings.
        /// </summary>
        /// <param name="id">The unique identifier of the match to update.</param>
        /// <param name="model">The updated match details.</param>
        /// <returns>An IActionResult containing a ServiceResult with the updated match view model.</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMatch(int id, [FromBody] CreateMatchViewModel model)
        {
            var result = await _matchService.UpdateAsync(id, model);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Predicts the match score based on team rankings.
        /// </summary>
        /// <param name="matchId">The unique identifier of the match to predict.</param>
        /// <returns>An IActionResult containing a ServiceResult with a match prediction view model.</returns>
        [HttpGet("predict/{matchId}")]
        public async Task<IActionResult> PredictMatchScore(int matchId)
        {
            var result = await _matchService.PredictScoreAsync(matchId);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deletes a match by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the match to delete.</param>
        /// <returns>An IActionResult containing a ServiceResult with the deleted match view model.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var result = await _matchService.DeleteAsync(id);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
