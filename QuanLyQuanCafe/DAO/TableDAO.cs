using QuanLyQuanCafe.DTO;
using QuanLyQuanCafe.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }

        public static int TableWidth = 90;
        public static int TableHeight = 90;

        private TableDAO() { }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTabel @idTable1 , @idTabel2", new object[] { id1, id2 });
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = LoadTableData();

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public DataTable LoadTableData()
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");
            return data;
        }

        public bool CreateTable(string tableName, bool tableStatus)
        {
            var query = $"INSERT INTO dbo.TableFood values(N'{tableName}',N'{(tableStatus ? "Đã ngồi" : "Trống")}')";
            var result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateTable(string tableName, bool tableStatus, int tableId)
        {
            var query = $"update TableFood set name =N'{tableName}', status=N'{(tableStatus ? "Đã ngồi" : "Trống")}' where id ={tableId}";
            var result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteTable(int tableId)
        {
            var query = $"Delete from TableFood where id ={tableId}";
            var result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }
}
