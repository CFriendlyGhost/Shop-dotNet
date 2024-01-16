using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataContext;
using System;
using System.IO;
using System.Threading.Tasks;
using Shop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticleController : Controller
    {
        private readonly ShopDbContext _articleContext;

        public ArticleController(ShopDbContext context)
        {
            _articleContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var articlesDbContext = _articleContext.Articles
                .Include(a => a.Category);

            var articlesWithImagePaths = await articlesDbContext.ToListAsync();


            foreach (var article in articlesWithImagePaths)
            {
                if(article.FileName != null)
                {
                    string path = "~/upload";
                    article.FileName = Path.Combine(path, article.FileName);
                }
            }


            return View(articlesWithImagePaths);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _articleContext.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }


            if (article.FileName != null)
            {
                string path = "~/upload";
                article.FileName = Path.Combine(path, article.FileName);
            }

            var categoryId = article.CategoryId;
            article.Category = _articleContext.Categories.FirstOrDefault(m => m.CategoryId == categoryId);

            return View(article);
        }

        public ActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_articleContext.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id", "BarCode", "ProductName", "Price", "File", "FileName", "CountryOfOrigin", "Weight", "ExpirationDate", "CategoryId")] Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(article.File != null)
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(article.File.FileName)}";
                        string filePath = Path.Combine(path, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await article.File.CopyToAsync(stream);
                        }

                        article.FileName = fileName;
                    }

                    _articleContext.Add(article);
                    await _articleContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                
                }
                return View(nameof(Create), article);
            }
            catch(DbUpdateException) 
            {
                return RedirectToAction(nameof(Create));
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _articleContext.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_articleContext.Categories, "CategoryId", "CategoryName");
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "BarCode", "ProductName", "Price", "CountryOfOrigin", "Weight", "ExpirationDate", "CategoryId")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var existingArticle = await _articleContext.Articles
                        .FirstOrDefaultAsync(a => a.Id == article.Id);

                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    article.FileName = existingArticle.FileName;
                    _articleContext.Entry(existingArticle).CurrentValues.SetValues(article);

                    await _articleContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExist(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _articleContext.Articles
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _articleContext.Articles.FindAsync(id);
            if (article.FileName != null)
            {
                var path = Path.GetFullPath("wwwroot");
                System.IO.File.Delete(Path.Combine(path, "upload", article.FileName));
            }
            _articleContext.Articles.Remove(article);
            await _articleContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExist(int id)
        {
            return _articleContext.Articles.Any(a => a.Id == id);
        }
    }
}