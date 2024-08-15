namespace Ecommerce.ViewModels
{
    public class PageViewModel<T>
    {
        public IEnumerable<T> item { get; set; } // Danh sách hàng hóa
        public int PageNumber { get; set; } // Số trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
        public int PageSize { get; set; } // Số lượng sản phẩm mỗi trang

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
