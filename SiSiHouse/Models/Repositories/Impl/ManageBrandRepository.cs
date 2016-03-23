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
    public class ManageBrandRepository : CommonRepository, IManageBrandRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Brand> GetBrandList(BrandCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT BRAND_ID
                        , BRAND_NAME
                        , DESCRIPTION
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_BRAND.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_BRAND
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.BRAND_NAME))
                    sqlContent.AppendFormat(" AND BRAND_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.BRAND_NAME) + "%");

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (0 == tableSetting.iSortCol_0)
                    tableSetting.sSortDir_0 = "desc";

                var sqlQuery = this.BuildSQLPaging(sqlContent, tableSetting);
                IList<Brand> brandList = new List<Brand>();
                totalData = 0;

                sqlConnection.Open();

                brandList = sqlConnection.Query<Brand>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "BRAND_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return brandList;
            }
        }

        public Brand GetBrandInfo(int brandID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT BRAND_ID
                        , BRAND_NAME
                        , DESCRIPTION
                        , DELETE_FLAG
                        , CREATED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_BRAND.CREATED_USER_ID) CREATED_USER
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_BRAND.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_BRAND
                    WHERE BRAND_ID = @BRAND_ID");

                Brand brandInfo = new Brand();

                sqlConnection.Open();

                brandInfo = sqlConnection.Query<Brand>(
                    sqlQuery.ToString(),
                    new
                    {
                        BRAND_ID = brandID
                    }
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return brandInfo;
            }
        }

        public int UpdateBrandInfo(Brand data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                if (data.BRAND_ID == 0)
                {
                    sqlQuery.Append(@"
                        INSERT INTO M_BRAND
                            (BRAND_NAME
                            , DESCRIPTION
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (@BRAND_NAME, @DESCRIPTION, @DELETE_FLAG, @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");
                }
                else
                {
                    sqlQuery.Append(@"
                        UPDATE M_BRAND
                            SET BRAND_NAME = @BRAND_NAME
                            , DESCRIPTION = @DESCRIPTION
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                        WHERE BRAND_ID = @BRAND_ID");
                }

                int result = 0;

                sqlConnection.Open();

                result = sqlConnection.Execute(
                    sqlQuery.ToString(),
                    new
                    {
                        BRAND_ID = data.BRAND_ID,
                        BRAND_NAME = data.BRAND_NAME,
                        DESCRIPTION = data.DESCRIPTION,
                        DELETE_FLAG = string.IsNullOrEmpty(data.DELETE_FLAG) ? Constant.DeleteFlag.NON_DELETE : data.DELETE_FLAG,
                        CREATED_DATE = data.MODIFIED_DATE,
                        CREATED_USER_ID = data.MODIFIED_USER_ID,
                        MODIFIED_DATE = data.MODIFIED_DATE,
                        MODIFIED_USER_ID = data.MODIFIED_USER_ID
                    });

                sqlConnection.Dispose();
                sqlConnection.Close();

                return result;
            }
        }
    }
}