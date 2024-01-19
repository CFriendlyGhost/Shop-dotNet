using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.DataContext;
using System.Linq;
using Shop.Models;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections;


namespace Shop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ShopDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .FirstOrDefault();

                cartItems ??= new Cart();

                var productsInCart = _context.Articles
                    .Where(article => cartItems.Items.Keys.Contains(article.Id))
                    .ToList();

                var productQuantities = cartItems.Items;
                ViewBag.ProductQuantities = productQuantities;

                var cartValue = 0f;

                foreach (var product in productsInCart)
                {
                    if (cartItems.Items.TryGetValue(product.Id, out int quantity))
                    {
                        cartValue += product.Price * quantity;
                    }
                }
                ViewBag.CartValue = cartValue;
                return View(productsInCart);
            }
            else
            {
                TempData["MustBeLoggedIn"] = "You must to be logged in to finish checkout!";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }
        public async Task<IActionResult> CompleteCheckout()
        {
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .FirstOrDefault();
/*
                IdentityUser user = _context.Users.Find(userId);

                if (user != null)
                {
                    

                    _context.SaveChanges();
                }*/
                cartItems.Items.Clear();
                _context.Carts.Update(cartItems);
                _context.SaveChanges();
                
            }
            return RedirectToAction("Index", "Cart");
        }
    }
}
