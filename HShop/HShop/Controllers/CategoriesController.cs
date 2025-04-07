using AutoMapper;
using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper mapper;

        public CategoriesController(Hshop2023Context db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/admin/categories/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var Category = await db.Loais.FirstOrDefaultAsync(c => c.MaLoai == id);
            if(Category == null)
            {
                return NotFound();
            }
            var categoryDTO = mapper.Map<CategoryVM>(Category);

            return View(categoryDTO);
        }
    }
}
