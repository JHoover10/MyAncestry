using AutoMapper;
using MyAncestry.Interfaces;
using MyAncestry.Models;
using MyAncestry.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyAncestry.Services;

public class DataRepository : IDataRepository
{
    private readonly List<Person> people = new List<Person>();
    private readonly List<Place> places = new List<Place>();
    private readonly List<Event> events = new List<Event>();
    private readonly List<Family> families = new List<Family>();

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
                case "Place":
                    places.Add(mapper.Map<JToken, Place>(jObject));
                    break;
                case "Event":
                    events.Add(mapper.Map<JToken, Event>(jObject));
                    break;
                case "Family":
                    families.Add(mapper.Map<JToken, Family>(jObject));
                    break;
                default:
                    break;
            }
        }
    }

    public IEnumerable<TResult> Query<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate = null, Func<TEntity, TResult> selector = null)
    {
        var tableName = typeof(TEntity).Name;
        var table = new List<TEntity>();
        
        switch (tableName)
        {
            case "Person":
                table = people as List<TEntity>;
                break;
            case "Place":
                table = places as List<TEntity>;
                break;
            case "Event":
                table = events as List<TEntity>;
                break;
            case "Family":
                table = families as List<TEntity>;
                break;
            default:
                return null;
                break;
        }

        var tableQuery = table.AsQueryable();

        if (predicate != null)
        {
            tableQuery = tableQuery.Where(predicate);
        }

        if (selector != null) 
        {
            return tableQuery.Select(selector);
        }

        return tableQuery.AsEnumerable() as IEnumerable<TResult>;
    }
}
