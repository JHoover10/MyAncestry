using AutoMapper;
using MyAncestry.Models;
using Newtonsoft.Json.Linq;

namespace MyAncestry;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<JToken, Person>()
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
                .ForMember(dest => dest.FirstName, src => src.MapFrom(x => x["primary_name"]["first_name"]))
                .ForMember(dest => dest.LastName, src => src.MapFrom(x => x["primary_name"]["surname_list"][0]["surname"]))
                ;
    }
}
