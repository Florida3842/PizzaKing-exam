namespace PizzaKing.Models
{
    public class CustomCartSelection
    {
        public int? CrustId { get; set; }
        public int? SauceId { get; set; }
        public List<int> ToppingIds { get; set; } = new();
    }
}
