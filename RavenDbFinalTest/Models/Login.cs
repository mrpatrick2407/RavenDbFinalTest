namespace RavenDbFinalTest.Models
{
    public class Login
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; }

        public Login()
        {
            Id = "Log-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        }
    }
}
