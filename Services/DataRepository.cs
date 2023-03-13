using AutoMapper;
using MyAncestry.Interfaces;
using MyAncestry.Models;
using MyAncestry.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;

namespace MyAncestry.Services;

public class DataRepository : IDataRepository
{
    private readonly List<Person> people = new List<Person>();

    public DataRepository(IMapper mapper)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MyAncestry.Data.AncestryData.json");
        using var stringReader = new StreamReader(stream);

        while (!stringReader.EndOfStream)
        {
            var jObject = JObject.Parse(stringReader.ReadLine());

            switch (jObject["_class"].Value<string>())
            {
                case "Person":
                    people.Add(mapper.Map<JToken, Person>(jObject));
                    break;
                default:
                    break;
            }
        }
    }

    public IEnumerable<string> GetDistinctLastNames()
    {
        return people.Select(p => p.LastName).Distinct();
    }

    public IEnumerable<T> PeopleQuery<T>(Func<Person, bool> predicate, Func<Person, T> selector = null)
    {
        if (selector == null) 
        {
            return people.Where(predicate) as IEnumerable<T>;
        }

        return people.Where(predicate).Select(selector);
    }
}
