namespace FastFoodWeb.Models
{
    public class RestaurantViewModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string OpeningHours { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}