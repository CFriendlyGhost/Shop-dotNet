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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(ShopDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            ClaimsPrincipal claimsPrincipal = this.User;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                AddToCartForAuthenticatedUser(id);
            }
            else
            {
                AddToCartForGuestUser(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private void AddToCartForAuthenticatedUser(int id)
        {
            string userId = _userManager.GetUserId(User);
            Cart cartItems = _context.Carts.SingleOrDefault(c => c.UserId == userId);

            if (cartItems == null)
            {
                cartItems = new Cart
                {
                    UserId = userId
                };
                _context.Carts.Add(cartItems);
            }

            UpdateCartItems(cartItems, id);
            _context.SaveChanges();
        }

        private void AddToCartForGuestUser(int id)
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

            UpdateCartItems(cartItems, id);

            HttpContext.Response.Cookies.Append("cart", Newtonsoft.Json.JsonConvert.SerializeObject(cartItems), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            });
        }

        private void UpdateCartItems(Cart cartItems, int id)
        {
            if (cartItems.Items.ContainsKey(id))
            {
                cartItems.Items[id]++;
            }
            else
            {
                cartItems.Items[id] = 1;
            }
        }
    }
}