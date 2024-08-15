using Ecommerce.Data;
using Ecommerce.Service;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("hanghoa")]
    public class HangHoaController : Controller
    {
        
        private readonly HangHoaService _hangHoaService;
        public HangHoaController(HangHoaService hangHoaService)
        {
            _hangHoaService = hangHoaService;
        }
        [HttpGet("index")]
        public IActionResult Index(int? loai, int pageNumber = 1, int pageSize = 10)
        {
            var result = _hangHoaService.GetHangHoa(loai, pageNumber, pageSize);
            ViewBag.Loai = loai;
            return View(result);
        }
        [HttpGet("search")]
        public IActionResult Search(string? query)
        {
            var result = _hangHoaService.SearchHangHoa(query);
            return View(result);
        }
        [HttpGet("detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            var result = _hangHoaService.GetHangHoaDetail(id);
            if (result == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
            return View(result);
        }
    }
}
