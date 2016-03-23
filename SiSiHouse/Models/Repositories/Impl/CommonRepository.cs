using Dapper;
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
    public class CommonRepository: ICommonRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        protected static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected StringBuilder BuildSQLPaging(StringBuilder sqlContent, DataTablesModel table)
        {
            var sql = new StringBuilder();
            string orderBy = this.GetColumnOrderBy(table);

            sql.Append("SELECT * FROM ( ");
            sql.AppendFormat("SELECT ROW_NUMBER() OVER (ORDER BY {0} {1}) peta_rn, tbData.* ", orderBy, table.sSortDir_0);
            sql.Append("FROM ( ");
            sql.Append(sqlContent.ToString());
            sql.Append(") tbData ) tbPaging ");
            sql.AppendFormat("WHERE peta_rn > {0} AND peta_rn <= {1} ", table.iDisplayStart, table.iDisplayLength + table.iDisplayStart);

            return sql;
        }

        protected string GetColumnOrderBy(DataTablesModel table)
        {
            string orderBy = string.Empty;

            if (table.iSortCol_0.HasValue && !string.IsNullOrEmpty(table.sColumns))
            {
                string[] sCol = table.sColumns.Split(',');

                if (table.iSortCol_0 < sCol.Length && !string.IsNullOrEmpty(sCol[table.iSortCol_0.Value]))
                    orderBy = sCol[table.iSortCol_0.Value];
            }

            return orderBy;
        }

        protected int GetTotalData(SqlConnection sqlConnection, StringBuilder sqlContent, string countItem)
        {
            var sql = new StringBuilder();

            sql.AppendFormat("SELECT COUNT({0}) FROM (", countItem);
            sql.Append(sqlContent.ToString());
            sql.Append(") tbTotalItem");

            return sqlConnection.Query<int>(
                sql.ToString()
            ).FirstOrDefault();
        }

        protected string replaceWildcardCharacters(string value)
        {
            return value.Replace("\\", "\\\\").Replace("[", "\\[").Replace("_", "\\_").Replace("%", "\\%").Replace("'", "''").Replace("@", "@\\");
        }

        public IList<Brand> GetBrandList()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT BRAND_ID
                        , BRAND_NAME
                    FROM M_BRAND
                    WHERE DELETE_FLAG = '0'
                    ORDER BY BRAND_NAME");

                IList<Brand> brandList = new List<Brand>();

                sqlConnection.Open();

                brandList = sqlConnection.Query<Brand>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return brandList;
            }
        }

        public IList<Category> GetCategoryList()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT CATEGORY_ID
                        , CATEGORY_NAME
                    FROM M_CATEGORY
                    WHERE DELETE_FLAG = '0'
                    ORDER BY CATEGORY_NAME");

                IList<Category> categoryList = new List<Category>();

                sqlConnection.Open();

                categoryList = sqlConnection.Query<Category>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return categoryList;
            }
        }

        public IList<Money> GetMoneyList()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT MONEY_ID
                        , MONEY_NAME
                        , MONEY_SIGN
                        , EXCHANGE_RATE
                        , WEIGHT_POSTAGE
                        , WAGE
                    FROM M_MONEY
                    WHERE DELETE_FLAG = '0'
                    ORDER BY MONEY_NAME");

                IList<Money> moneyList = new List<Money>();

                sqlConnection.Open();

                moneyList = sqlConnection.Query<Money>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return moneyList;
            }
        }

        public IList<Color> GetColorList()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.Append(@"
                    SELECT COLOR_ID
                        , COLOR_NAME
                        , COLOR_CODE
                    FROM M_COLOR
                    WHERE DELETE_FLAG = '0'
                    ORDER BY COLOR_NAME");

                IList<Color> colorList = new List<Color>();

                sqlConnection.Open();

                colorList = sqlConnection.Query<Color>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return colorList;
            }
        }

        public IList<Color> GetColorListByProduct(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT
                        tbColorID.COLOR_ID
                        , M_COLOR.COLOR_NAME
                    FROM (
                        SELECT COLOR_ID
                        FROM PRODUCT_DETAIL
                        WHERE PRODUCT_ID = {0}
                            AND QUANTITY > 0
                        GROUP BY COLOR_ID
                        ) tbColorID
                        INNER JOIN M_COLOR
                            ON tbColorID.COLOR_ID = M_COLOR.COLOR_ID", productID);

                IList<Color> colorList = new List<Color>();

                sqlConnection.Open();

                colorList = sqlConnection.Query<Color>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return colorList;
            }
        }

        public IList<ProductDetail> GetSizeByColor(long productID, int colorID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT SIZE
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = {0}
                        AND COLOR_ID = {1}
                        AND QUANTITY > 0", productID, colorID);

                IList<ProductDetail> sizeList = new List<ProductDetail>();

                sqlConnection.Open();

                sizeList = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return sizeList;
            }
        }

        public ProductDetail GetDetailInStock(long productID, int colorID, string size)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT PRODUCT_DETAIL_ID, QUANTITY
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = {0}
                        AND COLOR_ID = {1}
                        AND SIZE = '{2}'", productID, colorID, size);

                sqlConnection.Open();

                var detail = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return detail;
            }
        }

        public IList<Artwork> GetArtworkList(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT ARTWORK_ID
                        , FILE_PATH
                        , FILE_PATH FILE_PATH_OLD
                    FROM ARTWORK
                    WHERE PRODUCT_ID = {0}", productID);

                IList<Artwork> artworkList = new List<Artwork>();

                sqlConnection.Open();

                artworkList = sqlConnection.Query<Artwork>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return artworkList;
            }
        }
    }
}