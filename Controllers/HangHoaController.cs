using Ecommerce.Data;
using Ecommerce.Service;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("product")]
    public class HangHoaController : Controller
    {
        
        private readonly HangHoaService _hangHoaService;
        public HangHoaController(HangHoaService hangHoaService)
        {
            _hangHoaService = hangHoaService;
        }
        [HttpGet("index")]
        public async Task<IActionResult> Index(int? loai, int pageNumber = 1, int pageSize = 10)
        {
            var hangHoas = await _hangHoaService.GetHangHoasAsync(loai, pageNumber, pageSize);
            return View(hangHoas);
        }
        [HttpGet("search")]
		public async Task<IActionResult> Search(string? query)
		{
			var hangHoas = await _hangHoaService.SearchHangHoaAsync(query);
			return View(hangHoas); // Trả về kết quả cho view
		}
		[HttpGet("detail/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var hangHoaDetail = await _hangHoaService.GetHangHoaDetailAsync(id);
                return View(hangHoaDetail);
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home"); // Giả sử bạn có Action Error trong HomeController
            }
        }
    }
}
