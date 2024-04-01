

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Persons
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the Last name")]
        public string LastName { get; set; }
        public bool IsActive { get; set; }
    }
}
