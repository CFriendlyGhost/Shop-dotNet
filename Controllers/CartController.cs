﻿using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Newtonsoft.Json;
using Shop.DataContext;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Collections.Generic;


namespace Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ShopDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
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
            }
            else
            {
                var cart = Request.Cookies["cart"];
                if (cart == null)
                {
                    cartItems = new Cart();
                }
                else
                {
                    cartItems = JsonConvert.DeserializeObject<Cart>(cart);
                }
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
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .FirstOrDefault();

                cartItems.Items[id]++;
                _context.Carts.Update(cartItems);
                _context.SaveChanges();
            }
            else
            {
                var cart = Request.Cookies["cart"];
                cartItems = JsonConvert.DeserializeObject<Cart>(cart);

                var productsInCart = _context.Articles
                    .Where(article => cartItems.Items.Keys.Contains(article.Id))
                    .ToList();

                cartItems.Items[id]++;
                var updatedCart = JsonConvert.SerializeObject(cartItems);
                Response.Cookies.Append("cart", updatedCart);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Decrease(int id)
        {
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .FirstOrDefault();

                if (cartItems.Items[id] == 1)
                {
                    cartItems.Items.Remove(id);
                }
                else
                {
                    cartItems.Items[id]--;
                }
                _context.Carts.Update(cartItems);
                _context.SaveChanges();
            }
            else
            {
                var cart = Request.Cookies["cart"];
                cartItems = JsonConvert.DeserializeObject<Cart>(cart);

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
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ClaimsPrincipal claimsPrincipal = this.User;
            Cart cartItems;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                string userId = _userManager.GetUserId(claimsPrincipal);
                cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .FirstOrDefault();

                cartItems.Items.Remove(id);
                _context.Carts.Update(cartItems);
                _context.SaveChanges();
            }
            else
            {
                var cart = Request.Cookies["cart"];
                cartItems = JsonConvert.DeserializeObject<Cart>(cart);

                var productsInCart = _context.Articles
                    .Where(article => cartItems.Items.Keys.Contains(article.Id))
                    .ToList();

                cartItems.Items.Remove(id);
                var updatedCart = JsonConvert.SerializeObject(cartItems);
                Response.Cookies.Append("cart", updatedCart);
            }

            return RedirectToAction("Index");
        }
    }
}
