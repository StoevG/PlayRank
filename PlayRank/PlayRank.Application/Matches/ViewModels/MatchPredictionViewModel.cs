namespace PlayRank.Application.Core.Matches.ViewModels
{
    public class MatchPredictionViewModel
    {
        public int MatchId { get; set; }

        public int PredictedHomeScore { get; set; }

        public int PredictedAwayScore { get; set; }

        public string Prediction { get; set; }
    }
}
