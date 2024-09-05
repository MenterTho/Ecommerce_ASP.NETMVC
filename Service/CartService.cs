using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Service
{
    public interface ICartService
    {
        // using promise 
        List<CartViewModel> GetCartItems();
        Task AddToCart(int id, int quantity);
        Task RemoveFromCart(int id);
        Task Checkout(CheckoutVM model, string customerId);
    }
    public class CartService: ICartService
    {
        private readonly Hshop2023Context db;

        private readonly ISession session;
        public CartService(Hshop2023Context conetxt, IHttpContextAccessor httpContextAccessor)
        {
            db = conetxt;
            session = httpContextAccessor.HttpContext.Session;

        }
        public List<CartViewModel> GetCart()
        {
            return session.Get<List<CartViewModel>>(MySetting.CART_KEY) ?? new List<CartViewModel>();
        }
        public async Task AddToCart(int id, int quantity)
        {
            try
            {
                var cart = GetCartItems();
                var item = cart.SingleOrDefault(p => p.MaHh == id);

                if (item == null)
                {
                    var hangHoa = await db.HangHoas.SingleOrDefaultAsync(p => p.MaHh == id);
                    if (hangHoa == null)
                        throw new KeyNotFoundException($"Không tìm thấy hàng hóa có mã {id}");

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

                session.Set(MySetting.CART_KEY, cart);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần)
                throw new Exception($"Lỗi khi thêm hàng vào giỏ: {ex.Message}");
            }
        }
        // Xóa sản phẩm khỏi giỏ hàng
        public async Task RemoveCart(int id)
        {
            try
            {
                var cart = GetCartItems();
                var item = cart.SingleOrDefault(p => p.MaHh == id);
                if (item != null)
                {
                    cart.Remove(item);
                    session.Set(MySetting.CART_KEY, cart);
                }
                else
                {
                    throw new KeyNotFoundException($"Không tìm thấy sản phẩm có mã {id} trong giỏ hàng.");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần)
                throw new Exception($"Lỗi khi xóa sản phẩm khỏi giỏ hàng: {ex.Message}");
            }
        }
        // Xử lý thanh toán
        public async Task Checkout(CheckoutVM model, string customerId)
        {
            try
            {
                var khachHang = await db.KhachHangs.SingleOrDefaultAsync(kh => kh.MaKh == customerId);
                if (khachHang == null)
                {
                    throw new Exception("Không tìm thấy khách hàng.");
                }

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

                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await db.AddAsync(hoadon);
                        await db.SaveChangesAsync();

                        var cthds = new List<ChiTietHd>();
                        foreach (var item in GetCartItems())
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

                        await db.AddRangeAsync(cthds);
                        await db.SaveChangesAsync();

                        session.Set<List<CartViewModel>>(MySetting.CART_KEY, new List<CartViewModel>());
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"Lỗi khi xử lý thanh toán: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thực hiện thanh toán: {ex.Message}");
            }
        }

        public List<CartViewModel> GetCartItems()
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromCart(int id)
        {
            throw new NotImplementedException();
        }
    }
}
