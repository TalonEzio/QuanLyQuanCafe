using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyQuanCafe.Helper;

namespace QuanLyQuanCafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();

        BindingSource accountList = new BindingSource();

        public Account loginAccount;
        public fAdmin()
        {
            InitializeComponent();
            LoadData();
        }

        #region methods

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }
        void LoadData()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            AddFoodBinding();
            AddAccountBinding();
            LoadFoodCategories();
            LoadTable(true);
        }

        private void LoadTable(bool loadStatus = false)
        {
            dtgvTable.DataSource = TableDAO.Instance.LoadTableList();
            if (loadStatus) { LoadTableStatus(); }

        }

        private void LoadTableStatus()
        {
            List<TableStatus> listTableStatus = new List<TableStatus>();
            listTableStatus.Add(new TableStatus(false, "Trống"));
            listTableStatus.Add(new TableStatus(true, "Đã ngồi"));

            cbTableStatus.DataSource = listTableStatus;
            cbTableStatus.SelectedIndex = 0;
            cbTableStatus.DisplayMember = "StatusText";
            cbTableStatus.ValueMember = "Status";
        }

        private void LoadFoodCategories()
        {
            dtgvCategory.DataSource = FoodCategoryDAO.Instance.GetFoodCategories();
        }

        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            numericUpDown1.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.Register(userName, CommonConstants.DefaultPassword, displayName, type))
            {
                MessageBox.Show(@"Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show(@"Thêm tài khoản thất bại");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Vui lòng đừng xóa chính bạn chứ");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }

            LoadAccount();
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        #endregion

        #region events
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDown1.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDown1.Value;

            EditAccount(userName, displayName, type);
        }


        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            ResetPass(userName);
        }


        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }


        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }
        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    FoodCategory cateogory = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = cateogory;

                    int index = -1;
                    int i = 0;
                    foreach (FoodCategory item in cbFoodCategory.Items)
                    {
                        if (item.ID == cateogory.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as FoodCategory).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm thức ăn");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as FoodCategory).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa thức ăn");
            }
        }
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        #endregion              

        private void btnFristBillPage_Click(object sender, EventArgs e)
        {
            txbPageBill.Text = "1";
        }

        private void btnLastBillPage_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            int lastPage = sumRecord / 10;

            if (sumRecord % 10 != 0)
                lastPage++;

            txbPageBill.Text = lastPage.ToString();
        }

        private void txbPageBill_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
        }

        private void btnPrevioursBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);

            if (page > 1)
                page--;

            txbPageBill.Text = page.ToString();
        }

        private void btnNextBillPage_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPageBill.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            if (page < sumRecord)
                page++;

            txbPageBill.Text = page.ToString();
        }

        private void fAdmin_Load(object sender, EventArgs e)
        {
            //uncomment

            //this.USP_GetListBillByDateForReportTableAdapter.Fill(this.QuanLyQuanCafeDataSet2.USP_GetListBillByDateForReport, dtpkFromDate.Value, dtpkToDate.Value);           

            this.rpViewer.RefreshReport();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string foodCategoryName = txtTenDanhMuc.Text;
            if (String.IsNullOrEmpty(foodCategoryName) || foodCategoryName.Length > 100)
            {
                MessageBox.Show(@"Tên danh mục thức ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var result = FoodCategoryDAO.Instance.CreateFoodCategory(foodCategoryName);
            if (result)
            {
                MessageBox.Show(@"Thêm mới thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadFoodCategories();
            }
            else
            {
                MessageBox.Show(@"Thêm mới thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            string foodCategoryName = txtTenDanhMuc.Text;
            int foodCaregoryId = int.Parse(txbCategoryID.Text);
            if (String.IsNullOrEmpty(foodCategoryName) || foodCategoryName.Length > 100)
            {
                MessageBox.Show(@"Tên danh mục thức ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var result = FoodCategoryDAO.Instance.UpdateFoodCategory(foodCategoryName, foodCaregoryId);
            if (result)
            {
                MessageBox.Show(@"Cập nhật thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadFoodCategories();
            }
            else
            {
                MessageBox.Show(@"Cập nhật thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dtgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            DataGridViewRow row = dtgvCategory.Rows[e.RowIndex];

            var foodCategoryId = (int)row.Cells[0].Value;
            var foodCategoryName = row.Cells[1].Value.ToString();

            txbCategoryID.Text = foodCategoryId + "";
            txtTenDanhMuc.Text = foodCategoryName;


        }

        public void ResetInput()
        {
            txbCategoryID.Text = "";
            txtTenDanhMuc.Text = "";
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            string foodCategoryName = txtTenDanhMuc.Text;
            int foodCaregoryId = int.Parse(txbCategoryID.Text);
            if (String.IsNullOrEmpty(foodCategoryName) || foodCategoryName.Length > 100)
            {
                MessageBox.Show(@"Tên danh mục thức ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var result = FoodCategoryDAO.Instance.DeleteFoodCategory(foodCaregoryId);
            if (result)
            {
                MessageBox.Show(@"Xóa thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadFoodCategories();
            }
            else
            {
                MessageBox.Show(@"Xóa thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {

            if (cbTableStatus.SelectedIndex == -1) return;
            var tableName = txbTableName.Text;

            if (String.IsNullOrEmpty(tableName) || tableName.Length > 100)
            {
                MessageBox.Show(@"Tên bàn ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var tableStatus = ((TableStatus)cbTableStatus.SelectedItem).Status;

            var result = TableDAO.Instance.CreateTable(tableName, tableStatus);
            if (result)
            {
                MessageBox.Show(@"Thêm thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadTable();
            }
            else
            {
                MessageBox.Show(@"Thêm thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            DataGridViewRow row = dtgvTable.Rows[e.RowIndex];

            var id = (int)row.Cells[0].Value;
            var name = row.Cells[1].Value.ToString();
            var status = row.Cells[2].Value.ToString();
            cbTableStatus.SelectedIndex = status =="Trống" ? 0 : 1;

            txbTableName.Text = name;
            txbTableId.Text = id + "";
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            if (cbTableStatus.SelectedIndex == -1) return;
            var tableName = txbTableName.Text;

            if (String.IsNullOrEmpty(tableName) || tableName.Length > 100)
            {
                MessageBox.Show(@"Tên bàn ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var tableStatus = ((TableStatus)cbTableStatus.SelectedItem).Status;
            int tableId = int.Parse(txbTableId.Text);
            var result = TableDAO.Instance.UpdateTable(tableName, tableStatus,tableId);
            if (result)
            {
                MessageBox.Show(@"Sửa thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadTable();
            }
            else
            {
                MessageBox.Show(@"Sửa thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            if (cbTableStatus.SelectedIndex == -1) return;
            var tableName = txbTableName.Text;

            if (String.IsNullOrEmpty(tableName) || tableName.Length > 100)
            {
                MessageBox.Show(@"Tên bàn ăn không hợp lệ", @"Lỗi dữ liệu", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            int tableId = int.Parse(txbTableId.Text);
            var result = TableDAO.Instance.DeleteTable(tableId);
            if (result)
            {
                MessageBox.Show(@"Xóa thành công", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetInput();
                LoadTable();
            }
            else
            {
                MessageBox.Show(@"Xóa thất bại", @"Trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}
