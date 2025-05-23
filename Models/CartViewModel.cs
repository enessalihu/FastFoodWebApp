namespace FastFoodWeb.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class CartSummaryViewModel
    {
        public List<CartViewModel> CartItems { get; set; } = new List<CartViewModel>();
        public decimal SubTotal => CartItems.Sum(item => item.ProductPrice * item.Quantity);
        public decimal Tax => SubTotal * 0.1m; 
        public decimal Total => SubTotal + Tax;
        public int ItemCount => CartItems.Sum(item => item.Quantity);
    }

    public class AddToCartViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}