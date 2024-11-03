using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;  // 정규 표현식을 사용하기 위한 클래스

namespace WinForms_CRUD
{
    public partial class Form1 : Form
    {
        private DataTable table; // 원본 데이터 테이블
        private DataTable filteredTable; // 필터링된 데이터 테이블
        private bool isFiltered; // 필터링 여부를 나타내는 플래그

        public Form1()
        {
            InitializeComponent();

            isFiltered = false;

            ResetFields();
            ResetErrorFields();

            table = new DataTable();
            table.Columns.Add("제품명", typeof(string));
            table.Columns.Add("수량", typeof(Int32));
            table.Columns.Add("단가", typeof(Decimal));
            table.Columns.Add("금액", typeof(Decimal));

            dgvProduct.ReadOnly = true; // 그리드 읽기 전용 설정
            dgvProduct.AllowUserToAddRows = false; // 비어 있는 행 추가 방지
            dgvProduct.MultiSelect = false; // 여러 행 선택 비활성화
        }

        private void ResetFields()
        {
            txtProductSearch.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
        }

        private void ResetErrorFields()
        {
            lblProductSearchError.Text = "";
            lblProductNameError.Text = "";
            lblQuantityError.Text = "";
            lblUnitPriceError.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ResetErrorFields();

            // 입력이 유효한지 확인
            if (!IsInputValid())
            {
                return;
            }

            table.Rows.Add(txtProductName.Text, txtQuantity.Text, txtUnitPrice.Text, 
                Int32.Parse(txtQuantity.Text) * Int32.Parse(txtUnitPrice.Text));

            // DataGridView에 DataTable 바인딩
            dgvProduct.DataSource = table;

            ResetFields();
        }

        private bool HasData(string keyColumn, string keyColumnItem)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[keyColumn].ToString().Equals(keyColumnItem, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsFormatValid(string inputText, string pattern)
        {
            if (!Regex.IsMatch(inputText, pattern))
            {
                return false;
            }

            return true;
        }

        private bool IsInputValid()
        {
            // 유효성을 추적할 변수
            bool isValid = true;

            // 중복 확인
            if (HasData("제품명", txtProductName.Text))
            {
                lblProductNameError.Text = "이미 존재하는 제품명입니다.";
                isValid = false;
            }

            // 제품명 형식 확인
            if (!IsFormatValid(txtProductName.Text, @"^.+$"))
            {
                lblProductNameError.Text = "제품명을 입력해 주세요.";
                isValid = false;
            }

            // 수량 형식 확인
            if (!IsFormatValid(txtQuantity.Text, @"^.+$"))
            {
                lblQuantityError.Text = "값을 입력해 주세요.";
                isValid = false;
            } 
            else if (!IsFormatValid(txtQuantity.Text, @"^\d+$"))
            {
                lblQuantityError.Text = "숫자만 입력해 주세요.";
                isValid = false;
            }

            // 단가 형식 확인
            if (!IsFormatValid(txtUnitPrice.Text, @"^.+$"))
            {
                lblUnitPriceError.Text = "값을 입력해 주세요.";
                isValid = false;
            }
            else if (!IsFormatValid(txtUnitPrice.Text, @"^\d+$"))
            {
                lblUnitPriceError.Text = "숫자만 입력해 주세요.";
                isValid = false;
            }

            return isValid;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ResetErrorFields();

            filteredTable = table.Clone();
            DataRow newRow = filteredTable.NewRow();

            // 전체조회
            if (!IsFormatValid(txtProductSearch.Text, @"^.+$"))
            {
                dgvProduct.DataSource = table;
                isFiltered = false;

                ResetFields();
                return;
            }

            // 데이터 조회
            if (!HasData("제품명", txtProductSearch.Text))
            {
                lblProductSearchError.Text = "존재하지 않는 제품입니다.";
                ResetFields();
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (row["제품명"].ToString() == txtProductSearch.Text)
                {
                    string productName = row["제품명"].ToString();
                    Int32 quantity = Convert.ToInt32(row["수량"]);
                    Int32 unitPrice = Convert.ToInt32(row["단가"]);
                    Int32 price = Convert.ToInt32(row["금액"]);

                    newRow["제품명"] = productName;
                    newRow["수량"] = quantity;
                    newRow["단가"] = unitPrice;
                    newRow["금액"] = price;

                    filteredTable.Rows.Add(newRow);
                    dgvProduct.DataSource = filteredTable;
                    isFiltered = true;

                    ResetFields();
                    return;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ResetErrorFields();

            if (dgvProduct.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvProduct.SelectedRows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;

                if (!isFiltered)
                {
                    if (rowView != null)
                    {
                        DataRow dataRow = rowView.Row;
                        dataRow.Delete();
                    }
                    dgvProduct.DataSource = table;
                }
                else
                {
                    if (rowView != null)
                    {
                        DataRow dataRow = rowView.Row;

                        DeleteRowByColumnValue("제품명", dataRow["제품명"].ToString());
                        dataRow.Delete();
                    }
                    dgvProduct.DataSource = filteredTable;

                }

                ResetFields();
                return;
            }
            MessageBox.Show("삭제할 항목을 선택해주세요.");
            ResetFields();
        }

        private void DeleteRowByColumnValue(string keyColumn, string keyColumnItem)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[keyColumn].ToString().Equals(keyColumnItem, StringComparison.OrdinalIgnoreCase))
                {
                    row.Delete();
                    break;
                }
            }
        }

