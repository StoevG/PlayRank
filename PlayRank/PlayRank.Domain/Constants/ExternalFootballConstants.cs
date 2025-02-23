namespace PlayRank.Domain.Constants
{
    public static class ExternalFootballConstants
    {
        // Configuration keys
        public const string ApiKeyConfigKey = "FootballApi:ApiKey";

        // HTTP header keys
        public const string ApiKeyHeader = "x-rapidapi-key";

        // API Endpoints
        public const string CountriesEndpoint = "teams/countries";
        public const string TeamsEndpointFormat = "teams?country={0}";

        // Error Messages
        public const string ApiKeyNotConfigured = "API key not configured.";
        public const string FailedToRetrieveCountries = "Failed to retrieve countries.";
        public const string NoCountriesDataReturned = "No countries data returned.";
        public const string FailedToFetchTeams = "Failed to fetch teams.";
    }
}
