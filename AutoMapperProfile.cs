using AutoMapper;
using MyAncestry.Models;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using static MudBlazor.Colors;

namespace MyAncestry;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<JToken, Person>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.FirstName, src => src.MapFrom(x => GetFirstName(x["primary_name"]["first_name"])))
            .ForMember(dest => dest.MiddleName, src => src.MapFrom(x => GetMiddleName(x["primary_name"]["first_name"])))
            .ForMember(dest => dest.LastName, src => src.MapFrom(x => x["primary_name"]["surname_list"][0]["surname"]))
            .ForMember(dest => dest.Nickname, src => src.MapFrom(x => x["primary_name"]["nick"]))
            .ForMember(dest => dest.FamilyIds, src => src.MapFrom(x => x["family_list"]))
            .ForMember(dest => dest.ParentIds, src => src.MapFrom(x => x["parent_family_list"]))
            .ForMember(dest => dest.EventLinks, src => src.MapFrom(x => GetEventIds(x["event_ref_list"])))
            ;

        CreateMap<JToken, Event>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.PlaceId, src => src.MapFrom(x => Parse(x["place"])))
            .ForMember(dest => dest.Description, src => src.MapFrom(x => x["description"]))
            .ForMember(dest => dest.EventType, src => src.MapFrom(x => ParseEnumOrDefault(x.SelectToken("type.string").Value<string>(), EventType.Unknown)))
            .ForMember(dest => dest.DateTime, src => src.MapFrom(x => ParseDateTime(x.SelectToken("date.dateval") as JArray)))
            ;

        CreateMap<JToken, Family>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.FatherId, src => src.MapFrom(x => x["father_handle"]))
            .ForMember(dest => dest.MotherId, src => src.MapFrom(x => x["mother_handle"]))
            .ForMember(dest => dest.FamilyType, src => src.MapFrom(x => Enum.Parse<FamilyType>(x["type"]["string"].Value<string>())))
            .ForMember(dest => dest.Children, src => src.MapFrom(x => x["child_ref_list"]))
            .ForMember(dest => dest.Events, src => src.MapFrom(x => GetEventIds(x["event_ref_list"])))
            ;

        CreateMap<JToken, Place>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["handle"]))
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x["name"]["value"]))
            .ForMember(dest => dest.PlaceType, src => src.MapFrom(x => Enum.Parse<PlaceType>(x["place_type"]["string"].Value<string>())))
            .ForMember(dest => dest.Latitude, src => src.MapFrom(x => x["lat"]))
            .ForMember(dest => dest.Longitude, src => src.MapFrom(x => x["long"]))
            ;

        CreateMap<JToken, Child>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x["ref"]))
            .ForMember(dest => dest.FatherRelation, src => src.MapFrom(x => ParseEnumOrDefault(x.SelectToken("frel.string").Value<string>(), Relation.Unknown)))
            .ForMember(dest => dest.MotherRelation, src => src.MapFrom(x => ParseEnumOrDefault(x.SelectToken("mrel.string").Value<string>(), Relation.Unknown)))
            ;
    }

    private string GetFirstName(JToken token)
    {
        return token.ToString().Split(' ')[0];
    }

    private string GetMiddleName(JToken token)
    {
        var names = token.ToString().Split(' ');

        if (names.Length == 1)
        {
            return string.Empty;
        }

        return string.Join(' ', names[1..]);
    }

    private string Parse(JToken token)
    {
        return string.IsNullOrWhiteSpace(token.ToString()) ? null : token.ToString();
    }

    private static DateTime ParseDateTime(JArray token)
    {
        try
        {
            var day = token[0].ToString();
            var month = token[1].ToString();
            var year = token[2].ToString();

            if (DateTime.TryParse($"{month}/{day}/{year}", out var result))
            {
                return result;
            }
            else if (Regex.IsMatch(year, @"\d\d\d\d"))
            {
                return DateTime.Parse($"1/1/{year}");
            }
        }
        catch (Exception)
        {
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
