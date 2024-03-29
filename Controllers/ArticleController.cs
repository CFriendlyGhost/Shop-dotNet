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
using Shop.Services;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticleController : Controller
    {
        private readonly ShopDbContext _articleContext;
        private readonly IImageService _imageService;

        public ArticleController(ShopDbContext context, IImageService imageService)
        {
            _articleContext = context;
            _imageService = imageService;   
        }

        public async Task<IActionResult> Index()
        {
            var articlesDbContext = _articleContext.Articles
                .Include(a => a.Category);

            var articlesWithImagePaths = await articlesDbContext.ToListAsync();

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
        public async Task<IActionResult> Create([Bind("Id", "BarCode", "ProductName", "Price", "File", "FileName", "CategoryId")] Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(article.File != null)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(article.FileName)}";
                        var fileUrl = await _imageService.UploadImageToAzureStorage(article.File, fileName);
                        article.FileName = fileUrl;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id", "BarCode", "ProductName", "Price", "CategoryId")] Article article)
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
                await _imageService.DeleteImage(article.FileName);
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