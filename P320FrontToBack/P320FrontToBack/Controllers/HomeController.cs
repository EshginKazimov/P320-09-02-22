using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P320FrontToBack.DataAccessLayer;
using P320FrontToBack.Models;
using P320FrontToBack.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P320FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //HttpContext.Session.SetString("session", "P320");

            //Response.Cookies.Append("cookie", "Hello");
            //Response.Cookies.Append("cookie", "Hello", new CookieOptions { Expires = System.DateTimeOffset.Now.AddHours(1)});

            var sliderImages = _dbContext.SliderImages.ToList();
            //var slider = _dbContext.Sliders.FirstOrDefault();
            var slider = _dbContext.Sliders.SingleOrDefault();
            var categories = _dbContext.Categories.ToList();
            //var products = _dbContext.Products.Include(x => x.Category).ToList();

            //IEnumerable vs IQueryable

            //var test = _dbContext.Products.Where(x => x.Id > 2).ToList();
            //var test2 = _dbContext.Products.ToList().Where(x => x.Id > 2);

            //Count() vs Count
            //var test = _dbContext.Products.ToList().Count();
            //var test = _dbContext.Products.ToList().Count;
            //var test = _dbContext.Products.Count();

            //Select
            //var test = _dbContext.Products.Select(x => new
            //{
            //    x.Id,
            //    x.Name
            //}).ToList();

            return View(new HomeViewModel
            {
                SliderImages = sliderImages,
                Slider = slider,
                Categories = categories,
                //Products = products
            });
        }

        //public async Task<IActionResult> Index()
        //{
        //    var sliderImages = _dbContext.SliderImages.ToListAsync();
        //    var slider = _dbContext.Sliders.SingleOrDefaultAsync();
        //    var categories = _dbContext.Categories.ToListAsync();
        //    var products = _dbContext.Products.Include(x => x.Category).ToListAsync();

        //    return View(new HomeViewModel
        //    {
        //        SliderImages = await sliderImages,
        //        Slider = await slider,
        //        Categories = await categories,
        //        Products = await products
        //    });
        //}

        public async Task<IActionResult> Search(string searchedProduct)
        {
            if (string.IsNullOrEmpty(searchedProduct))
            {
                return NoContent();
            }

            var products = await _dbContext.Products
                .Where(x => x.Name.ToLower().Contains(searchedProduct.ToLower()))
                .ToListAsync();

            //return Json(products);

            return PartialView("_SeachedProductPartial", products);
        }

        public async Task<IActionResult> Basket()
        {
            //var session = HttpContext.Session.GetString("session");
            //var cookie = Request.Cookies["cookie"];

            //return Content(session + " - " + cookie);

            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
            {
                return Content("Empty");
            }

            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            var newBasket = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                var product = await _dbContext.Products.FindAsync(basketViewModel.Id);
                if (product == null)
                    continue;

                newBasket.Add(new BasketViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Count = basketViewModel.Count
                });
            }

            basket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", basket);

            return Json(newBasket);
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            List<BasketViewModel> basketViewModels;
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
            {
                basketViewModels = new List<BasketViewModel>();
            }
            else
            {
                basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);
            }

            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            if (existBasketViewModel == null)
            {
                existBasketViewModel = new BasketViewModel
                {
                    Id = product.Id
                };
                basketViewModels.Add(existBasketViewModel);
            } 
            else
            {
                existBasketViewModel.Count++;
            }

            var basket = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", basket);

            return RedirectToAction(nameof(Index));
        }
    }
}
