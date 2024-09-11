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
			// Truy vấn danh sách sản phẩm từ cơ sở dữ liệu
			var hangHoas = _context.HangHoas.AsQueryable();

			// Lọc các sản phẩm dựa trên categoryName (tên loại) và loại bỏ sản phẩm hiện tại
			if (!string.IsNullOrEmpty(categoryName))
			{
				hangHoas = hangHoas.Where(p => p.MaLoaiNavigation.TenLoai == categoryName && p.MaHh != currentProductId);
			}

			// Thực hiện Select để chuyển đổi dữ liệu thành ViewModel
			var relatedProducts = await hangHoas.Select(p => new HangHoaViewModel
			{
				MaHh = p.MaHh,
				TenHH = p.TenHh,
				Hinh = p.Hinh ?? "",
				DonGia = p.DonGia ?? 0,
				MoTaNgan = p.MoTaDonVi ?? "",
				TenLoai = p.MaLoaiNavigation.TenLoai
			})
			.Take(4)  // Lấy tối đa 4 sản phẩm
			.ToListAsync();

			return View(relatedProducts);
		}
	}


}

