using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyQuanCafe.DTO;

namespace QuanLyQuanCafe.DAO
{
    public class FoodCategoryDAO
    {
        private static FoodCategoryDAO _instance;

        public static FoodCategoryDAO Instance
        {
            get => _instance ?? (_instance = new FoodCategoryDAO());
            private set => _instance = value;
        }

        private bool ErrorInput(string foodCategoryName)
        {
            return (String.IsNullOrEmpty(foodCategoryName) || foodCategoryName.Length > 100);
        }
        public bool CreateFoodCategory(string foodCategoryName)
        {
            if (!ErrorInput(foodCategoryName))
            {
                var query = $"insert into dbo.FoodCategory values(N'{foodCategoryName}')";

                var result = DataProvider.Instance.ExecuteNonQuery(query);
                return result > 0;
            }

            return false;

        }

        public bool UpdateFoodCategory(string newFoodCategoryName, int foodCategoryId)
        {
            if (!ErrorInput(newFoodCategoryName))
            {
                var query = $"update dbo.FoodCategory set name = N'{newFoodCategoryName}' where id={foodCategoryId}";
                var result = DataProvider.Instance.ExecuteNonQuery(query);

                return result > 0;
            }

            return false;

        }

        public IEnumerable<FoodCategory> GetMapFoodCategories()
        {
            var data = GetFoodCategories();

            foreach (DataRow row in data.Rows)
            {
                yield return new FoodCategory((int)row["id"], row["name"].ToString());
            }
        }

        public DataTable GetFoodCategories()
        {
            var query = "Select * from dbo.FoodCategory";
            var data = DataProvider.Instance.ExecuteQuery(query);
            return data;
        }

        public bool DeleteFoodCategory(int foodCaregoryId)
        {
            var query = $"delete from FoodCategory where id = {foodCaregoryId}";
            var result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }

}
