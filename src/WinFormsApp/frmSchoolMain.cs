using System;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmSchoolMain : Form
    {
        private MenuStrip menuStrip1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblCanBo;
        private ToolStripProgressBar progressBar1;

        public frmSchoolMain()
        {
            this.IsMdiContainer = true;
            this.Text = "Hệ Thống MDI Quản Lý Trường Học & Đào Tạo";
            this.Size = new System.Drawing.Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            menuStrip1 = new MenuStrip();

            // Menu Hệ thống
            ToolStripMenuItem menuHeThong = new ToolStripMenuItem("Hệ thống");
            menuHeThong.DropDownItems.Add("Đăng xuất");
            menuHeThong.DropDownItems.Add("Đổi mật khẩu");
            menuHeThong.DropDownItems.Add(new ToolStripSeparator());
            menuHeThong.DropDownItems.Add(new ToolStripMenuItem("Thoát", null, (s, e) => Application.Exit()));

            // Menu Quản lý
            ToolStripMenuItem menuQuanLy = new ToolStripMenuItem("Quản lý");
            menuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Danh sách Sinh viên", null, OpenStudentList_Click));
            menuQuanLy.DropDownItems.Add(new ToolStripMenuItem("Danh mục Môn học", null, OpenCourseList_Click));

            // Menu Cửa sổ
            ToolStripMenuItem menuCuaSo = new ToolStripMenuItem("Cửa sổ");
            menuCuaSo.DropDownItems.Add("Xếp tầng (Cascade)", null, (s, e) => LayoutMdi(MdiLayout.Cascade));
            menuCuaSo.DropDownItems.Add("Ngang (Tile Horizontal)", null, (s, e) => LayoutMdi(MdiLayout.TileHorizontal));
            menuCuaSo.DropDownItems.Add("Dọc (Tile Vertical)", null, (s, e) => LayoutMdi(MdiLayout.TileVertical));
            menuCuaSo.DropDownItems.Add(new ToolStripSeparator());
            menuCuaSo.DropDownItems.Add("Đóng tất cả", null, (s, e) => {
                foreach (Form child in MdiChildren) child.Close();
            });

            menuStrip1.Items.AddRange(new ToolStripItem[] { menuHeThong, menuQuanLy, menuCuaSo });
            this.MainMenuStrip = menuStrip1;

            // StatusStrip
            statusStrip1 = new StatusStrip();
            lblCanBo = new ToolStripStatusLabel("Cán bộ quản lý: Nguyễn Vũ Nhật Minh | Trạng thái: Đã đăng nhập");
            progressBar1 = new ToolStripProgressBar { Value = 100 };
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblCanBo, progressBar1 });

            this.Controls.Add(statusStrip1);
            this.Controls.Add(menuStrip1);
        }

        private void OpenStudentList_Click(object sender, EventArgs e)
        {
            Form existing = MdiChildren.FirstOrDefault(f => f is frmStudentList);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                frmStudentList frm = new frmStudentList();
                frm.MdiParent = this;
                frm.Show();
            }
        }

        private void OpenCourseList_Click(object sender, EventArgs e)
        {
            Form existing = MdiChildren.FirstOrDefault(f => f is frmCourseList);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                frmCourseList frm = new frmCourseList();
                frm.MdiParent = this;
                frm.Show();
            }
        }
    }
}