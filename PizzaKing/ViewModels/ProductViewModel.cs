using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaKing.Models;
using System.ComponentModel.DataAnnotations;

namespace PizzaKing.ViewModels
{
    public class ProductViewModel
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Не указано название")]
        public string? Name { get; set; }
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Не указано описание")]
        public string? Description { get; set; }
        [Display(Name = "Изображение")]
        public IFormFile? File { get; set; }
        public string? Image { get; set; }

        [Display(Name = "Вес")]
        [Required(ErrorMessage = "Не указан вес")]
        public float Weight { get; set; }

        [Display(Name = "Калории")]
        [Required(ErrorMessage = "Не указаны калории")]
        public float Calories { get; set; }

        [Display(Name = "Стоимость")]
        [Required(ErrorMessage = "Не указана стоимость")]
        [Range(typeof(decimal), "5.0", "100000.0", ErrorMessage = "Наименьшая цена - 5 гривен")]
        public decimal Price { get; set; }

        [Display(Name = "Производитель")]
        public string? Brand { get; set; }

        [Display(Name = "Тип")]
        public ProductType Type { get; set; }

        public DateTime DateOfPublication { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Категория")]
        public SelectList? AllCategories { get; set; } = null!;

    }

}
