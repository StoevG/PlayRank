using Microsoft.AspNetCore.Mvc;
using PlayRank.Application.Core.Interfaces;
using PlayRank.Application.Core.Teams.ViewModels;

namespace PlayRank.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing football teams.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class.
        /// </summary>
        /// <param name="teamService">The team service instance.</param>
        public TeamController(ITeamService teamService)
        {
            this._teamService = teamService;
        }

        /// <summary>
        /// Retrieves all teams.
        /// </summary>
        /// <returns>An IActionResult containing a ServiceResult with a list of team view models.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetTeams()
        {
            var result = await this._teamService.GetAllAsync();

            return result.Succeeded ? this.Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves a team by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the team.</param>
        /// <returns>An IActionResult containing a ServiceResult with the team view model.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var result = await this._teamService.GetAsync(id);

            return this.Ok(result);
        }

        /// <summary>
        /// Creates a new team.
        /// </summary>
        /// <param name="model">The team details for creation.</param>
        /// <returns>An IActionResult containing a ServiceResult with the created team view model.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _teamService.CreateAsync(model);

            return result.Succeeded ? this.Ok(result) : BadRequest(result);
        }

        //// <summary>
        /// Updates an existing team.
        /// </summary>
        /// <param name="id">The unique identifier of the team to update.</param>
        /// <param name="model">The updated team details.</param>
        /// <returns>An IActionResult containing a ServiceResult with the updated team view model.</returns>
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _teamService.UpdateAsync(id, model);

            return result.Succeeded ? this.Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deletes a team by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the team to delete.</param>
        /// <returns>An IActionResult containing a ServiceResult with the deleted team view model.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteAsync(id);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
