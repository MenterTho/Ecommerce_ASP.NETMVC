using Ecommerce.Data;
using Ecommerce.Service;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    [ViewComponent(Name = "Product")]
    public class RelatedproductViewComponent : ViewComponent
	{
        private readonly Hshop2023Context _context;

        public RelatedproductViewComponent(Hshop2023Context context)
        {
            _context = context;
        }

		public async Task<IViewComponentResult> InvokeAsync(string categoryName, int currentProductId)
		{
			var allProducts = await _context.HangHoas
				.Where(h => h.MaLoaiNavigation.TenLoai == categoryName && h.MaHh != currentProductId)
				.Select(h => new HangHoaViewModel
				{
					MaHh = h.MaHh,
					TenHH = h.TenHh,
					Hinh = h.Hinh,
					DonGia = h.DonGia ?? 0,
					MoTaNgan = h.MoTaDonVi,
					TenLoai = h.MaLoaiNavigation.TenLoai
				})
				.ToListAsync();

			var groupedProducts = allProducts
				.GroupBy(h => h.TenLoai)
				.Select(g => g.FirstOrDefault())  
				.Take(4)
				.ToList();

			return View(groupedProducts);
		}


	}
}
