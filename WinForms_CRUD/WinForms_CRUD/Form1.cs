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
        private DataTable table;

        public Form1()
        {
            InitializeComponent();

            ResetFields();
            ResetErrorFields();

            table = new DataTable();
            table.Columns.Add("제품명", typeof(string));
            table.Columns.Add("수량", typeof(Int32));
            table.Columns.Add("단가", typeof(Decimal));
            table.Columns.Add("금액", typeof(Decimal));
        }

        private void ResetFields()
        {
            txtProductName.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
        }

        private void ResetErrorFields()
        { 
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

        private bool IsDuplicate(string columnName, string item)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[columnName].ToString().Equals(item, StringComparison.OrdinalIgnoreCase))
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
            if (IsDuplicate("제품명", txtProductName.Text))
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
    }
}
