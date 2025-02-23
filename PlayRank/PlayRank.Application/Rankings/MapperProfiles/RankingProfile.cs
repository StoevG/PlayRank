using AutoMapper;
using PlayRank.Application.Core.Rankings.ViewModels;
using PlayRank.Domain.Entities;

namespace PlayRank.Application.Core.Rankings.MapperProfiles
{
    public class RankingProfile : Profile
    {
        public RankingProfile()
        {
            CreateMap<Ranking, RankingViewModel>()
                .ForMember(x => x.TeamName, opt => opt.MapFrom(x => x.Team.Name));

            CreateMap<Ranking, TeamStatisticViewModel>()
                .ForMember(x => x.TeamName, opt => opt.MapFrom(x => x.Team.Name));
        }
    }
}
