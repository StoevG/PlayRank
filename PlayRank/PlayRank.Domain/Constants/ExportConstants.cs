using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayRank.Domain.Constants
{
    public static class ExportConstants
    {
        // File names.
        public const string TeamsFileName = "Teams.csv";
        public const string MatchesFileName = "Matches.csv";
        public const string RankingsFileName = "Rankings.csv";
        public const string ExternalTeamsFileName = "ExternalTeams.csv";

        // MIME type.
        public const string CsvContentType = "text/csv";

        // Teams CSV Headers.
        public const string TeamsHeaderId = "Id";
        public const string TeamsHeaderName = "Name";

        // Matches CSV Headers.
        public const string MatchesHeaderId = "Id";
        public const string MatchesHeaderHomeTeam = "HomeTeam";
        public const string MatchesHeaderAwayTeam = "AwayTeam";
        public const string MatchesHeaderHomeTeamScore = "HomeTeamScore";
        public const string MatchesHeaderAwayTeamScore = "AwayTeamScore";
        public const string MatchesHeaderIsOver = "IsOver";

        // Rankings CSV Headers.
        public const string RankingsHeaderTeamId = "TeamId";
        public const string RankingsHeaderTeamName = "TeamName";
        public const string RankingsHeaderPoints = "Points";
        public const string RankingsHeaderPlayedGames = "PlayedGames";
        public const string RankingsHeaderWins = "Wins";
        public const string RankingsHeaderDraws = "Draws";
        public const string RankingsHeaderLosses = "Losses";
        public const string RankingsHeaderRankPosition = "RankPosition";

        // External Teams CSV Headers.
        public const string ExternalTeamsHeaderId = "Id";
        public const string ExternalTeamsHeaderName = "Name";
        public const string ExternalTeamsHeaderCode = "Code";
        public const string ExternalTeamsHeaderCountry = "Country";
        public const string ExternalTeamsHeaderFounded = "Founded";
        public const string ExternalTeamsHeaderNational = "National";
        public const string ExternalTeamsHeaderLogo = "Logo";
    }
}
