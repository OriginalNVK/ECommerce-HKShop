using AutoMapper;
using HShop.Data;
using HShop.ViewModels;

namespace HShop.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() {
            CreateMap<RegisterVM, KhachHang>();
            CreateMap<ClientVM, KhachHang>();
            CreateMap<Loai, CategoryVM>();
            CreateMap<CategoryVM, Loai>();
                //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
                //.ReverseMap();
        }
    }
}
