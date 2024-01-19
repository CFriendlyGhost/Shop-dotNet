using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    public class CheckoutController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
