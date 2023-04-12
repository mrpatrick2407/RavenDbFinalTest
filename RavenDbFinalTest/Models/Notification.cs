namespace RavenDbFinalTest.Models
{
    public class Notification
    {
        public int? Eid { get; set; }
        public Names? name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public bool? Status { get; set; }

    }
    public class Names
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
