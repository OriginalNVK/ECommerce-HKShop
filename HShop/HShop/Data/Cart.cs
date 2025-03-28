using System;
using System.Collections.Generic;

namespace HShop.Data;

public partial class Cart
{
    public int MaCart { get; set; }

    public string MaKh { get; set; } = null!;

    public int MaHh { get; set; }

    public int SoLuong { get; set; }

    public double DonGia { get; set; }

    public DateTime NgayThem { get; set; }

    public virtual HangHoa MaHhNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
