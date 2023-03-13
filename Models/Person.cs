namespace MyAncestry.Models;

public class Person
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }
    public List<string> Families { get; set; }
    public List<EventLink> Events { get; set; }
}
