using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class StoreController(ApplicationDBContext  db) : Controller
    {
        private readonly int PageSize = 8;
        public IActionResult Index(int PageNumber )
        {
           IQueryable<Product> query = db.Products;

            if(PageNumber <= 0)
                PageNumber = 1;

            query = query.OrderByDescending(x => x.Id);

            var count  = query.Count();
            var totalPages  =   (int)Math.Ceiling(count/(double) PageSize);




            query= query.Skip((PageNumber-1) * PageSize).Take(PageSize);

            var products = query.ToList();
            ViewBag.Products = products;
            ViewBag.PageNumber = PageNumber;
            ViewBag.totalPages = totalPages;







            return View();
        }
    }
}
