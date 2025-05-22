using FastFoodWeb.Models;
using FastFoodWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FastFoodWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ITeamService _teamService;
        private readonly IRestaurantService _restaurantService;

        public HomeController(
            IProductService productService,
            ITeamService teamService,
            IRestaurantService restaurantService)
        {
            _productService = productService;
            _teamService = teamService;
            _restaurantService = restaurantService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _productService.GetCategoriesAsync();
            var productCategories = new List<ProductCategoryViewModel>();

            foreach (var category in categories)
            {
                var products = await _productService.GetProductsByCategoryAsync(category);
                productCategories.Add(new ProductCategoryViewModel
                {
                    Category = category,
                    Products = products
                });
            }

            return View(productCategories);
        }

        public async Task<IActionResult> About()
        {
            var viewModel = new AboutViewModel
            {
              
                TeamMembers = await _teamService.GetRandomTeamMembersAsync(3)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Contact()
        {
            var viewModel = new ContactViewModel
            {
                Restaurants = await _restaurantService.GetAllRestaurantsAsync()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}