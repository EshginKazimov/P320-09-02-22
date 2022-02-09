using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P320FrontToBack.DataAccessLayer;
using System.Linq;
using System.Threading.Tasks;

namespace P320FrontToBack.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly int _productsCount;

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _productsCount = _dbContext.Products.Count();
        }

        public IActionResult Index()
        {
            ViewBag.ProductsCount = _productsCount;
            //var products = _dbContext.Products.Include(x => x.Category).Take(4).ToList();

            return View(/*products*/);
        }

        public IActionResult Load(int skip)
        {
            #region Old way

            //var products = _dbContext.Products.Include(x => x.Category).Skip(4).Take(4).ToList();

            //return Json(products);

            #endregion

            if (skip >= _productsCount)
            {
                return BadRequest();
            }

            var products = _dbContext.Products.Include(x => x.Category).Skip(skip).Take(4).ToList();

            return PartialView("_ProductPartial", products);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
