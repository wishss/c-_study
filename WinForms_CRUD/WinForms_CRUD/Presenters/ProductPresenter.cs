using System;
using System.Data;

namespace WinForms_CRUD
{
    class ProductPresenter
    {
        private IProductView view;
        private DataTable table;

        public ProductPresenter(IProductView view)
        {
            this.view = view;
            InitializeDataTable();
        }

        private void InitializeDataTable()
        {
            table = new DataTable();
            table.Columns.Add("제품명", typeof(string));
            table.Columns.Add("수량", typeof(int));
            table.Columns.Add("단가", typeof(decimal));
            table.Columns.Add("금액", typeof(decimal));

            // 기본 키 설정
            table.PrimaryKey = new DataColumn[] { table.Columns["제품명"] };
        }

        public void AddProduct()
        {
            if (HasData("제품명", view.ProductName))
            {
                view.ShowError("제품명", "이미 존재하는 제품명입니다.");
                return;
            }

            if (!IsInputValid())
                return;

            DataRow row = table.NewRow();
            row["제품명"] = view.ProductName;
            row["수량"] = int.Parse(view.Quantity);
            row["단가"] = decimal.Parse(view.UnitPrice);
            row["금액"] = int.Parse(view.Quantity) * decimal.Parse(view.UnitPrice);
            table.Rows.Add(row);

            view.DisplayProducts(table);
        }

        public void UpdateProduct()
        {
            if (view.SelectedProduct == null)
            {
                view.ShowError("수정할 항목을 선택해주세요.");
                return;
            }

            DataRow selectedRow = table.Rows.Find(view.SelectedProduct);
            if (selectedRow != null)
            {
                if (view.SelectedProduct != view.ProductName)
                {
                    if (HasData("제품명", view.ProductName))
                    {
                        view.ShowError("제품명", "이미 존재하는 제품명입니다.");
                        return;
                    }
                }

                if (!IsInputValid())
                    return;

                selectedRow["제품명"] = view.ProductName;
                selectedRow["수량"] = int.Parse(view.Quantity);
                selectedRow["단가"] = decimal.Parse(view.UnitPrice);
                selectedRow["금액"] = int.Parse(view.Quantity) * decimal.Parse(view.UnitPrice);

                view.DisplayProducts(table);
            }
        }

        public void DeleteProduct()
        {
            if (view.SelectedProduct == null)
            {
                view.ShowError("삭제할 항목을 선택해주세요.");
                return;
            }

            DataRow selectedRow = table.Rows.Find(view.SelectedProduct);
            if (selectedRow != null)
            {
                table.Rows.Remove(selectedRow);
                view.DisplayProducts(table);
            }
        }

        public void SearchProduct()
        {
            if (table.Rows.Count == 0)
            {
                view.ShowError("제품명 검색", "재고 내역이 없습니다.");
                return;
            }

            string searchProductName = view.SearchProductName;
            DataRow[] foundRows = table.Select($"제품명 = '{searchProductName}'");
            
            if (foundRows.Length > 0)
            {
                view.DisplaySearchResults(foundRows);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(view.SearchProductName))
                {
                    view.DisplaySearchResults(table.Select());
                    return;
                }
                view.ShowError("제품명 검색", "존재하지 않는 제품입니다.");
            }
        }

        private bool IsInputValid()
        {
            // 유효성을 추적할 변수
            bool isValid = true;

            //제품명 형식 확인
            if (string.IsNullOrWhiteSpace(view.ProductName))
            {
                view.ShowError("제품명", "제품명을 입력해 주세요.");
                isValid = false;
            }

            // 수량 형식 확인
            if (string.IsNullOrWhiteSpace(view.Quantity))
            {
                view.ShowError("수량", "수량을 입력해 주세요.");
                isValid = false;
            }
            else if (!int.TryParse(view.Quantity, out _))
            {
                view.ShowError("수량", "수량은 숫자여야 합니다.");
                isValid = false;
            }

            //단가 형식 확인
            if (string.IsNullOrWhiteSpace(view.UnitPrice))
            {
                view.ShowError("단가", "단가를 입력해 주세요.");
                isValid = false;
            }
            else if (!decimal.TryParse(view.UnitPrice, out _))
            {
                view.ShowError("단가", "단가는 숫자여야 합니다.");
                isValid = false;
            }
            return isValid;
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
    }
}
