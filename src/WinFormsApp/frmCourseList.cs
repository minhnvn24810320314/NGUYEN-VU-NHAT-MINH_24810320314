using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmCourseList : Form
    {
        public frmCourseList()
        {
            this.Text = "Danh Mục Môn Học";
            this.Size = new Size(550, 350);

            Label lbl = new Label
            {
                Text = "DANH MỤC MÔN HỌC & ĐÀO TẠO",
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
            dgv.Columns.Add("MaMH", "Mã Môn");
            dgv.Columns.Add("TenMH", "Tên Môn Học");
            dgv.Columns.Add("SoTC", "Số Tín Chỉ");

            dgv.Rows.Add("PRG201", "Lập trình C# WinForms", "3");
            dgv.Rows.Add("DBI202", "Cơ sở dữ liệu SQL", "3");

            this.Controls.Add(dgv);
            this.Controls.Add(lbl);
        }
    }
}