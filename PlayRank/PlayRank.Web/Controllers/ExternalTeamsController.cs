using System.Text;
using Microsoft.AspNetCore.Mvc;
using PlayRank.Application.Core.ExternalDtos;
using PlayRank.Application.Core.Interfaces.External;

namespace PlayRank.Api.Controllers
{
    /// <summary>
    /// Controller for fetching external football team data and exporting it in CSV format.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalTeamsController : ControllerBase
    {
        private readonly IFootballTeamService _footballTeamService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalTeamsController"/> class.
        /// </summary>
        /// <param name="footballTeamService">The football team service instance.</param>
        public ExternalTeamsController(IFootballTeamService footballTeamService)
        {
            _footballTeamService = footballTeamService;
        }

        /// <summary>
        /// Retrieves all external football teams from the API.
        /// </summary>
        /// <returns>
        /// An IActionResult containing a ServiceResult with a list of <see cref="FootballTeamDto"/> objects.
        /// </returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExternalTeams()
        {
            var result = await _footballTeamService.GetAllFootballTeamsAsync();
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Searches for external football teams by name or country.
        /// </summary>
        /// <param name="query">The search query.</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchExternalTeams([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { error = "Query parameter is required." });
            }

            var result = await _footballTeamService.SearchFootballTeamsAsync(query);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
