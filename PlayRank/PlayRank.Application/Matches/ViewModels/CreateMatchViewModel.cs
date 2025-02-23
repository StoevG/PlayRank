namespace PlayRank.Application.Core.Matches.ViewModels
{
    public class CreateMatchViewModel
    {
        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }

        public int? HomeTeamScore { get; set; }

        public int? AwayTeamScore { get; set; }

        public bool IsOver { get; set; }
    }
}
