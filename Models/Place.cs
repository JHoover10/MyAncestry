namespace MyAncestry.Models;

public class Place
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PlaceType PlaceType { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}
