using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace _1150080113_ChauThiNgocQuyen_Lab7_Tuan9
{
    public partial class Form1 : Form
    {
        // ✅ Chuỗi kết nối SQL Server
        private string strCon =
            @"Data Source=DESKTOP-KMRCV44;Initial Catalog=QuanLyBanSach;Persist Security Info=True;User ID=sa;Password=123;TrustServerCertificate=True";

        private SqlConnection sqlCon = null;

        

        // ========================== KẾT NỐI CƠ SỞ DỮ LIỆU ==========================

        private void MoKetNoi()
        {
            if (sqlCon == null)
                sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
        }

        private void DongKetNoi()
        {
            if (sqlCon != null && sqlCon.State == ConnectionState.Open)
                sqlCon.Close();
        }

        // ========================== HIỂN THỊ DỮ LIỆU ==========================

        private void HienThiDanhSachNXB()
        {
            try
            {
                MoKetNoi();
                string sql = "SELECT * FROM NhaXuatBan";
                SqlDataAdapter da = new SqlDataAdapter(sql, sqlCon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvNXB.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                DongKetNoi();
            }
        }

        // ========================== CHỨC NĂNG THÊM ==========================

        private void ThemNXB()
        {
            if (txtMaNXB.Text.Trim() == "" || txtTenNXB.Text.Trim() == "" || txtDiaChi.Text.Trim() == "")
            {
                MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                MoKetNoi();

                string check = "SELECT COUNT(*) FROM NhaXuatBan WHERE MaNXB=@Ma";
                SqlCommand checkCmd = new SqlCommand(check, sqlCon);
                checkCmd.Parameters.AddWithValue("@Ma", txtMaNXB.Text.Trim());
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("❌ Mã NXB đã tồn tại!", "Thông báo");
                    return;
                }

                string sql = "INSERT INTO NhaXuatBan (MaNXB, TenNXB, DiaChi) VALUES (@Ma, @Ten, @DiaChi)";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.Parameters.AddWithValue("@Ma", txtMaNXB.Text.Trim());
                cmd.Parameters.AddWithValue("@Ten", txtTenNXB.Text.Trim());
                cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());
                cmd.ExecuteNonQuery();

                MessageBox.Show("✅ Thêm nhà xuất bản thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                HienThiDanhSachNXB();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message);
            }
            finally
            {
                DongKetNoi();
            }
        }

        // ========================== CHỨC NĂNG CẬP NHẬT ==========================

        private void CapNhatNXB()
        {
            string ma = txtMaNXB.Text.Trim();
            string ten = txtTenNXB.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show("⚠️ Vui lòng chọn NXB cần cập nhật!", "Thông báo");
                return;
            }

            try
            {
                MoKetNoi();

                string sql = "UPDATE NhaXuatBan SET TenNXB=@Ten, DiaChi=@DiaChi WHERE MaNXB=@Ma";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.Parameters.AddWithValue("@Ma", ma);
                cmd.Parameters.AddWithValue("@Ten", ten);
                cmd.Parameters.AddWithValue("@DiaChi", diaChi);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("✅ Cập nhật thành công!", "Thông báo");
                    HienThiDanhSachNXB();
                    XoaTrang();
                }
                else
                {
                    MessageBox.Show("❌ Không tìm thấy NXB để cập nhật.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
            finally
            {
                DongKetNoi();
            }
        }

        // ========================== CHỨC NĂNG XÓA ==========================

        private void XoaNXB()
        {
            string ma = txtMaNXB.Text.Trim();

            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show("⚠️ Vui lòng nhập hoặc chọn Mã NXB cần xóa!", "Thông báo");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa NXB này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    MoKetNoi();
                    string sql = "DELETE FROM NhaXuatBan WHERE MaNXB=@Ma";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.Parameters.AddWithValue("@Ma", ma);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("🗑️ Xóa thành công!", "Thông báo");
                        HienThiDanhSachNXB();
                        XoaTrang();
                    }
                    else
                    {
                        MessageBox.Show("❌ Không tìm thấy NXB để xóa.", "Thông báo");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa: " + ex.Message);
                }
                finally
                {
                    DongKetNoi();
                }
            }
        }

        // ========================== HỖ TRỢ ==========================

        private void XoaTrang()
        {
            txtMaNXB.Clear();
            txtTenNXB.Clear();
            txtDiaChi.Clear();
        }

        private void dgvNXB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNXB.Rows[e.RowIndex];
                txtMaNXB.Text = row.Cells["MaNXB"].Value.ToString();
                txtTenNXB.Text = row.Cells["TenNXB"].Value.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HienThiDanhSachNXB();
        }

        // ========================== GIAO DIỆN ==========================

        private DataGridView dgvNXB;
        private TextBox txtMaNXB, txtTenNXB, txtDiaChi;
        private Label lblMa, lblTen, lblDiaChi;
        private Button btnThem, btnCapNhat, btnXoa;

        private void InitializeComponent()
        {
            this.dgvNXB = new DataGridView();
            this.txtMaNXB = new TextBox();
            this.txtTenNXB = new TextBox();
            this.txtDiaChi = new TextBox();
            this.lblMa = new Label();
            this.lblTen = new Label();
            this.lblDiaChi = new Label();
            this.btnThem = new Button();
            this.btnCapNhat = new Button();
            this.btnXoa = new Button();
            this.SuspendLayout();

            // ========================= DataGridView =========================
            this.dgvNXB.Location = new Point(30, 60);
            this.dgvNXB.Size = new Size(500, 320);
            this.dgvNXB.ReadOnly = true;
            this.dgvNXB.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvNXB.CellClick += new DataGridViewCellEventHandler(this.dgvNXB_CellClick);

            // ========================= Labels =========================
            this.lblMa.Location = new Point(560, 80);
            this.lblMa.Text = "Mã NXB:";

            this.lblTen.Location = new Point(560, 130);
            this.lblTen.Text = "Tên NXB:";

            this.lblDiaChi.Location = new Point(560, 180);
            this.lblDiaChi.Text = "Địa chỉ:";

            // ========================= TextBoxes =========================
            this.txtMaNXB.Location = new Point(640, 77);
            this.txtMaNXB.Size = new Size(180, 22);

            this.txtTenNXB.Location = new Point(640, 127);
            this.txtTenNXB.Size = new Size(180, 22);

            this.txtDiaChi.Location = new Point(640, 177);
            this.txtDiaChi.Size = new Size(180, 22);

            // ========================= Buttons =========================
            this.btnThem.BackColor = Color.DarkGray;
            this.btnThem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnThem.Location = new Point(640, 230);
            this.btnThem.Size = new Size(180, 40);
            this.btnThem.Text = "Thêm";
            this.btnThem.Click += new EventHandler((s, e) => ThemNXB());

            this.btnCapNhat.BackColor = Color.DarkGray;
            this.btnCapNhat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnCapNhat.Location = new Point(640, 290);
            this.btnCapNhat.Size = new Size(180, 40);
            this.btnCapNhat.Text = "Cập nhật";
            this.btnCapNhat.Click += new EventHandler((s, e) => CapNhatNXB());

            this.btnXoa.BackColor = Color.DarkGray;
            this.btnXoa.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnXoa.Location = new Point(640, 350);
            this.btnXoa.Size = new Size(180, 40);
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Click += new EventHandler((s, e) => XoaNXB());

            // ========================= Form =========================
            this.ClientSize = new Size(860, 450);
            this.Controls.AddRange(new Control[]
            {
                dgvNXB, txtMaNXB, txtTenNXB, txtDiaChi,
                lblMa, lblTen, lblDiaChi,
                btnThem, btnCapNhat, btnXoa
            });
            this.Text = "QUẢN LÝ NHÀ XUẤT BẢN - Châu Thị Ngọc Quyên - 1150080113";
            this.Load += new EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }
    }
}
