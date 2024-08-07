using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [Range(0.1, double.MaxValue ,
            ErrorMessage ="Price Must Be Grater Than 0")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue,
            ErrorMessage = "Quantuty Must Be one Item at Least")]
        public int Quantuty { get; set; }
    }
}
