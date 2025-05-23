namespace FastFoodWeb.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductCategoryViewModel
    {
        public string Category { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}