using FastFoodWeb.Models;
using FastFoodWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FastFoodWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _cache;

        public ProductsController(IProductService productService, IMemoryCache cache)
        {
            _productService = productService;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var cacheKey = "all_products";
            var products = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _productService.GetAllProductsAsync();
            });

            return View(products);
        }

        public async Task<IActionResult> Category(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var cacheKey = $"products_category_{id}";
            var products = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _productService.GetProductsByCategoryAsync(id);
            });

            ViewBag.Category = id;
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cacheKeyProduct = $"product_{id}";

            var product = await _cache.GetOrCreateAsync(cacheKeyProduct, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _productService.GetProductByIdAsync(id);
            });

            if (product == null)
            {
                return NotFound();
            }

            var cacheKeyRelated = $"related_products_{product.Category}_{id}";
            var relatedProducts = await _cache.GetOrCreateAsync(cacheKeyRelated, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var related = await _productService.GetProductsByCategoryAsync(product.Category);
                return related.Where(p => p.Id != product.Id).Take(3).ToList();
            });

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }
    }
}
