using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Ecommerce.Service
{
    public interface IRegisterService
    {
        Task DangKyAsync(RegisterViewModel model, IFormFile hinh);

        Task<KhachHang> DangNhapAsync(LoginViewModel model);
    }
	public class RegisterService : IRegisterService
	{
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        public RegisterService(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }



        public async Task DangKyAsync(RegisterViewModel model, IFormFile Hinh)
        {
            var khachHang = _mapper.Map<KhachHang>(model);
            khachHang.RandomKey = MyUtil.GenerateRamdomKey();
            khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
            khachHang.HieuLuc = true;
            khachHang.VaiTro = 0;

            if (Hinh != null)
            {
                var extension = Path.GetExtension(Hinh.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    throw new Exception("Chỉ được upload hình ảnh định dạng .jpg, .png, .gif.");
                }

                khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
            }

            db.Add(khachHang);
            await db.SaveChangesAsync();
        }

        public async Task<KhachHang> DangNhapAsync(LoginViewModel model)
        {
            var khachHang = await db.KhachHangs.SingleOrDefaultAsync(kh => kh.MaKh == model.UserName);

            if (khachHang == null)
            {
                throw new Exception("Không có khách hàng này.");
            }

            if (!khachHang.HieuLuc)
            {
                throw new Exception("Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
            }

            if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
            {
                throw new Exception("Sai thông tin đăng nhập.");
            }

            return khachHang;
        }


    }

}
