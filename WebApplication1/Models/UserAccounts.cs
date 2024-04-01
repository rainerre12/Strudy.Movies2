using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class UserAccounts
    { 
        public int Id { get; set; }
        public int personid { get; set; }

        [Required(ErrorMessage = "Please enter the user name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        public string Userpassword { get; set; }
        public bool hasPrivelage { get; set; }

        [NotMapped]
        public Persons Person { get; set; }
    }
}
