using System.ComponentModel.DataAnnotations;

namespace PlayRank.Application.Core.Teams.ViewModels
{
    public class CreateTeamViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}