using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp
{
    public class frmWarehouseAnalytics : Form
    {
        private Button btnStart;
        private Button btnCancel;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblPercent;
        private DataGridView dgvInventory;

        private CancellationTokenSource _cts;
        private BindingList<InventoryModel> _inventoryList = new BindingList<InventoryModel>();

        public frmWarehouseAnalytics()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Thống Kê Kho Hàng Async/Await - Bài Tập 4";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnStart = new Button { Text = "Thống Kê Kho", Location = new Point(20, 20), Size = new Size(120, 35) };
            btnCancel = new Button { Text = "Hủy Tác Vụ", Location = new Point(150, 20), Size = new Size(120, 35), Enabled = false };

            progressBar = new ProgressBar { Location = new Point(280, 25), Size = new Size(380, 25) };
            lblPercent = new Label { Text = "0%", Location = new Point(670, 28), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            lblStatus = new Label { Text = "Trạng thái: Sẵn sàng", Location = new Point(20, 65), AutoSize = true };

            dgvInventory = new DataGridView
            {
                Location = new Point(20, 95),
                Size = new Size(745, 340),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _inventoryList
            };

            btnStart.Click += BtnStart_Click;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { btnStart, btnCancel, progressBar, lblPercent, lblStatus, dgvInventory });
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnCancel.Enabled = true;
            progressBar.Value = 0;
            lblPercent.Text = "0%";
            lblStatus.Text = "Trạng thái: Đang xử lý dữ liệu kho hàng...";
            _inventoryList.Clear();

            _cts = new CancellationTokenSource();
            var progress = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
                lblPercent.Text = $"{percent}%";
            });

            try
            {
                var result = await Task.Run(() => ProcessData(_cts.Token, progress));

                foreach (var item in result)
                {
                    _inventoryList.Add(item);
                }

                lblStatus.Text = "Trạng thái: Hoàn tất thống kê kho hàng!";
                MessageBox.Show("Thống kê dữ liệu tồn kho hoàn tất thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Trạng thái: Đã hủy tác vụ!";
                MessageBox.Show("Tác vụ đã bị hủy bởi người dùng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStart.Enabled = true;
                btnCancel.Enabled = false;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private BindingList<InventoryModel> ProcessData(CancellationToken token, IProgress<int> progress)
        {
            var list = new BindingList<InventoryModel>();

            for (int i = 1; i <= 10; i++)
            {
                token.ThrowIfCancellationRequested();

                // Giả lập tiến trình tính toán dữ liệu kho hàng nặng
                Thread.Sleep(400);

                list.Add(new InventoryModel
                {
                    MaSanPham = $"SP00{i}",
                    TenSanPham = $"Mặt hàng kho thứ {i}",
                    SoLuongTon = i * 15,
                    DonGia = i * 150000m
                });

                progress?.Report(i * 10);
            }

            return list;
        }
    }
}