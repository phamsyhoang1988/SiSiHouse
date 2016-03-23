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
    public class ManageColorRepository : CommonRepository, IManageColorRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Color> GetColorList(ColorCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT COLOR_ID
                        , COLOR_NAME
                        , COLOR_CODE
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_COLOR.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_COLOR
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.COLOR_NAME))
                    sqlContent.AppendFormat(" AND COLOR_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.COLOR_NAME) + "%");

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (0 == tableSetting.iSortCol_0)
                    tableSetting.sSortDir_0 = "desc";

                var sqlQuery = this.BuildSQLPaging(sqlContent, tableSetting);
                IList<Color> colorList = new List<Color>();
                totalData = 0;


                sqlConnection.Open();

                colorList = sqlConnection.Query<Color>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "COLOR_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return colorList;
            }
        }

        public Color GetColorInfo(int colorID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT COLOR_ID
                        , COLOR_NAME
                        , COLOR_CODE
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_COLOR.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_COLOR
                    WHERE COLOR_ID = @COLOR_ID");

                Color colorInfo = new Color();

                sqlConnection.Open();

                colorInfo = sqlConnection.Query<Color>(
                    sqlQuery.ToString(),
                    new
                    {
                        COLOR_ID = colorID
                    }
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return colorInfo;
            }
        }

        public int UpdateColorInfo(Color data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                if (data.COLOR_ID == 0)
                {
                    sqlQuery.Append(@"
                        INSERT INTO M_COLOR
                            (COLOR_NAME
                            , COLOR_CODE
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (@COLOR_NAME, @COLOR_CODE, @DELETE_FLAG, @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");
                }
                else
                {
                    sqlQuery.Append(@"
                        UPDATE M_COLOR
                            SET COLOR_NAME = @COLOR_NAME
                            , COLOR_CODE = @COLOR_CODE
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                        WHERE COLOR_ID = @COLOR_ID");
                }

                int result = 0;

                sqlConnection.Open();

                result = sqlConnection.Execute(
                    sqlQuery.ToString(),
                    new
                    {
                        COLOR_ID = data.COLOR_ID,
                        COLOR_NAME = data.COLOR_NAME,
                        COLOR_CODE = data.COLOR_CODE,
                        DELETE_FLAG = string.IsNullOrEmpty(data.DELETE_FLAG) ? Constant.DeleteFlag.NON_DELETE : data.DELETE_FLAG,
                        CREATED_DATE = data.MODIFIED_DATE,
                        CREATED_USER_ID = data.MODIFIED_USER_ID,
                        MODIFIED_DATE = data.MODIFIED_DATE,
                        MODIFIED_USER_ID = data.MODIFIED_USER_ID
                    }
                );

                sqlConnection.Dispose();
                sqlConnection.Close();

                return result;
            }
        }
    }
}