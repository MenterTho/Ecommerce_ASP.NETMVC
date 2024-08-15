using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
	public class LoginViewModel
	{
		[Display(Name = "Tên đăng nhập")]
		[Required(ErrorMessage = "Chưa nhập tên đăng nhập")]
		[MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
		public string UserName { get; set; }

		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Chưa nhập mật khẩu")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