        private void dgvProduct_SelectionChanged(object sender, EventArgs e)
        {
            ResetFields();

            if (dgvProduct.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvProduct.SelectedRows[0];

                string productName = selectedRow.Cells["제품명"].Value.ToString();
                Int32 quantity = Convert.ToInt32(selectedRow.Cells["수량"].Value);
                Int32 unitPrice = Convert.ToInt32(selectedRow.Cells["단가"].Value);

                txtProductName.Text = productName;
                txtQuantity.Text = quantity.ToString();
                txtUnitPrice.Text = unitPrice.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProduct.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvProduct.SelectedRows[0];

                string productName = selectedRow.Cells["제품명"].Value.ToString();
                string quantity = selectedRow.Cells["수량"].Value.ToString();
                string unitPrice = selectedRow.Cells["단가"].Value.ToString();

                if(productName == txtProductName.Text 
                    && quantity == txtQuantity.Text 
                    && unitPrice == txtUnitPrice.Text)
                {
                    MessageBox.Show("수정할 항목이 없습니다.");
                    ResetFields();
                    return;
                }

                if(productName != txtProductName.Text)
                {
                    UpdateRowByColumnValue("제품명", productName, "제품명", txtProductName.Text);
                }

                if (quantity != txtQuantity.Text)
                {
                    UpdateRowByColumnValue("제품명", productName, "수량", txtQuantity.Text);
                    UpdateRowByColumnValue("제품명", productName, "금액", (Int32.Parse(txtQuantity.Text) * Int32.Parse(txtUnitPrice.Text)).ToString());
                }

                if (unitPrice != txtUnitPrice.Text)
                {
                    UpdateRowByColumnValue("제품명", productName, "단가", txtUnitPrice.Text);
                    UpdateRowByColumnValue("제품명", productName, "금액", (Int32.Parse(txtQuantity.Text) * Int32.Parse(txtUnitPrice.Text)).ToString());
                }

                ResetFields();
                return;
            }
            MessageBox.Show("수정할 항목을 선택해주세요.");
            ResetFields();
        }

        private void UpdateRowByColumnValue(string keyColumn, string keyColumnItem, string chageColumn, string chageColumnItem)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[keyColumn].ToString().Equals(keyColumnItem, StringComparison.OrdinalIgnoreCase))
                {
                    row[chageColumn] = chageColumnItem;
                    break;
                }
            }
        }
    }
}
