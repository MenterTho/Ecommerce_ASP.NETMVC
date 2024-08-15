using Ecommerce.Data;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Ecommerce.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;
        public CategoriesViewComponent(Hshop2023Context context) => db = context;
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new CategoriesViewModel
            {
                Maloai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }); 
            return View(data); 

        }
    }
}
