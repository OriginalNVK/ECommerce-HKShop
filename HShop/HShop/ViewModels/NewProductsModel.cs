using HShop.Data;
using Microsoft.Build.Evaluation;

namespace HShop.ViewModels
{
    public class NewProductsModel
    {
        public List<CategoryProducts> Categories { get; set; }
    }

    public class CategoryProducts
    {
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
        public List<HangHoaVM> Products { get; set; }
    }
}
