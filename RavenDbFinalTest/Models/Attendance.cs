using System.Text.Json.Serialization;

public class Attendance
{
    public int Eid { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<DateCreated> DateCreated { get; set; }
    public Metadata Metadata { get; set; }
}

public class DateCreated
{
    public string Date { get; set; }
    public bool IsPresent { get; set; }
    public int TestInt { get; set; }
}

public class Metadata
{
    [JsonPropertyName("@collection")]
    public string Collection { get; set; }
    [JsonPropertyName("Raven-Clr-Type")]
    public string RavenClrType { get; set; }
}