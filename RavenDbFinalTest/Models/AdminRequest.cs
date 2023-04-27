using System.ComponentModel.DataAnnotations;

namespace RavenDbFinalTest.Models
{
    public class AdminRequest
    {
        public string? Id { get; set; }
        public int? eid { get; set; }
        [Display(Name = "First Name ")]

        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string? LastName { get; set; }

        public string? Email { get; set; }
        [Display(Name = "Phone Number")]
        public string? Phone_Number { get; set; }

        public string? Address { get; set; }
        [Display(Name = "Job Title")]

        public string? Job_Title { get; set; }

        public string? Department { get; set; }

        public string? Manager { get; set; }

        public string? ImageBase64 { get; set; }


    }
}
