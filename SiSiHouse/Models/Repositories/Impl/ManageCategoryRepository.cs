using Dapper;
using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SiSiHouse.Models.Repositories.Impl
{
    public class ManageCategoryRepository : CommonRepository, IManageCategoryRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Category> GetCategoryList(CategoryCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT CATEGORY_ID
                        , (SELECT CATEGORY_NAME FROM M_CATEGORY WHERE M_CATEGORY.CATEGORY_ID = tbMain.PARENT_CATEGORY_ID) PARENT_CATEGORY_NAME
                        , CATEGORY_NAME
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = tbMain.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_CATEGORY tbMain
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.CATEGORY_NAME))
                    sqlContent.AppendFormat(" AND CATEGORY_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.CATEGORY_NAME) + "%");

                if (condition.PARENT_CATEGORY_ID.HasValue)
                    sqlContent.AppendFormat(" AND PARENT_CATEGORY_ID = {0}", condition.PARENT_CATEGORY_ID);

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (0 == tableSetting.iSortCol_0)
                    tableSetting.sSortDir_0 = "desc";

                var sqlQuery = this.BuildSQLPaging(sqlContent, tableSetting);
                IList<Category> categoryList = new List<Category>();
                totalData = 0;


                sqlConnection.Open();

                categoryList = sqlConnection.Query<Category>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "CATEGORY_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return categoryList;
            }
        }

        public Category GetCategoryInfo(int categoryID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT CATEGORY_ID
                        , CATEGORY_NAME
                        , PARENT_CATEGORY_ID
                        , DELETE_FLAG
                        , CREATED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_CATEGORY.CREATED_USER_ID) CREATED_USER
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_CATEGORY.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_CATEGORY
                    WHERE CATEGORY_ID = {0}", categoryID);

                Category categoryInfo = new Category();


                sqlConnection.Open();

                categoryInfo = sqlConnection.Query<Category>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return categoryInfo;
            }
        }

        public int UpdateCategoryInfo(Category data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                if (data.CATEGORY_ID == 0)
                {
                    sqlQuery.AppendFormat(@"
                        INSERT INTO M_CATEGORY
                            (CATEGORY_NAME
                            , PARENT_CATEGORY_ID
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (N'{0}', {1}, '{2}', '{3}', {4}, '{5}', {6})"
                      , data.CATEGORY_NAME
                      , (data.PARENT_CATEGORY_ID.HasValue ? data.PARENT_CATEGORY_ID.Value.ToString() : "null")
                      , Constant.DeleteFlag.NON_DELETE
                      , data.MODIFIED_DATE
                      , data.MODIFIED_USER_ID
                      , data.MODIFIED_DATE
                      , data.MODIFIED_USER_ID);
                }
                else
                {
                    sqlQuery.AppendFormat(@"
                        UPDATE M_CATEGORY
                            SET CATEGORY_NAME = N'{0}'
                            , PARENT_CATEGORY_ID = {1}
                            , DELETE_FLAG = '{2}'
                            , MODIFIED_DATE = '{3}'
                            , MODIFIED_USER_ID = {4}
                        WHERE CATEGORY_ID = {5}"
                     , data.CATEGORY_NAME
                     , (data.PARENT_CATEGORY_ID.HasValue ? data.PARENT_CATEGORY_ID.Value.ToString() : "null")
                     , data.DELETE_FLAG
                     , data.MODIFIED_DATE
                     , data.MODIFIED_USER_ID
                     , data.CATEGORY_ID);
                }

                int result = 0;

                sqlConnection.Open();

                result = sqlConnection.Execute(sqlQuery.ToString());

                sqlConnection.Dispose();
                sqlConnection.Close();

                return result;
            }
        }
    }
}