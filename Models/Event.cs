namespace MyAncestry.Models;

public class Event
{
    public string Id { get; set; }
    public string PlaceId { get; set; }
    public EventType EventType { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}
