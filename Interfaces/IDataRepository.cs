using MyAncestry.Models;

namespace MyAncestry.Interfaces;

public interface IDataRepository
{
    IEnumerable<string> GetDistinctLastNames();
    IEnumerable<T> PeopleQuery<T>(Func<Person, bool> predicate, Func<Person, T> select = null);
}
