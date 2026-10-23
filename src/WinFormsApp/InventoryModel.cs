namespace WinFormsApp
{
    public class InventoryModel
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuongTon { get; set; }
        public decimal DonGia { get; set; }
        public decimal TongGiaiTri => SoLuongTon * DonGia;
    }
}