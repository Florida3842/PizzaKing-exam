using System.ComponentModel.DataAnnotations;

namespace PizzaKing.ViewModels
{
    public class ReviewCreateViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [StringLength(100)]
        public string Author { get; set; }

        [Required(ErrorMessage = "Введите текст отзыва")]
        [StringLength(2000, MinimumLength = 10)]
        public string Text { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;
    }
}
