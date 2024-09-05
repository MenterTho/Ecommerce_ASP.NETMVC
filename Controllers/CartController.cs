using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.Service;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Controllers
{
    [Route("Cart")]
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
            var cart = _cartService.GetCartItems();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost("add/{id}")]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            try
            {
                _cartService.AddToCart(id, quantity);
                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Message"] = ex.Message; 
                return Redirect("/404");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost("remove/{id}")]
        public IActionResult RemoveCart(int id)
        {
            try
            {
                _cartService.RemoveCart(id);
                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Message"] = ex.Message;
                return Redirect("/404");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        // Hiển thị trang thanh toán
        [Authorize]
        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartItems();
            if (cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return Redirect("/");
            }

            return View(cart);
        }

        // Xử lý yêu cầu thanh toán
        [Authorize]
		[HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                if (customerId == null)
                {
                    return Unauthorized();
                }

                try
                {
                    await _cartService.Checkout(model, customerId);
                    return View("Success");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi khi thanh toán: {ex.Message}");
                }
            }

            return View(_cartService.GetCartItems());
        }
    }
}

