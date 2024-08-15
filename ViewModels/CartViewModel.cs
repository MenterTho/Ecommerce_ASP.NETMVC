namespace Ecommerce.ViewModels
{
	public class CartViewModel
	{
		public int MaHh { get; set; }
		public string Hinh { get; set; }
		public string TenHH { get; set; }
		public double DonGia { get; set; }
		public int SoLuong { get; set; }
		public double ThanhTien => SoLuong * DonGia;
	}
	public class CartModel
	{
		public int Quantity { get; set; }
		public double Total { get; set; }
	}
	public class CheckoutVM
	{
		public bool GiongKhachHang { get; set; }
		public string? HoTen { get; set; }
		public string? DiaChi { get; set; }
/*		public string? DienThoai { get; set; }*/
		public string? GhiChu { get; set; }
	}
}
