using AutoMapper;
using MyAncestry.Models;
using Newtonsoft.Json.Linq;
using static MudBlazor.Colors;

namespace MyAncestry;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<JToken, Person>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.FirstName, src => src.MapFrom(x => x["primary_name"]["first_name"]))
            .ForMember(dest => dest.LastName, src => src.MapFrom(x => x["primary_name"]["surname_list"][0]["surname"]))
            .ForMember(dest => dest.Nickname, src => src.MapFrom(x => x["primary_name"]["nick"]))
            .ForMember(dest => dest.Families, src => src.MapFrom(x => x["parent_family_list"]))
            .ForMember(dest => dest.Events, src => src.MapFrom(x => GetEventIds(x["event_ref_list"])))
            ;

        CreateMap<JToken, Event>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.EventType, src => src.MapFrom(x => ParseEnumOrDefault(x.SelectToken("type.string").Value<string>() ?? string.Empty, EventType.Unknown)))
            .ForMember(dest => dest.DateTime, src => src.MapFrom(x => ParseDateTime(x.SelectToken("date.text").Value<string>() ?? string.Empty)))
            ;

        CreateMap<JToken, Family>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.FatherId, src => src.MapFrom(x => x["father_handle"]))
            .ForMember(dest => dest.MotherId, src => src.MapFrom(x => x["mother_handle"]))
            .ForMember(dest => dest.FamilyType, src => src.MapFrom(x => Enum.Parse<FamilyType>(x["type"]["string"].Value<string>())))
            .ForMember(dest => dest.Events, src => src.MapFrom(x => GetEventIds(x["event_ref_list"])))
            ;

        CreateMap<JToken, Place>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x["name"]["value"]))
            .ForMember(dest => dest.PlaceType, src => src.MapFrom(x => Enum.Parse<PlaceType>(x["place_type"]["string"].Value<string>())))
            .ForMember(dest => dest.Latitude, src => src.MapFrom(x => x["lat"]))
            .ForMember(dest => dest.Longitude, src => src.MapFrom(x => x["long"]))
            ;
    }

    private static DateTime ParseDateTime(string dateTime)
    {
        if (dateTime != null && DateTime.TryParse(dateTime, out var result)) 
        {
            return result;
        }

        return DateTime.MinValue;
    }

    private static T ParseEnumOrDefault<T>(string value, T defaultValue) where T : struct, Enum
    {
        if (value != null && Enum.TryParse(value, out T result))
        {
            return result;
        }

        return defaultValue;
    }

    private static List<EventLink> GetEventIds(JToken token)
    {
        var eventIds = new List<EventLink>();

        if (token == null)
        {
            return new List<EventLink>();
        }

        foreach (var item in token as JArray)
        {
            eventIds.Add(new EventLink()
            {
                Id = item["ref"].Value<string>(),
                EventType = ParseEnumOrDefault(item["role"]["string"].Value<string>(), EventType.Unknown)
            });
        }

        return eventIds;
    }
}
