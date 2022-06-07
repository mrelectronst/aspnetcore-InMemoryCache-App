using InMemoryCache.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCache.App.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("dateNow")))
            //    _memoryCache.Set<string>("dateNow", DateTime.Now.ToString());

            if (!_memoryCache.TryGetValue("dateNow", out string dateCache))
            {
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions();

                cacheEntryOptions.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(5);

                //cacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(10);

                cacheEntryOptions.Priority = CacheItemPriority.Normal;

                cacheEntryOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _memoryCache.Set("callback", $"{key}->{value} => reason:{reason}");
                });

                _memoryCache.Set<string>("dateNow", DateTime.Now.ToString(), cacheEntryOptions);
            }

            Product product = new Product { Id = 1, Name = "Pencil", Price = 10 };

            _memoryCache.Set<Product>("product1", product);

            return View();
        }

        public IActionResult ShowDate()
        {
            //ViewBag.Date = _memoryCache.Get<string>("dateNow");

            //ViewBag.Date = _memoryCache.GetOrCreate<string>("dateOneYearAgo", entry =>
            //{
            //    return DateTime.UtcNow.AddYears(-1).ToString();
            //});

            _memoryCache.TryGetValue("dateNow", out string dateCache);
            _memoryCache.TryGetValue("callback", out string callback);

            ViewBag.Date = dateCache;
            ViewBag.CallBack = callback;
            ViewBag.Product = _memoryCache.Get<Product>("product1");

            //_memoryCache.Remove("dateNow");

            return View();
        }
    }
}
