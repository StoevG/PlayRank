namespace PlayRank.Application.Core.Rankings.ViewModels
{
    public class TeamStatisticViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int PlayedGames { get; set; }
        public int Points { get; set; }
        public int RankPosition { get; set; }
    }
}