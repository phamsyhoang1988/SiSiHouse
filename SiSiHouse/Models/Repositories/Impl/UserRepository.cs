using Dapper;
using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Repositories.Impl
{
    public class UserRepository : CommonRepository, IUserRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public User GetUserByAccount(string account, string password)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT USER_ID
                        , ACCOUNT
                        , FULL_NAME
                        , ROLE_ID
                    FROM M_USER
                    WHERE ACCOUNT = '{0}'
                    AND PASSWORD = '{1}'
                    AND DELETE_FLAG ='0'", account, password);

                User user = null;

                sqlConnection.Open();

                user = sqlConnection.Query<User>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return user;
            }
        }

        private int GetTotalUser(SqlConnection sqlConnection, StringBuilder sqlContent)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT COUNT(USER_ID) FROM (");
            sql.Append(sqlContent.ToString());
            sql.Append(") tbTotalItem");

            return sqlConnection.Query<int>(
                sql.ToString()
            ).FirstOrDefault();
        }

        public IList<User> GetUserList(UserCondition condition, DataTablesModel table, out int totalUser)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT USER_ID
                        , ACCOUNT
                        , FULL_NAME
                        , ROLE_ID
                        , MOBILE
                        , EMAIL
                        , PRIVATE_PAGE
                        , ADDRESS
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = tbMain.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_USER tbMain
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.FULL_NAME))
                    sqlContent.AppendFormat(" AND FULL_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.FULL_NAME) + "%");

                if (!string.IsNullOrEmpty(condition.MOBILE))
                    sqlContent.AppendFormat(" AND MOBILE LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.MOBILE) + "%");

                if (!string.IsNullOrEmpty(condition.EMAIL))
                    sqlContent.AppendFormat(" AND EMAIL LIKE '{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.EMAIL) + "%");

                if (!string.IsNullOrEmpty(condition.PRIVATE_PAGE))
                    sqlContent.AppendFormat(" AND PRIVATE_PAGE LIKE N'{0}' ESCAPE '\\' ", "%" + replaceWildcardCharacters(condition.PRIVATE_PAGE) + "%");

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (condition.IS_SELECT_DIALOG)
                    sqlContent.Append(" AND ROLE_ID = 0");

                if (0 == table.iSortCol_0)
                    table.sSortDir_0 = "desc";

                var sqlQuery = BuildSQLPaging(sqlContent, table);
                IList<User> locationList = new List<User>();
                totalUser = 0;

                sqlConnection.Open();

                locationList = sqlConnection.Query<User>(
                    sqlQuery.ToString()
                ).ToList();

                totalUser = GetTotalUser(sqlConnection, sqlContent);

                sqlConnection.Dispose();
                sqlConnection.Close();

                return locationList;
            }
        }

        public User GetUserInfo(long userID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT USER_ID
                        , ACCOUNT
                        , PASSWORD
                        , PASSWORD OLD_PASSWORD
                        , FULL_NAME
                        , ROLE_ID
                        , MOBILE
                        , EMAIL
                        , PRIVATE_PAGE
                        , ADDRESS
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = tbMain.MODIFIED_USER_ID) MODIFIED_USER
                    FROM M_USER tbMain
                    WHERE USER_ID = {0}", userID);

                User userInfo = new User();

                sqlConnection.Open();

                userInfo = sqlConnection.Query<User>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return userInfo;
            }
        }

        public bool CheckAccountExist(long userID, string account)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT COUNT(USER_ID)
                    FROM M_USER
                    WHERE ACCOUNT = '{0}'
                        AND USER_ID <> {1}", account, userID);

                int countUser = 0;

                sqlConnection.Open();

                countUser = sqlConnection.Query<int>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (countUser > 0);
            }
        }

        public int UpdateUserInfo(User data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                if (data.USER_ID == 0)
                {
                    sqlQuery.AppendFormat(@"
                        INSERT INTO M_USER
                            (ACCOUNT
                            , PASSWORD
                            , FULL_NAME
                            , ROLE_ID
                            , MOBILE
                            , EMAIL
                            , PRIVATE_PAGE
                            , ADDRESS
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID)
                        VALUES
                            ('{0}', '{1}', N'{2}', {3}, '{4}', '{5}', N'{6}', N'{7}', '{8}', '{9}', {10}, '{11}', {12})"
                    , data.ACCOUNT
                    , (string.IsNullOrEmpty(data.ACCOUNT) ? "" : data.PASSWORD)
                    , data.FULL_NAME
                    , data.ROLE_ID
                    , data.MOBILE
                    , data.EMAIL
                    , data.PRIVATE_PAGE
                    , data.ADDRESS
                    , Constant.DeleteFlag.NON_DELETE
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID);
                }
                else
                {
                    sqlQuery.AppendFormat(@"
                        UPDATE M_USER
                            SET ACCOUNT = '{0}'
                            , PASSWORD = '{1}'
                            , FULL_NAME = N'{2}'
                            , ROLE_ID = {3}
                            , MOBILE = '{4}'
                            , EMAIL = '{5}'
                            , PRIVATE_PAGE = N'{6}'
                            , ADDRESS = N'{7}'
                            , DELETE_FLAG = '{8}'
                            , MODIFIED_DATE = '{9}'
                            , MODIFIED_USER_ID = {10}
                        WHERE USER_ID = {11}"
                    , data.ACCOUNT
                    , (string.IsNullOrEmpty(data.ACCOUNT) ? "" : data.PASSWORD)
                    , data.FULL_NAME
                    , data.ROLE_ID
                    , data.MOBILE
                    , data.EMAIL
                    , data.PRIVATE_PAGE
                    , data.ADDRESS
                    , data.DELETE_FLAG
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.USER_ID);
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