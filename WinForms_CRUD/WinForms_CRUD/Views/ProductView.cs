using System;
using System.Data;
using System.Windows.Forms;

namespace WinForms_CRUD
{
    // 뷰 인터페이스
    public interface IProductView
    {
        string ProductName { get; set; }
        string Quantity { get; set; }
        string UnitPrice { get; set; }
        string SelectedProduct { get; }
        string SearchProductName { get; } 
        void DisplayProducts(DataTable products);
        void DisplaySearchResults(DataRow[] foundRows);
        void ShowError(string message);
        void ShowError(string cell, string message);
    }

    // 뷰
    public partial class ProductView : Form, IProductView
    {
        private ProductPresenter presenter;

        public ProductView()
        {
            InitializeComponent();
            InitializeUI();
            presenter = new ProductPresenter(this);
        }

        public string ProductName
        {
            get => txtProductName.Text;
            set => txtProductName.Text = value;
        }

        public string Quantity
        {
            get => txtQuantity.Text;
            set => txtQuantity.Text = value;
        }

        public string UnitPrice
        {
            get => txtUnitPrice.Text;
            set => txtUnitPrice.Text = value;
        }

        public string SelectedProduct => dgvProduct.SelectedRows.Count > 0 ? dgvProduct.SelectedRows[0].Cells["제품명"].Value.ToString() : null;
        public string SearchProductName => txtProductSearch.Text;

        private void InitializeUI()
        {
            ResetFields();
            ResetErrorFields();

            dgvProduct.ReadOnly = true; // 그리드 읽기 전용 설정
            dgvProduct.AllowUserToAddRows = false; // 비어 있는 행 추가 방지
            dgvProduct.MultiSelect = false; // 여러 행 선택 비활성화
        }

        public void DisplayProducts(DataTable products)
        {
            dgvProduct.DataSource = products;
        }

        public void DisplaySearchResults(DataRow[] foundRows)
        {
            var searchResults = foundRows.CopyToDataTable();
            dgvProduct.DataSource = searchResults;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowError(string cell, string message)
        {
            if(cell == "제품명 검색")
            {
                lblProductSearchError.Text = message;
            }
            if (cell == "제품명")
            {
                lblProductNameError.Text = message;
            }
            if (cell == "수량")
            {
                lblQuantityError.Text = message;
            }
            if (cell == "단가")
            {
                lblUnitPriceError.Text = message;
            }
        }

        public void ResetFields()
        {
            txtProductName.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtProductSearch.Text = "";
        }

        public void ResetErrorFields()
        {
            lblProductSearchError.Text = "";
            lblProductNameError.Text = "";
            lblQuantityError.Text = "";
            lblUnitPriceError.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ResetErrorFields();
            presenter.AddProduct();
            ResetFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ResetErrorFields();
            presenter.UpdateProduct();
            ResetFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ResetErrorFields();
            presenter.DeleteProduct();
            ResetFields();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ResetErrorFields();
            presenter.SearchProduct();
            ResetFields();
        }

        private void dgvProduct_SelectionChanged(object sender, EventArgs e)
        {
            ResetErrorFields();
            if (dgvProduct.SelectedRows.Count > 0)
            {
                var selectedRow = dgvProduct.SelectedRows[0];
                txtProductName.Text = selectedRow.Cells["제품명"].Value.ToString();
                txtQuantity.Text = selectedRow.Cells["수량"].Value.ToString();
                txtUnitPrice.Text = selectedRow.Cells["단가"].Value.ToString();
            }
        }
    }
}
