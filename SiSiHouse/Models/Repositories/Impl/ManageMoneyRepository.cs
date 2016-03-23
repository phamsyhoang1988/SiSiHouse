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

                sqlQuery.Append(@"
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
                    WHERE MONEY_ID = @MONEY_ID");

                Money moneyInfo = new Money();

                sqlConnection.Open();
                moneyInfo = sqlConnection.Query<Money>(
                    sqlQuery.ToString(),
                    new
                    {
                        MONEY_ID = moneyID
                    }
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
                    sqlQuery.Append(@"
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
                            (@MONEY_NAME, @MONEY_SIGN, @APPLIED_DATE, @EXCHANGE_RATE, @WEIGHT_POSTAGE, @WAGE
                            , @DESCRIPTION, @DELETE_FLAG, @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");
                }
                else
                {
                    sqlQuery.Append(@"
                        UPDATE M_MONEY
                            SET MONEY_NAME = @MONEY_NAME
                            , MONEY_SIGN = @MONEY_SIGN
                            , APPLIED_DATE = @APPLIED_DATE
                            , EXCHANGE_RATE = @EXCHANGE_RATE
                            , WEIGHT_POSTAGE = @WEIGHT_POSTAGE
                            , WAGE = @WAGE
                            , DESCRIPTION = @DESCRIPTION
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                        WHERE MONEY_ID = @MONEY_ID");
                }

                int result = 0;

                sqlConnection.Open();
                result = sqlConnection.Execute(
                    sqlQuery.ToString(),
                    new {
                        MONEY_ID = data.MONEY_ID,
                        MONEY_NAME = data.MONEY_NAME,
                        MONEY_SIGN = data.MONEY_SIGN,
                        APPLIED_DATE = data.APPLIED_DATE,
                        EXCHANGE_RATE = data.EXCHANGE_RATE,
                        WEIGHT_POSTAGE = data.WEIGHT_POSTAGE,
                        WAGE = data.WAGE,
                        DESCRIPTION = data.DESCRIPTION,
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