using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P320FrontToBack.DataAccessLayer;
using P320FrontToBack.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P320FrontToBack.ViewComponents
{
    //1.0.0 - 1
    //1.1.0 - 2
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public HeaderViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = 0;
            var basket = Request.Cookies["basket"];
            if (!string.IsNullOrEmpty(basket))
            {
                var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
                count = products.Count;
            }
            ViewBag.BasketCount = count;

            var bio = await _dbContext.Bios.SingleOrDefaultAsync();

            return View(bio);
        }
    }
}
