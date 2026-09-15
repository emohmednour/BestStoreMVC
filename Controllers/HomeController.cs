using System.Diagnostics;
using BestStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using BestStoreMVC.Services;

namespace BestStoreMVC.Controllers
{
    public class HomeController(ApplicationDBContext db) : Controller
    {
        public IActionResult Index()
        {
            return View(db.Products.OrderByDescending(x=>x.Id).Take(4).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
