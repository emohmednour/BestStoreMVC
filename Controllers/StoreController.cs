using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class StoreController(ApplicationDBContext  db) : Controller
    {
        public IActionResult Index()
        {
            var products  = db.Products.OrderByDescending(x=>x.Id).ToList();
            ViewBag.Products = products;
            return View();
        }
    }
}
