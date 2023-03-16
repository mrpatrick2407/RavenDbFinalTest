using Microsoft.Build.Evaluation;

namespace RavenDbFinalTest.Models
{
    public class Empdata
    {
        public string EmpId { get; set; }
        
        public Name? Name { get; set; }

        public string DoB { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string MaritalStatus { get; set; }

        public string JoinDate { get; set; }
        
        public string PhoneNo { get; set; }

        public string Role { get; set; }

        public string Position { get; set; }

        public Address Address { get; set; }

        public Project Project { get; set; }
    }

    public class Name
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class Address
    { 
        public string Street { get; set; }
        
        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }

    public class Project
    {
        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDuration { get; set; }

        public string ProjectStatus { get; set; }
    }

}
