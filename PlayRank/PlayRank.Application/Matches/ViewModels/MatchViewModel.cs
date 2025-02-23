namespace PlayRank.Application.Core.Matches.ViewModels
{
    public class MatchViewModel
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public int HomeTeamScore { get; set; }

        public int AwayTeamScore { get; set; }

        public bool IsOver { get; set; }
    }
}
