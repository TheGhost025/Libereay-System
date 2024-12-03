using System.ComponentModel.DataAnnotations;

namespace Libereay_System.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; } // 13-character ISBN

        [Required]
        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }
    }
}
