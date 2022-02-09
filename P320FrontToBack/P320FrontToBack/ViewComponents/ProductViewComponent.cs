using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P320FrontToBack.DataAccessLayer;
using System.Linq;
using System.Threading.Tasks;

namespace P320FrontToBack.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public ProductViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(int take = 8)
        {
            var products = await _dbContext.Products.Include(x => x.Category).Take(take).ToListAsync();

            return View(products);
        }
    }
}
