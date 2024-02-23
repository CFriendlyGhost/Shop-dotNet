using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DataContext;
using System.Linq;
using Shop.Models;
using System.Security.Claims;
using System.IO;
using System.Threading.Tasks;
using System;


namespace Shop.Controllers
{
    public class DiscountController : Controller
    {
		private readonly ShopDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

        public DiscountController(ShopDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
			ClaimsPrincipal claimsPrincipal = this.User;

			string userId = _userManager.GetUserId(claimsPrincipal);
			var discount = _context.Discounts.Where(c => c.CustomerId == userId).FirstOrDefault();
			ViewBag.points = discount.points;
			return View();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<IActionResult> DiscountChoose(int pointsValue)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			ClaimsPrincipal claimsPrincipal = this.User;

			string userId = _userManager.GetUserId(claimsPrincipal);
			var discount = _context.Discounts.Where(c => c.CustomerId == userId).FirstOrDefault();

			if (discount == null && pointsValue != 0)
			{
				discount = new Discount { CustomerId = userId, points = 0};
				_context.Discounts.Add(discount);
				TempData["NoPoints"] = "Nie posiadasz żadnych punktów, nie możesz zrealizować zniżki!";
			}

			else if(discount.points >= pointsValue && pointsValue != 0)
			{
				discount.points -= pointsValue;
				discount.discountValue = pointsValue/100;
				_context.Discounts.Update(discount);
				TempData["DiscountAdded"] = "Kupon został pomyślnie dodany do Twojego konta!";
			}

			else if(discount.points < pointsValue && pointsValue != 0)
            {
                TempData["NotEnoughPoints"] = "Nie masz wystarczająco punktów aby zrealizować zniżkę!";
            }

            _context.SaveChanges();
			return View();
		}
	}
}
