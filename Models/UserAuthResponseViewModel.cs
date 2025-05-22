namespace FastFoodWeb.Models
{
    public class UserAuthResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}