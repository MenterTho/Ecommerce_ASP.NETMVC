using Ecommerce.Helpers;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ViewComponents
{
	public class CartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var cart = HttpContext.Session.Get<List<CartViewModel>>(MySetting.CART_KEY) ?? new List<CartViewModel>();

			return View("CartPanel", new CartModel
			{
				Quantity = cart.Sum(p => p.SoLuong),
				Total = cart.Sum(p => p.ThanhTien)
			});
		}
	}
}
