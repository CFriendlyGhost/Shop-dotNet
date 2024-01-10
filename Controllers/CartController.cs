using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Newtonsoft.Json;
using Shop.DataContext;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;

        public CartController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cart = Request.Cookies["cart"];
            Cart cartItems;
            if (cart == null)
            {
                cartItems = new Cart();
            }
            else
            {
                cartItems = JsonConvert.DeserializeObject<Cart>(cart);
            }

            var productsInCart = _context.Articles
                .Where(article => cartItems.Items.Keys.Contains(article.Id))
                .ToList();

            var productQuantities = cartItems.Items;
            ViewBag.ProductQuantities = productQuantities;

            var cartValue = 0f;

            foreach (var product in productsInCart)
            {
                if(cartItems.Items.TryGetValue(product.Id, out int quantity))
                {
                    cartValue += product.Price * quantity;
                }
            }
            ViewBag.CartValue = cartValue;

            return View(productsInCart);
        }

        public async Task<IActionResult> IndexJs()
        {
            var cart = Request.Cookies["cart"];
            Cart cartItems;
            if (cart == null)
            {
                cartItems = new Cart();
            }
            else
            {
                cartItems = JsonConvert.DeserializeObject<Cart>(cart);
            }

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

        [HttpGet]
        public async Task<IActionResult> AddMore(int id)
        {
            var cart = Request.Cookies["cart"];
            var cartItems = JsonConvert.DeserializeObject<Cart>(cart);

            var productsInCart = _context.Articles
                .Where(article => cartItems.Items.Keys.Contains(article.Id))
                .ToList();

            cartItems.Items[id]++;

            var updatedCart = JsonConvert.SerializeObject(cartItems);
            Response.Cookies.Append("cart", updatedCart);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Decrease(int id)
        {
            var cart = Request.Cookies["cart"];
            var cartItems = JsonConvert.DeserializeObject<Cart>(cart);

            var productsInCart = _context.Articles
                .Where(article => cartItems.Items.Keys.Contains(article.Id))
                .ToList();

            if (cartItems.Items[id] == 1)
            {
                cartItems.Items.Remove(id);
            }

            else
            {
                cartItems.Items[id]--;
            }

            var updatedCart = JsonConvert.SerializeObject(cartItems);
            Response.Cookies.Append("cart", updatedCart);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cart = Request.Cookies["cart"];
            var cartItems = JsonConvert.DeserializeObject<Cart>(cart);

            var productsInCart = _context.Articles
                .Where(article => cartItems.Items.Keys.Contains(article.Id))
                .ToList();

            cartItems.Items.Remove(id);

            var updatedCart = JsonConvert.SerializeObject(cartItems);
            Response.Cookies.Append("cart", updatedCart);

            return RedirectToAction("Index");
        }
    }
}
