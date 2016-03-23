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
    public class ManageStatisticsRepository : CommonRepository, IManageStatisticsRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Statistics> GetSalesStatistics(StatisticsCondition condition, DataTablesModel table)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string orderBy = "TARGET_YEAR, TARGET_MONTH";

                if (table.iSortCol_0.HasValue && table.iSortCol_0.Value > 0)
                    orderBy = this.GetColumnOrderBy(table);

                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT
                        TARGET_YEAR
                        , TARGET_MONTH
                        , SUM(QUANTITY) TOTAL_QUANTITY
                        , SUM(SALES) TOTAL_SALES
                        , SUM(COST) TOTAL_COST
                        , SUM(PROFIT) TOTAL_PROFIT
                    FROM (
                        SELECT
                            tbRetail.PRODUCT_ID
                            , tbRetail.TARGET_YEAR
                            , tbRetail.TARGET_MONTH
                            , tbRetail.QUANTITY
                            , tbRetail.SALES
                            , (tbRetail.QUANTITY * (SELECT REAL_PRICE FROM PRODUCT WHERE PRODUCT_ID = tbRetail.PRODUCT_ID)) COST
                            , (tbRetail.SALES - (tbRetail.QUANTITY * (SELECT REAL_PRICE FROM PRODUCT WHERE PRODUCT_ID = tbRetail.PRODUCT_ID))) PROFIT
                        FROM (
                            SELECT
                                PRODUCT_ID
                                , YEAR(CREATED_DATE) TARGET_YEAR
                                , MONTH(CREATED_DATE) TARGET_MONTH
                                , SUM(QUANTITY) QUANTITY
                                , SUM(TOTAL_PRICE) SALES
                            FROM
                                RETAIL
                            WHERE
                                YEAR(CREATED_DATE) = {0} ", condition.TARGET_YEAR);

                if (!string.IsNullOrEmpty(condition.SEX))
                    sqlQuery.AppendFormat("AND (SELECT SEX FROM PRODUCT WHERE PRODUCT_ID = RETAIL.PRODUCT_ID) IN ({0}) ", condition.SEX);

                if (!string.IsNullOrEmpty(condition.CATEGORY_ID))
                    sqlQuery.AppendFormat("AND (SELECT CATEGORY_ID FROM PRODUCT WHERE PRODUCT_ID = RETAIL.PRODUCT_ID) IN ({0}) ", condition.CATEGORY_ID);

                if (!string.IsNullOrEmpty(condition.BRAND_ID))
                    sqlQuery.AppendFormat("AND (SELECT BRAND_ID FROM PRODUCT WHERE PRODUCT_ID = RETAIL.PRODUCT_ID) IN ({0}) ", condition.BRAND_ID);

                sqlQuery.AppendFormat(@"
                            GROUP BY PRODUCT_ID, YEAR(CREATED_DATE), MONTH(CREATED_DATE)
                            ) tbRetail
                        ) tbCalculateProfit
                    GROUP BY TARGET_YEAR, TARGET_MONTH
                    ORDER BY {0} {1}", orderBy, table.sSortDir_0);

                sqlConnection.Open();

                var list = sqlConnection.Query<Statistics>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return list;
            }
        }

        public IList<Product> GetSalesStatisticsDetail(StatisticsCondition condition, DataTablesModel table, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.AppendFormat(@"
                    SELECT
                        RETAIL.PRODUCT_ID
                        , (SELECT TOP 1 FILE_PATH FROM PICTURE WHERE PICTURE.PRODUCT_ID = RETAIL.PRODUCT_ID AND PICTURE.DISPLAY_FLAG = '1') AS PICTURE
                        , PRODUCT.PRODUCT_CODE
                        , PRODUCT.PRODUCT_NAME
                        , M_BRAND.BRAND_NAME
                        , M_CATEGORY.CATEGORY_NAME
                        , M_COLOR.COLOR_NAME
                        , SIZE
                        , RETAIL.QUANTITY
                        , RETAIL.TOTAL_PRICE SALES
                        , PRODUCT.REAL_PRICE
                        , RETAIL.MODIFIED_DATE
                    FROM
                        RETAIL 
                        LEFT JOIN PRODUCT
                            ON RETAIL.PRODUCT_ID = PRODUCT.PRODUCT_ID
                        LEFT JOIN M_BRAND
                            ON PRODUCT.BRAND_ID = M_BRAND.BRAND_ID
                        LEFT JOIN M_CATEGORY
                            ON PRODUCT.CATEGORY_ID = M_CATEGORY.CATEGORY_ID
                        LEFT JOIN M_COLOR
                            ON RETAIL.COLOR_ID = M_COLOR.COLOR_ID
                    WHERE
                        YEAR(RETAIL.CREATED_DATE) = {0}
                        AND MONTH(RETAIL.CREATED_DATE) = {1} ", condition.TARGET_YEAR, condition.TARGET_MONTH);

                if (!string.IsNullOrEmpty(condition.SEX))
                    sqlContent.AppendFormat("AND PRODUCT.SEX IN ({0}) ", condition.SEX);

                if (!string.IsNullOrEmpty(condition.CATEGORY_ID))
                    sqlContent.AppendFormat("AND PRODUCT.CATEGORY_ID IN ({0}) ", condition.CATEGORY_ID);

                if (!string.IsNullOrEmpty(condition.BRAND_ID))
                    sqlContent.AppendFormat("AND PRODUCT.BRAND_ID IN ({0}) ", condition.BRAND_ID);

                var sqlQuery = this.BuildSQLPaging(sqlContent, table);
                IList<Product> dataList = new List<Product>();
                totalData = 0;

                sqlConnection.Open();

                dataList = sqlConnection.Query<Product>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "PRODUCT_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return dataList;
            }
        }

        public IList<Product> GetAllSalesStatisticsDetail(StatisticsCondition condition)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT
                        RETAIL.PRODUCT_ID
                        , PRODUCT.PRODUCT_CODE
                        , PRODUCT.PRODUCT_NAME
                        , M_BRAND.BRAND_NAME
                        , M_CATEGORY.CATEGORY_NAME
                        , M_COLOR.COLOR_NAME
                        , SIZE
                        , RETAIL.QUANTITY
                        , RETAIL.TOTAL_PRICE SALES
                        , PRODUCT.REAL_PRICE
                        , RETAIL.MODIFIED_DATE
                    FROM
                        RETAIL 
                        LEFT JOIN PRODUCT
                            ON RETAIL.PRODUCT_ID = PRODUCT.PRODUCT_ID
                        LEFT JOIN M_BRAND
                            ON PRODUCT.BRAND_ID = M_BRAND.BRAND_ID
                        LEFT JOIN M_CATEGORY
                            ON PRODUCT.CATEGORY_ID = M_CATEGORY.CATEGORY_ID
                        LEFT JOIN M_COLOR
                            ON RETAIL.COLOR_ID = M_COLOR.COLOR_ID
                    WHERE
                        YEAR(RETAIL.CREATED_DATE) = {0}
                        AND MONTH(RETAIL.CREATED_DATE) = {1}
                    ORDER BY RETAIL.CREATED_DATE ", condition.TARGET_YEAR, condition.TARGET_MONTH);

                IList<Product> dataList = new List<Product>();
                sqlConnection.Open();

                dataList = sqlConnection.Query<Product>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return dataList;
            }
        }
    }
}