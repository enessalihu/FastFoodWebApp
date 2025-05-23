namespace FastFoodWeb.Models
{
    public class ProductDetailsViewModel
    {
        public ProductViewModel Product { get; set; }
        public List<ProductViewModel> RelatedProducts { get; set; } = new List<ProductViewModel>();
    }
}