using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataContext;
using System;
using System.IO;
using System.Threading.Tasks;
using Shop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopDbContext _context;

        public ShopController(ShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult ShowProducts(int categoryId)
        {
            var category = _context.Categories
                .Include(c => c.Articles)
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Request.Cookies["cart"];
            Cart cartItems;
            if (cart == null)
            {
                cartItems = new Cart();
            }
            else
            {
                cartItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Cart>(cart);
            }

            if (cartItems.Items.ContainsKey(id))
            {
                cartItems.Items[id]++;
            }
            else
            {
                cartItems.Items[id] = 1;
            }

            HttpContext.Response.Cookies.Append("cart", Newtonsoft.Json.JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            });

            return RedirectToAction(nameof(Index));
        }
    }
}