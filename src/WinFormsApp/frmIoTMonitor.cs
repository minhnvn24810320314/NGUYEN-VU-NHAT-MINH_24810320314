using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp
{
    public class frmIoTMonitor : Form
    {
        private Button btnStart;
        private Button btnStop;
        private Label lblNhietDoVal;
        private Label lblDoAmVal;
        private Label lblNhietDoTxt;
        private Label lblDoAmTxt;
        private GroupBox gbRealtime;
        private DataGridView dgvLogs;

        private CancellationTokenSource _cts;
        private BindingList<IoTDataModel> _logList = new BindingList<IoTDataModel>();

        public frmIoTMonitor()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Giám Sát IoT Real-time - Bài Tập 5";
            this.Size = new Size(820, 560);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnStart = new Button { Text = "Bắt đầu Giám sát", Location = new Point(20, 20), Size = new Size(140, 35) };
            btnStop = new Button { Text = "Dừng Giám sát", Location = new Point(170, 20), Size = new Size(140, 35), Enabled = false };

            gbRealtime = new GroupBox { Text = "Thông Số Cảm Biến Real-Time", Location = new Point(20, 65), Size = new Size(760, 110) };

            // Đẩy tọa độ X của nhãn Nhiệt độ ra vị trí 200px để không bị che đè
            lblNhietDoTxt = new Label { Text = "Nhiệt độ phòng:", Location = new Point(30, 40), AutoSize = true, Font = new Font("Segoe UI", 11F) };
            lblNhietDoVal = new Label { Text = "0.0 °C", Location = new Point(200, 32), AutoSize = true, Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = Color.Blue };

            lblDoAmTxt = new Label { Text = "Độ ẩm không khí:", Location = new Point(440, 40), AutoSize = true, Font = new Font("Segoe UI", 11F) };
            lblDoAmVal = new Label { Text = "0.0 %", Location = new Point(600, 32), AutoSize = true, Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = Color.Green };

            gbRealtime.Controls.AddRange(new Control[] { lblNhietDoTxt, lblNhietDoVal, lblDoAmTxt, lblDoAmVal });

            dgvLogs = new DataGridView
            {
                Location = new Point(20, 190),
                Size = new Size(760, 310),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = _logList
            };

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;

            this.Controls.AddRange(new Control[] { btnStart, btnStop, gbRealtime, dgvLogs });
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            _cts = new CancellationTokenSource();

            // Chạy luồng ngầm nhận dữ liệu IoT
            Task.Run(() => SimulateIoTSensor(_cts.Token));
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void SimulateIoTSensor(CancellationToken token)
        {
            Random rand = new Random();

            while (!token.IsCancellationRequested)
            {
                // Sinh dữ liệu cảm biến ngẫu nhiên
                double temp = Math.Round(30.0 + rand.NextDouble() * 12.0, 1);
                double humidity = Math.Round(50.0 + rand.NextDouble() * 30.0, 1);
                string status = temp > 37.0 ? "CẢNH BÁO QUÁ NHIỆT" : "Bình thường";

                // Lấy chính xác thời gian thực tại thời điểm phát sinh dữ liệu
                string realTimeNow = DateTime.Now.ToString("HH:mm:ss");

                this.Invoke(new Action(() =>
                {
                    lblNhietDoVal.Text = $"{temp} °C";
                    lblDoAmVal.Text = $"{humidity} %";

                    if (temp > 37.0)
                        lblNhietDoVal.ForeColor = Color.Red;
                    else
                        lblNhietDoVal.ForeColor = Color.Blue;

                    _logList.Insert(0, new IoTDataModel
                    {
                        ThoiGian = realTimeNow,
                        NhietDo = temp,
                        DoAm = humidity,
                        TrangThai = status
                    });
                }));

                Thread.Sleep(1500); // Cập nhật dữ liệu mỗi 1.5 giây
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Cancel();
            base.OnFormClosing(e);
        }
    }
}