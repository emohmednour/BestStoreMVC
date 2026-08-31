using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BestStoreMVC.Models.DTOs;
using BestStoreMVC.Models;

namespace BestStoreMVC.Controllers
{
    public class ProductsController(ApplicationDBContext  db , IWebHostEnvironment webHostEnvironment) : Controller
    {
        public IActionResult Index()
        {
            var products  = db.Products.OrderByDescending(x=>x.Id).ToList();
            return View(products);
        }

        public  IActionResult Create()
        {
           return View();
        }

      [HttpPost]
public IActionResult Create(ProductDTO productDto)
{
    if (productDto.ImageFile == null)
    {
        ModelState.AddModelError("ImageFile", "Please select an image file.");
    }

    if (!ModelState.IsValid)
    {
        return View(productDto);
    }

    // Save image file to wwwroot/images folder
    var fileName = Guid.NewGuid().ToString() +
                   Path.GetExtension(productDto.ImageFile.FileName);

    var imagePath = Path.Combine(
        webHostEnvironment.WebRootPath,
        "products",
        fileName
    );

    using (var stream = System.IO.File.Create(imagePath))
    {
        productDto.ImageFile.CopyTo(stream);
    }

    var product = new Product
    {
        Name = productDto.Name,
        Brand = productDto.Brand,
        Category = productDto.Category,
        Price = productDto.Price,
        Description = productDto.Description,
        ImageFileName = fileName,      
        CreatedAt = DateTime.Now 
    };

    db.Products.Add(product);
    db.SaveChanges();

    return RedirectToAction("Index", "Products");
}

    }
}
