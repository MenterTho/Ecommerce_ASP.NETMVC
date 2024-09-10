using Ecommerce.Data;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace Ecommerce.Service
{
    public interface IHangHoaService
    {
        Task<PageViewModel<HangHoaViewModel>> GetHangHoasAsync(int? loai, int pageNumber = 1, int pageSize = 10);
        Task<ChiTietHangHoaVM> GetHangHoaDetailAsync(int id);
        Task<List<HangHoaViewModel>> SearchHangHoaAsync(string query);
	}
    public class HangHoaService : IHangHoaService
    {
        private readonly Hshop2023Context db;

        public HangHoaService(Hshop2023Context conetxt)
        {
            db = conetxt;
        }
        
        public async Task<PageViewModel<HangHoaViewModel>> GetHangHoasAsync(int? loai, int pageNumber = 1, int pageSize = 10)
        {
            // Khởi tạo truy vấn
            var hangHoasQuery = db.HangHoas.AsQueryable();

            // Lọc theo loại nếu có
            if (loai.HasValue)
            {
                hangHoasQuery = hangHoasQuery.Where(p => p.MaLoai == loai.Value);
            }

            var totalItems = hangHoasQuery.Count();

            // Lấy dữ liệu phân trang và chọn dữ liệu
            var hangHoasList = await hangHoasQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new HangHoaViewModel
                {
                    MaHh = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? "Empty",
                    MoTaNgan = p.MoTaDonVi ?? "Empty Des",
                    TenLoai = p.MaLoaiNavigation.TenLoai,
                })
                .ToListAsync();

            var model = new PageViewModel<HangHoaViewModel>
            {
                item = hangHoasList,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                PageSize = pageSize
            };

            return model;
        }
		public async Task<List<HangHoaViewModel>> SearchHangHoaAsync(string query)
		{
			var hangHoasQuery = db.HangHoas.AsQueryable();

			if (!string.IsNullOrEmpty(query))
			{
				hangHoasQuery = hangHoasQuery.Where(p => p.TenHh.Contains(query));
			}

			var hangHoasList = await hangHoasQuery
				.Select(p => new HangHoaViewModel
				{
					MaHh = p.MaHh,
					TenHH = p.TenHh,
					DonGia = p.DonGia ?? 0,
					Hinh = p.Hinh ?? "",
					MoTaNgan = p.MoTaDonVi ?? "",
					TenLoai = p.MaLoaiNavigation.TenLoai
				})
				.ToListAsync();

			return hangHoasList;
		}



		public async Task<ChiTietHangHoaVM> GetHangHoaDetailAsync(int id)
        {
            var data = await db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefaultAsync(p => p.MaHh == id);

            if (data == null)
            {
                throw new KeyNotFoundException($"Không thấy sản phẩm có mã {id}");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10, // Tính sau
                DiemDanhGia = 5, // Check sau
            };

            return result;
        }

	}
}
