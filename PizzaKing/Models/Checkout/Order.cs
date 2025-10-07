namespace PizzaKing.Models.Checkout
{
    public enum OrderStatus { Created, Confirmed, Baking, OutForDelivery, Delivered, Canceled }
    public enum PaymentStatus { Pending, Succeeded, Failed, Canceled }

    public class Order
    {
        public int Id { get; set; }
        public string? Fio { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }


        public decimal Amount { get; set; }
        public string Currency { get; set; } = "UAH";
        public OrderStatus Status { get; set; } = OrderStatus.Created;


        public string PaymentProvider { get; set; } = "LiqPay";
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? PaymentRef { get; set; }

        public IEnumerable<OrderDetails> OrderDetails { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
