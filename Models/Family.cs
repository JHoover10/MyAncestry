namespace MyAncestry.Models;

public class Family
{
    public string Id { get; set; }
    public string FatherId { get; set; }
    public string MotherId { get; set; }
    public FamilyType FamilyType { get; set; }
    public List<Child> Children { get; set; }
    public List<EventLink> Events { get; set; }
}
