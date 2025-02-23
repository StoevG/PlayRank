using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlayRank.Application.Core.ExternalDtos;
using PlayRank.Application.Core.Interfaces.External;
using PlayRank.Domain.Commont;
using PlayRank.Domain.Constants;

namespace PlayRank.Application.Core.Services.External
{
    public class FootballTeamService : IFootballTeamService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FootballTeamService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ServiceResult<List<FootballTeamDto>>> GetAllFootballTeamsAsync()
        {
            var apiKey = _configuration[ExternalFootballConstants.ApiKeyConfigKey];
            if (string.IsNullOrEmpty(apiKey))
            {
                return ServiceResult.Failed<List<FootballTeamDto>>(new ServiceError(ExternalFootballConstants.ApiKeyNotConfigured, 500));
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(ExternalFootballConstants.ApiKeyHeader, apiKey);

            var countriesResponse = await _httpClient.GetAsync(ExternalFootballConstants.CountriesEndpoint);
            if (!countriesResponse.IsSuccessStatusCode)
            {
                return ServiceResult.Failed<List<FootballTeamDto>>(new ServiceError(ExternalFootballConstants.FailedToRetrieveCountries, (int)countriesResponse.StatusCode));
            }

            var countriesContent = await countriesResponse.Content.ReadAsStringAsync();
            var countriesApiResponse = JsonSerializer.Deserialize<FootballCountriesApiResponse>(countriesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (countriesApiResponse?.Response == null || countriesApiResponse.Response.Count == 0)
            {
                return ServiceResult.Failed<List<FootballTeamDto>>(new ServiceError(ExternalFootballConstants.NoCountriesDataReturned, 500));
            }

            var allTeams = new List<FootballTeamDto>();

            foreach (var country in countriesApiResponse.Response)
            {
                var countryParam = Uri.EscapeDataString(country.Name);
                var teamsEndpoint = string.Format(ExternalFootballConstants.TeamsEndpointFormat, countryParam);
                var teamsResponse = await _httpClient.GetAsync(teamsEndpoint);
                if (!teamsResponse.IsSuccessStatusCode)
                {
                    continue;
                }

                var teamsContent = await teamsResponse.Content.ReadAsStringAsync();
                var teamsApiResponse = JsonSerializer.Deserialize<FootballTeamsApiResponse>(teamsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (teamsApiResponse?.Response != null)
                {
                    foreach (var teamWrapper in teamsApiResponse.Response)
                    {
                        if (teamWrapper?.Team != null)
                        {
                            allTeams.Add(teamWrapper.Team);
                        }
                    }
                }
            }

            var distinctTeams = allTeams.GroupBy(t => t.Id).Select(g => g.First()).ToList();
            return ServiceResult.Success(distinctTeams);
        }

        public async Task<ServiceResult<List<FootballTeamDto>>> SearchFootballTeamsAsync(string query)
        {
            var allTeamsResult = await GetAllFootballTeamsAsync();
            if (!allTeamsResult.Succeeded)
            {
                return ServiceResult.Failed<List<FootballTeamDto>>(new ServiceError(ExternalFootballConstants.FailedToFetchTeams, 500));
            }

            var filteredTeams = allTeamsResult.Data
                .Where(t =>
                    (!string.IsNullOrEmpty(t.Name) && t.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (!string.IsNullOrEmpty(t.Country) && t.Country.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            return ServiceResult.Success(filteredTeams);
        }
    }
}
