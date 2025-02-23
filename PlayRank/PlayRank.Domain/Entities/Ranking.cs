using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayRank.Domain.Entities.Abstract;

namespace PlayRank.Domain.Entities
{
    public class Ranking : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; }

        public int Points { get; set; }
        public int PlayedGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
    }
}