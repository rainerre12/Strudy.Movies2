using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Movies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public bool IsAvailanble { get; set; }
    }
}
