using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using BestStoreMVC.Models.DTOs;
using BestStoreMVC.Models;

namespace BestStoreMVC.Controllers
{
    public class ProductsController(ApplicationDBContext db, IWebHostEnvironment webHostEnvironment) : Controller
    {
        public IActionResult Index()
        {
            var query = db.Products;.OrderByDescending(x => x.Id).ToList();




            return View(products);
        }

        public IActionResult Create()
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
        public IActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // يفضل استخدام ViewModel أو DTO خاص بالـ Edit
            var productDto = new ProductDTO
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

            // نمرر بيانات المنتج الأصلي للـ View
            ViewData["ProductId"] = product.Id;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
            ViewData["ImageFileName"] = product.ImageFileName;

            return View(productDto);
        }


        [HttpPost]
        public IActionResult Edit(int id, ProductDTO ProductDTO)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                ViewData["ImageFileName"] = product.ImageFileName;

                return View(ProductDTO);
            }

             string? newimg = product.ImageFileName;


            if (ProductDTO.ImageFile != null)
            {
            //update imge new first then delete old
             newimg = Guid.NewGuid().ToString() + Path.GetExtension(ProductDTO.ImageFile.FileName);

            var fullPath = Path.Combine(webHostEnvironment.WebRootPath, "products", newimg);
            using (var stream  = System.IO.File.Create(fullPath)) {
                ProductDTO.ImageFile.CopyTo(stream);
            }






            var oldimg = Path.Combine(webHostEnvironment.WebRootPath, "products", product.ImageFileName!);
            System.IO.File.Delete(oldimg);
                // ارفع الصورة الجديدة واحذف القديمة
            }

            product.Name = ProductDTO.Name;
            product.Description = ProductDTO.Description;
            product.Brand = ProductDTO.Brand;
            product.Price = ProductDTO.Price;
            product.Category = ProductDTO.Category;
            product.ImageFileName = newimg;

            db.SaveChanges();
            
            return RedirectToAction(nameof(Index));
            


        }

        public IActionResult Delete(int id){

            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var fileName = Path.Combine(webHostEnvironment.WebRootPath,
                "products", product.ImageFileName!);

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            db.Products.Remove(product);

            db.SaveChanges();

            return RedirectToAction(nameof(Index));

        }


    }
}
