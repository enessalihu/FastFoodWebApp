namespace FastFoodWeb.Models
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}