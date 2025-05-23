namespace FastFoodWeb.Models
{
    public class ContactViewModel
    {
        public List<RestaurantViewModel> Restaurants { get; set; } = new List<RestaurantViewModel>();
        public RestaurantViewModel PrimaryRestaurant => Restaurants.FirstOrDefault();
    }
}