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

                sqlQuery.AppendFormat(@"
                    SELECT COLOR_ID
                        , COLOR_NAME
                        , COLOR_CODE
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_COLOR.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_COLOR
                    WHERE COLOR_ID = {0}", colorID);

                Color colorInfo = new Color();

                sqlConnection.Open();

                colorInfo = sqlConnection.Query<Color>(
                    sqlQuery.ToString()
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
                    sqlQuery.AppendFormat(@"
                        INSERT INTO M_COLOR
                            (COLOR_NAME
                            , COLOR_CODE
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (N'{0}', '{1}', '{2}', '{3}', {4}, '{5}', {6})"
                      , data.COLOR_NAME
                      , data.COLOR_CODE
                      , Constant.DeleteFlag.NON_DELETE
                      , data.MODIFIED_DATE
                      , data.MODIFIED_USER_ID
                      , data.MODIFIED_DATE
                      , data.MODIFIED_USER_ID);
                }
                else
                {
                    sqlQuery.AppendFormat(@"
                        UPDATE M_COLOR
                            SET COLOR_NAME = N'{0}'
                            , COLOR_CODE = '{1}'
                            , DELETE_FLAG = '{2}'
                            , MODIFIED_DATE = '{3}'
                            , MODIFIED_USER_ID = {4}
                        WHERE COLOR_ID = {5}"
                     , data.COLOR_NAME
                     , data.COLOR_CODE
                     , data.DELETE_FLAG
                     , data.MODIFIED_DATE
                     , data.MODIFIED_USER_ID
                     , data.COLOR_ID);
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