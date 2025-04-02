﻿using HShop.Data;

namespace HShop.ViewModels
{
    public class ProductAdminVM
    {
        public int MaHh { get; set; }

        public string TenHh { get; set; } = null!;

        public string? TenAlias { get; set; }

        public int MaLoai { get; set; }

        public string? MoTaDonVi { get; set; }

        public double? DonGia { get; set; }

        public string? Hinh { get; set; }

        public DateTime NgaySx { get; set; }

        public double GiamGia { get; set; }

        public int SoLanXem { get; set; }

        public string? MoTa { get; set; }

        public string MaNcc { get; set; } = null!;

        public virtual Loai MaLoaiNavigation { get; set; } = null!;

        public virtual NhaCungCap MaNccNavigation { get; set; } = null!;
    }
}
