using System.ComponentModel.DataAnnotations;

namespace PizzaKing.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Author { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
