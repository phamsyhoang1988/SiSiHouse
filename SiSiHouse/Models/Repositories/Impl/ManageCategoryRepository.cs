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
                        , CATEGORY_NAME
                        , TYPE
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = tbMain.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_CATEGORY tbMain
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.CATEGORY_NAME))
                    sqlContent.AppendFormat(" AND CATEGORY_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.CATEGORY_NAME) + "%");

                if (!string.IsNullOrEmpty(condition.TYPE))
                    sqlContent.AppendFormat(" AND TYPE IN ({0})", condition.TYPE);

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

                sqlQuery.Append(@"
                    SELECT CATEGORY_ID
                        , CATEGORY_NAME
                        , TYPE
                        , DELETE_FLAG
                        , CREATED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_CATEGORY.CREATED_USER_ID) CREATED_USER
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_CATEGORY.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_CATEGORY
                    WHERE CATEGORY_ID = @CATEGORY_ID");

                Category categoryInfo = new Category();


                sqlConnection.Open();

                categoryInfo = sqlConnection.Query<Category>(
                    sqlQuery.ToString(),
                    new
                    {
                        CATEGORY_ID = categoryID
                    }
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
                    sqlQuery.Append(@"
                        INSERT INTO M_CATEGORY
                            (CATEGORY_NAME
                            , TYPE
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (@CATEGORY_NAME, @TYPE, @DELETE_FLAG, @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");
                }
                else
                {
                    sqlQuery.Append(@"
                        UPDATE M_CATEGORY
                            SET CATEGORY_NAME = @CATEGORY_NAME
                            , TYPE = @TYPE
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                        WHERE CATEGORY_ID = @CATEGORY_ID");
                }

                int result = 0;

                sqlConnection.Open();

                result = sqlConnection.Execute(sqlQuery.ToString(), new {
                    CATEGORY_NAME = data.CATEGORY_NAME,
                    TYPE = data.TYPE,
                    DELETE_FLAG = string.IsNullOrEmpty(data.DELETE_FLAG) ? Constant.DeleteFlag.NON_DELETE : data.DELETE_FLAG,
                    CREATED_DATE = data.MODIFIED_DATE,
                    CREATED_USER_ID = data.MODIFIED_USER_ID,
                    MODIFIED_DATE = data.MODIFIED_DATE,
                    MODIFIED_USER_ID = data.MODIFIED_USER_ID,
                    CATEGORY_ID = data.CATEGORY_ID
                });

                sqlConnection.Dispose();
                sqlConnection.Close();

                return result;
            }
        }
    }
}