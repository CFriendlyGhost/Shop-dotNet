using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataContext;
using System.Linq;
using Shop.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


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

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

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

                ViewBag.CartValue = sumCartValue(productsInCart, cartItems);
                return View(productsInCart);
            }
            else
            {
                TempData["MustBeLoggedIn"] = "You must to be logged in to finish checkout!";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> CompleteCheckout()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);                
                var discount = _context.Discounts.Where(c => c.CustomerId == userId).FirstOrDefault();
                cartItems = _context.Carts.Where(c => c.UserId == userId).FirstOrDefault();
                var productsInCart = _context.Articles
                    .Where(article => cartItems.Items.Keys.Contains(article.Id))
                    .ToList();

                if (discount == null) {
                    discount = new Discount { CustomerId = userId, points = (int)sumCartValue(productsInCart, cartItems)};
                    _context.Discounts.Add(discount);
                }

                cartItems.Items.Clear();
                _context.Carts.Update(cartItems);
                _context.SaveChanges();
                
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult ShowConfirmation(CheckoutDetails checkoutDetails)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CompleteCheckout();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return View(checkoutDetails);
        }

        public float sumCartValue(List<Article> productsInCart, Cart cartItems)
        {
            var cartValue = 0f;

            foreach (var product in productsInCart)
            {
                if (cartItems.Items.TryGetValue(product.Id, out int quantity))
                {
                    cartValue += product.Price * quantity;
                }
            }
            return cartValue;
        }
    }
}
