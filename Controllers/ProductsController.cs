using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using BestStoreMVC.Models.DTOs;
using BestStoreMVC.Models;

namespace BestStoreMVC.Controllers
{
    [Route("/admin/[controller]/{action=Index}")]
    public class ProductsController(
        ApplicationDBContext db,
        IWebHostEnvironment webHostEnvironment) : Controller
    {

        private readonly int pageSize = 5;
        public IActionResult Index(int pageNumber,string? search , string? column,string? orderby )
        {
            IQueryable<Product> query = db.Products;


            //search Function 
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x=>x.Name.Contains(search) || x.Brand.Contains(search)); 
            }

            //query string function 
            string[] ValidColumn = {"Id", "Name", "CreatedAt", "Brand", "Category", "Price" };
            string[] ValidOrderBy = { "desc", "asc" };


            if (!ValidColumn.Contains(column)) {
                column = "Id";
            }
            if (!ValidOrderBy.Contains(orderby)) {
                orderby = "desc";
            }


            //pagenation function 
            var count = query.Count();
            if (pageNumber <= 0)
                 pageNumber = 1;

            if (column == "Name")
            {

                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.Name);

                }
                else
                {
                    query = query.OrderByDescending(x => x.Name);
                }
            }

            else if (column == "CreatedAt")
            {

                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.CreatedAt);

                }
                else
                {
                    query = query.OrderByDescending(x => x.CreatedAt);
                }
            }
            else if (column == "Brand")
            {

                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.Brand);

                }
                else
                {
                    query = query.OrderByDescending(x => x.Brand);
                }
            }
            else if (column == "Price")
            {

                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.Price);

                }
                else
                {
                    query = query.OrderByDescending(x => x.Price);
                }
            }
          
            else if (column == "Category")
            {

                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.Category);

                }
                else
                {
                    query = query.OrderByDescending(x => x.Category);
                }
            }
            else
            {
                if (orderby == "asc")
                {
                    query = query.OrderBy(x => x.Id);

                }
                else
                {
                    query = query.OrderByDescending(x => x.Id);
                }
            }
               
           var totalpages = (int)Math.Ceiling((count / (double)pageSize));

           query =   query.Skip((pageNumber - 1) * pageSize).Take(pageSize);


            
            var products  = query.ToList();


            ViewData["TotalPage"] = totalpages;
            ViewData["pageNumber"] = pageNumber;
            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderby;
            


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
