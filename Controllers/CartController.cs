using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.Service;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // Hiển thị giỏ hàng
        [HttpGet("index")]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart(HttpContext);
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost("add/{id}")]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            try
            {
                _cartService.AddToCart(HttpContext, id, quantity);
                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Message"] = ex.Message;
                return Redirect("/404");
            }
        }


        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost("remove/{id}")]
        public IActionResult RemoveCart(int id)
        {
            _cartService.RemoveCart(HttpContext, id);
            return RedirectToAction("Index");
        }

        // Hiển thị trang thanh toán
        [Authorize]
        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart(HttpContext);
            if (cart.Count == 0)
            {
                return Redirect("/");
            }

            return View(cart);
        }

        // Xử lý yêu cầu thanh toán
        [Authorize]
		[HttpPost("checkout")]
		public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                if (customerId == null)
                {
                    return Unauthorized();
                }

                var success = _cartService.Checkout(HttpContext,model, customerId, out string errorMessage);

                if (success)
                {
                    return View("Success");
                }

                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return View(_cartService.GetCart(HttpContext));
        }
    }
}

