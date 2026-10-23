using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmEmployeeManager : Form
    {
        private BindingList<EmployeeModel> empList = new BindingList<EmployeeModel>();
        private BindingSource empSource = new BindingSource();
        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private string currentAvatarPath = "";

        public frmEmployeeManager()
        {
            InitializeComponent();
        }

        private void frmEmployeeManager_Load(object sender, EventArgs e)
        {
            empSource.DataSource = empList;
            dgvEmployees.DataSource = empSource;
            dgvEmployees.SelectionChanged += DgvEmployees_SelectionChanged;

            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, ev) =>
            {
                lblThoiGian.Text = $"| {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            };
            clockTimer.Start();

            UpdateStatus();
        }

        private void btnSelectAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh chân dung nhân viên";
                dialog.Filter = "File ảnh (*.jpg;*.png)|*.jpg;*.png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    currentAvatarPath = dialog.FileName;
                    picAvatar.Image = Image.FromFile(currentAvatarPath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã NV và Họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            empList.Add(new EmployeeModel
            {
                MaNV = txtMaNV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim(),
                LuongCoBan = decimal.TryParse(txtLuongCoBan.Text, out decimal luong) ? luong : 0,
                AvatarPath = currentAvatarPath
            });

            UpdateStatus();
            ClearInputFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow != null && dgvEmployees.CurrentRow.DataBoundItem is EmployeeModel selectedEmp)
            {
                if (MessageBox.Show($"Xóa nhân viên '{selectedEmp.HoTen}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    empList.Remove(selectedEmp);
                    UpdateStatus();
                    ClearInputFields();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtMaNV.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = txtHoTen.Text.Trim().ToLower();
            }

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập Mã NV hoặc Họ tên vào ô để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var found = empList.FirstOrDefault(x => x.MaNV.ToLower().Contains(keyword) || x.HoTen.ToLower().Contains(keyword));
            if (found != null)
            {
                int index = empList.IndexOf(found);
                dgvEmployees.ClearSelection();
                dgvEmployees.Rows[index].Selected = true;
                MessageBox.Show($"Đã tìm thấy nhân viên: {found.HoTen}", "KẾT QUẢ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên thỏa mãn!", "KẾT QUẢ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuSaveCSV_Click(object sender, EventArgs e)
        {
            if (empList.Count == 0)
            {
                MessageBox.Show("Danh sách nhân sự đang trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Lưu danh sách nhân sự";
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = "DanhSachNhanSu.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("MaNV,HoTen,ChucVu,LuongCoBan,AvatarPath");

                        foreach (var emp in empList)
                        {
                            sb.AppendLine($"{emp.MaNV},{emp.HoTen},{emp.ChucVu},{emp.LuongCoBan},{emp.AvatarPath}");
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void menuOpenCSV_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file CSV danh sách nhân sự";
                ofd.Filter = "CSV Files (*.csv)|*.csv";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(ofd.FileName);
                        if (lines.Length <= 1) return;

                        empList.Clear();
                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(lines[i])) continue;
                            string[] parts = lines[i].Split(',');
                            if (parts.Length >= 5)
                            {
                                empList.Add(new EmployeeModel
                                {
                                    MaNV = parts[0].Trim(),
                                    HoTen = parts[1].Trim(),
                                    ChucVu = parts[2].Trim(),
                                    LuongCoBan = decimal.Parse(parts[3].Trim()),
                                    AvatarPath = parts[4].Trim()
                                });
                            }
                        }
                        UpdateStatus();
                        MessageBox.Show("Nạp dữ liệu nhân sự thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi đọc file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void UpdateStatus()
        {
            int totalEmp = empList.Count;
            decimal totalSalary = empList.Sum(x => x.LuongCoBan);

            lblTongNhanVien.Text = $"Tổng số nhân viên: {totalEmp}";
            lblTongQuyLuong.Text = $"| Tổng quỹ lương: {totalSalary:N0} VNĐ";
        }

        private void DgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow != null && dgvEmployees.CurrentRow.DataBoundItem is EmployeeModel selectedEmp)
            {
                txtMaNV.Text = selectedEmp.MaNV;
                txtHoTen.Text = selectedEmp.HoTen;
                txtChucVu.Text = selectedEmp.ChucVu;
                txtLuongCoBan.Text = selectedEmp.LuongCoBan.ToString("G0");
                currentAvatarPath = selectedEmp.AvatarPath;

                if (!string.IsNullOrEmpty(selectedEmp.AvatarPath) && File.Exists(selectedEmp.AvatarPath))
                    picAvatar.Image = Image.FromFile(selectedEmp.AvatarPath);
                else
                    picAvatar.Image = null;
            }
        }

        private void ClearInputFields()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtChucVu.Clear();
            txtLuongCoBan.Clear();
            picAvatar.Image = null;
            currentAvatarPath = "";
        }

        private void menuExit_Click(object sender, EventArgs e) => Application.Exit();
    }
}