using AutoMapper;
using PlayRank.Application.Core.Teams.ViewModels;
using PlayRank.Domain.Entities;

namespace PlayRank.Application.Core.Teams.MapperProfiles
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamViewModel>();
            CreateMap<CreateTeamViewModel, Team>();
        }
    }
}
