﻿namespace RavenDbFinalTest.Models
{
    public class Notification
    {
        public int? Eid { get; set; }
        public RNames? name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public bool? Status { get; set; }

    }
    public class RNames
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
