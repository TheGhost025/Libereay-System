using System.ComponentModel.DataAnnotations;

namespace Libereay_System.Models
{
    public class FineConfiguration
    {
        public int Id { get; set; }

        [Required]
        public decimal FinePerDay { get; set; }
    }
}
