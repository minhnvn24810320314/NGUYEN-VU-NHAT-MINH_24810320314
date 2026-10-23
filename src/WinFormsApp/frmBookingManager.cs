using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp
{
    public partial class frmBookingManager : Form
    {
        private BindingList<BookingModel> bookingList = new BindingList<BookingModel>();
        private BindingSource bookingSource = new BindingSource();
        private int bookingCounter = 1;

        public frmBookingManager()
        {
            InitializeComponent();
        }

        private void frmBookingManager_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            bookingSource.DataSource = bookingList;
            dgvBookings.DataSource = bookingSource;

            cboRoomType.SelectedIndex = 0;
            GenerateNewBookingId();
            BindControls();
        }

        private void SetupDataGridView()
        {
            dgvBookings.AutoGenerateColumns = false;
            dgvBookings.Columns.Clear();

            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BookingId", HeaderText = "Mã Đặt" });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "Tên Khách" });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdentityCard", HeaderText = "CCCD" });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RoomType", HeaderText = "Loại Phòng" });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CheckInDate", HeaderText = "Ngày Nhận", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NumberOfDays", HeaderText = "Số Đêm" });
            dgvBookings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PricePerNight", HeaderText = "Giá/Đêm", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
        }

        private void BindControls()
        {
            txtBookingId.DataBindings.Add("Text", bookingSource, "BookingId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtCustomerName.DataBindings.Add("Text", bookingSource, "CustomerName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtIdentityCard.DataBindings.Add("Text", bookingSource, "IdentityCard", true, DataSourceUpdateMode.OnPropertyChanged);
            cboRoomType.DataBindings.Add("SelectedItem", bookingSource, "RoomType", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpCheckIn.DataBindings.Add("Value", bookingSource, "CheckInDate", true, DataSourceUpdateMode.OnPropertyChanged);
            numDays.DataBindings.Add("Value", bookingSource, "NumberOfDays", true, DataSourceUpdateMode.OnPropertyChanged);
            txtPricePerNight.DataBindings.Add("Text", bookingSource, "PricePerNight", true, DataSourceUpdateMode.OnPropertyChanged, 0, "N0");
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider1.Clear();

            // 1. Tên khách hàng không được để trống
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                errorProvider1.SetError(txtCustomerName, "Tên khách hàng không được để trống!");
                isValid = false;
            }

            // 2. CCCD phải đủ 9 hoặc 12 chữ số
            string cccd = txtIdentityCard.Text.Trim();
            if (!cccd.All(char.IsDigit) || (cccd.Length != 9 && cccd.Length != 12))
            {
                errorProvider1.SetError(txtIdentityCard, "CCCD phải là dãy số gồm đúng 9 hoặc 12 chữ số!");
                isValid = false;
            }

            // 3. Giá phòng phải là số lớn hơn 0
            if (!decimal.TryParse(txtPricePerNight.Text, out decimal price) || price <= 0)
            {
                errorProvider1.SetError(txtPricePerNight, "Giá phòng phải là một số lớn hơn 0!");
                isValid = false;
            }

            return isValid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var newBooking = new BookingModel
            {
                BookingId = txtBookingId.Text,
                CustomerName = txtCustomerName.Text.Trim(),
                IdentityCard = txtIdentityCard.Text.Trim(),
                RoomType = cboRoomType.SelectedItem.ToString(),
                CheckInDate = dtpCheckIn.Value,
                NumberOfDays = (int)numDays.Value,
                PricePerNight = decimal.Parse(txtPricePerNight.Text)
            };

            bookingList.Add(newBooking);
            bookingSource.MoveLast();
            bookingCounter++;
            GenerateNewBookingId();
            MessageBox.Show("Thêm đặt phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow == null) return;
            if (!ValidateInput()) return;

            bookingSource.EndEdit();
            dgvBookings.Refresh();
            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBookings.CurrentRow != null && dgvBookings.CurrentRow.DataBoundItem is BookingModel selected)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn đặt phòng '{selected.BookingId}'?",
                    "Cảnh báo xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    bookingList.Remove(selected);
                    ClearInputs();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            errorProvider1.Clear();
            GenerateNewBookingId();
            txtCustomerName.Clear();
            txtIdentityCard.Clear();
            cboRoomType.SelectedIndex = 0;
            dtpCheckIn.Value = DateTime.Now;
            numDays.Value = 1;
            txtPricePerNight.Clear();
        }

        private void GenerateNewBookingId()
        {
            txtBookingId.Text = $"BK{bookingCounter:D3}";
        }
    }
}