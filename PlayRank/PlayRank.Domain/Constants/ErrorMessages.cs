namespace PlayRank.Domain.Constants
{
    public static class ErrorMessages
    {
        // MatchService errors.
        public const string MatchScoreRequired = "Home Team Score and Away Team Score are required when the match is over.";
        public const string NegativeScoreNotAllowed = "Home Team Score and Away Team Score cannot be negative.";
        public const string HomeTeamNotFound = "Home Team does not exist.";
        public const string AwayTeamNotFound = "Away Team does not exist.";
        public const string MatchNotFound = "Match not found.";
        public const string UnableToDeleteMatch = "Unable to delete match.";
        public const string FailedToUpdateRankingAfterMatchCreated = "Match created, but failed to update ranking.";
        public const string FailedToUpdateRankingAfterMatchUpdated = "Match updated, but failed to update ranking.";
        public const string FailedToUpdateRankingAfterMatchDeleted = "Match deleted, but failed to update ranking.";
        public const string MatchAlreadyOver = "Match is already over.";
        public const string MatchNotOver = "Match is not over.";
        public const string UnableToRetrieveTeamRankings = "Unable to retrieve team rankings.";
        public const string FailedToUptateRanking = "Match processed, but failed to update ranking.";

        // TeamService errors.
        public const string TeamAlreadyExists = "A team with the specified name already exists.";
        public const string TeamNotFound = "Team not found.";
        public const string TeamDoesNotExist = "Team does not exist.";
        public const string UnableToDeleteTeam = "Unable to delete team.";
        public const string FailedToUpdateRankingAfterTeamDeleted = "Team deleted, but failed to update ranking.";
        public const string TeamCreatedButFailedRanking = "Team created, but failed to update ranking.";
        public const string TeamDeletedButFailedRanking = "Team deleted, but failed to update ranking.";

        // RankingService errors.
        public const string TeamRankingNotFound = "Team ranking not found.";
        public const string TeamNotFoundInRanking = "Team not found.";
    }
}
