using Ecommerce.Data;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Service
{
    public class HangHoaService
    {
        private readonly Hshop2023Context db;

        public HangHoaService(Hshop2023Context conetxt)
        {
            db = conetxt;
        }

        public PageViewModel<HangHoaViewModel> GetHangHoa(int? loai, int pageNumber = 1, int pageSize = 10)
        {
            var HangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                HangHoas = HangHoas.Where(p => p.MaLoai == loai.Value);
            }

            int totalItems = HangHoas.Count();
            var hangHoasList = HangHoas
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
                                .ToList();

            return new PageViewModel<HangHoaViewModel>
            {
                item = hangHoasList, // chứa ds product
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                PageSize = pageSize
            };
        }
        public IQueryable<HangHoaViewModel> SearchHangHoa(string query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            return hangHoas.Select(p => new HangHoaViewModel
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
        }

        public ChiTietHangHoaVM GetHangHoaDetail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                return null;
            }

            return new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10, // tính sau
                DiemDanhGia = 5, // check sau
            };
        }

    }
}
