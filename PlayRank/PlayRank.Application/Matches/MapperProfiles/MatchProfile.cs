using AutoMapper;
using PlayRank.Application.Core.Matches.ViewModels;
using PlayRank.Domain.Entities;

namespace PlayRank.Application.Core.Matches.MapperProfiles
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            CreateMap<Match, MatchViewModel>()
                .ForMember(x => x.HomeTeam, opt => opt.MapFrom(x => x.HomeTeam.Name))
                .ForMember(x => x.AwayTeam, opt => opt.MapFrom(x => x.AwayTeam.Name));

            CreateMap<CreateMatchViewModel, Match>();
        }
    }
}