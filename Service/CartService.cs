using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.ViewModels;

namespace Ecommerce.Service
{
    public class CartService
    {
        private readonly Hshop2023Context db;

        public CartService(Hshop2023Context conetxt)
        {
            db = conetxt;
        }
        public List<CartViewModel> GetCart(HttpContext context)
        {
            return context.Session.Get<List<CartViewModel>>(MySetting.CART_KEY) ?? new List<CartViewModel>();
        }
        public void AddToCart(HttpContext context, int id, int quantity)
        {
            var cart = GetCart(context);
            var item = cart.SingleOrDefault(p => p.MaHh == id);

            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null) throw new KeyNotFoundException($"Không tìm thấy hàng hóa có mã {id}");

                item = new CartViewModel
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                cart.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            context.Session.Set(MySetting.CART_KEY, cart);
        }
        // Xóa sản phẩm khỏi giỏ hàng
        public void RemoveCart(HttpContext context, int id)
        {
            var cart = GetCart(context);
            var item = cart.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                cart.Remove(item);
                context.Session.Set(MySetting.CART_KEY, cart);
            }
        }
        // Xử lý thanh toán
        public bool Checkout(HttpContext context,CheckoutVM model, string customerId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId) ?? new KhachHang();

            var hoadon = new HoaDon
            {
                MaKh = customerId,
                HoTen = model.HoTen ?? khachHang.HoTen,
                DiaChi = model.DiaChi ?? khachHang.DiaChi,
                NgayDat = DateTime.Now,
                CachThanhToan = "COD",
                CachVanChuyen = "GRAB",
                MaTrangThai = 0,
                GhiChu = model.GhiChu
            };

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in GetCart(context))
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }
    }
}
