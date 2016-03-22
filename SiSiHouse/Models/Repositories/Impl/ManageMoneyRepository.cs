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
    public class ManageMoneyRepository : CommonRepository, IManageMoneyRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Money> GetMoneyList(MoneyCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT MONEY_ID
                        , MONEY_NAME
                        , MONEY_SIGN
                        , APPLIED_DATE
                        , EXCHANGE_RATE
                        , WEIGHT_POSTAGE
                        , WAGE
                        , DESCRIPTION
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_MONEY.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_MONEY
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.MONEY_NAME))
                    sqlContent.AppendFormat(" AND MONEY_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.MONEY_NAME) + "%");

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (0 == tableSetting.iSortCol_0)
                    tableSetting.sSortDir_0 = "desc";

                var sqlQuery = this.BuildSQLPaging(sqlContent, tableSetting);
                IList<Money> moneyList = new List<Money>();
                totalData = 0;


                sqlConnection.Open();

                moneyList = sqlConnection.Query<Money>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "MONEY_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return moneyList;
            }
        }

        public Money GetMoneyInfo(int moneyID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT MONEY_ID
                        , MONEY_NAME
                        , MONEY_SIGN
                        , APPLIED_DATE
                        , EXCHANGE_RATE
                        , WEIGHT_POSTAGE
                        , WAGE
                        , DESCRIPTION
                        , DELETE_FLAG
                        , CREATED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_MONEY.CREATED_USER_ID) CREATED_USER
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = M_MONEY.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_MONEY
                    WHERE MONEY_ID = {0}", moneyID);

                Money moneyInfo = new Money();

                sqlConnection.Open();
                moneyInfo = sqlConnection.Query<Money>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return moneyInfo;
            }
        }

        public int UpdateMoneyInfo(Money data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                if (data.MONEY_ID == 0)
                {
                    sqlQuery.AppendFormat(@"
                        INSERT INTO M_MONEY
                            (MONEY_NAME
                            , MONEY_SIGN
                            , APPLIED_DATE
                            , EXCHANGE_RATE
                            , WEIGHT_POSTAGE
                            , WAGE
                            , DESCRIPTION
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            (N'{0}', N'{1}', '{2}', {3}, {4}, {5}, N'{6}', '{7}', '{8}', {9}, '{10}', {11})"
                    , data.MONEY_NAME
                    , data.MONEY_SIGN
                    , data.APPLIED_DATE
                    , data.EXCHANGE_RATE
                    , data.WEIGHT_POSTAGE
                    , data.WAGE
                    , data.DESCRIPTION
                    , Constant.DeleteFlag.NON_DELETE
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID);
                }
                else
                {
                    sqlQuery.AppendFormat(@"
                        UPDATE M_MONEY
                            SET MONEY_NAME = N'{0}'
                            , MONEY_SIGN = N'{1}'
                            , APPLIED_DATE = '{2}'
                            , EXCHANGE_RATE = {3}
                            , WEIGHT_POSTAGE = {4}
                            , WAGE = {5}
                            , DESCRIPTION = N'{6}'
                            , DELETE_FLAG = '{7}'
                            , MODIFIED_DATE = '{8}'
                            , MODIFIED_USER_ID = {9}
                        WHERE MONEY_ID = {10}"
                    , data.MONEY_NAME
                    , data.MONEY_SIGN
                    , data.APPLIED_DATE
                    , data.EXCHANGE_RATE
                    , data.WEIGHT_POSTAGE
                    , data.WAGE
                    , data.DESCRIPTION
                    , data.DELETE_FLAG
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.MONEY_ID);
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