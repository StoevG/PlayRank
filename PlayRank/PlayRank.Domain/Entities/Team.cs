using System.ComponentModel.DataAnnotations;
using PlayRank.Domain.Entities.Abstract;

namespace PlayRank.Domain.Entities
{
    public class Team : Entity
    {
        public Team()
        {
            HomeMatches = new HashSet<Match>();
            AwayMatches = new HashSet<Match>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public Ranking? Ranking { get; set; }

        public ICollection<Match>? HomeMatches { get; set; }
        public ICollection<Match>? AwayMatches { get; set; }
    }
}
