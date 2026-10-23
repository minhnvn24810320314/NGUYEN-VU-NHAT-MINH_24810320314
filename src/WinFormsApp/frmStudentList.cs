using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmStudentList : Form
    {
        public frmStudentList()
        {
            this.Text = "Danh Sách Sinh Viên";
            this.Size = new Size(550, 350);

            Label lbl = new Label
            {
                Text = "QUẢN LÝ HỒ SƠ SINH VIÊN",
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.Columns.Add("MaSV", "Mã Sinh Viên");
            dgv.Columns.Add("HoTen", "Họ và Tên");
            dgv.Columns.Add("Lop", "Lớp Học");

            dgv.Rows.Add("SV001", "Nguyễn Vũ Nhật Minh", "CNTT K18");
            dgv.Rows.Add("SV002", "Trần Văn Nam", "CNTT K18");

            this.Controls.Add(dgv);
            this.Controls.Add(lbl);
        }
    }
}