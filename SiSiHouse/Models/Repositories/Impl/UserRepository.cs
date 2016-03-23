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

                sqlQuery.Append(@"
                    SELECT USER_ID
                        , ACCOUNT
                        , FULL_NAME
                        , ROLE_ID
                    FROM M_USER
                    WHERE ACCOUNT = @ACCOUNT
                    AND PASSWORD = @PASSWORD
                    AND DELETE_FLAG = @DELETE_FLAG");

                User user = null;

                sqlConnection.Open();

                user = sqlConnection.Query<User>(
                    sqlQuery.ToString(),
                    new
                    {
                        ACCOUNT = account,
                        PASSWORD = password,
                        DELETE_FLAG = Constant.DeleteFlag.NON_DELETE
                    }
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

                sqlQuery.Append(@"
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
                    WHERE USER_ID = @USER_ID");

                User userInfo = new User();

                sqlConnection.Open();

                userInfo = sqlConnection.Query<User>(
                    sqlQuery.ToString(),
                    new
                    {
                        USER_ID = userID
                    }
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
                    WHERE ACCOUNT = @ACCOUNT
                        AND USER_ID <> @USER_ID", account, userID);

                int countUser = 0;

                sqlConnection.Open();

                countUser = sqlConnection.Query<int>(
                    sqlQuery.ToString(),
                    new
                    {
                        ACCOUNT = account,
                        USER_ID = userID
                    }
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
                    sqlQuery.Append(@"
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
                            (@ACCOUNT, @PASSWORD, @FULL_NAME, @ROLE_ID, @MOBILE, @EMAIL, @PRIVATE_PAGE, @ADDRESS, @DELETE_FLAG
                            , @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");
                }
                else
                {
                    sqlQuery.Append(@"
                        UPDATE M_USER
                            SET ACCOUNT = @ACCOUNT
                            , PASSWORD = @PASSWORD
                            , FULL_NAME = @FULL_NAME
                            , ROLE_ID = @ROLE_ID
                            , MOBILE = @MOBILE
                            , EMAIL = @EMAIL
                            , PRIVATE_PAGE = @PRIVATE_PAGE
                            , ADDRESS = @ADDRESS
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                        WHERE USER_ID = @USER_ID");
                }

                int result = 0;

                sqlConnection.Open();

                result = sqlConnection.Execute(
                    sqlQuery.ToString(),
                    new
                    {
                        USER_ID = data.USER_ID,
                        ACCOUNT = data.ACCOUNT,
                        PASSWORD = data.PASSWORD,
                        FULL_NAME = data.FULL_NAME,
                        ROLE_ID = data.ROLE_ID,
                        MOBILE = data.MOBILE,
                        EMAIL = data.EMAIL,
                        PRIVATE_PAGE = data.PRIVATE_PAGE,
                        ADDRESS = data.ADDRESS,
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