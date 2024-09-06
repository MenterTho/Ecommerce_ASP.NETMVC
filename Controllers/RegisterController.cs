using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Service;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using static Ecommerce.ViewModels.RegisterViewModel;
using Microsoft.EntityFrameworkCore.Internal;

namespace Ecommerce.Controllers
{
    [Route("User")]
    public class RegisterController : Controller
	{
        private readonly IRegisterService _khachHangService;

        public RegisterController(IRegisterService khachHangService)
        {
            _khachHangService = khachHangService;
        }
        [HttpGet("DangKy")]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost("DangKy")]
        public async Task<IActionResult> DangKy(RegisterViewModel model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _khachHangService.DangKyAsync(model, Hinh);
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet("DangNhap")]
        public IActionResult DangNhap(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

        [HttpPost("DangNhap")]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = await _khachHangService.DangNhapAsync(model);

                    // Xử lý đăng nhập và Claims
                    var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, khachHang.Email),
                    new Claim(ClaimTypes.Name, khachHang.HoTen),
                    new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return Redirect("/");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet("Profile")]
        public IActionResult Profile()
		{
			return View();
		}

        [Authorize]
        [HttpGet("DangXuat")]
        public async Task<IActionResult> DangXuat()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
	}
}
