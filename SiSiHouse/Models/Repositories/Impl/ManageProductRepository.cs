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

namespace SiSiHouse.Models.Repositories.Impl
{
    public class ManageProductRepository : CommonRepository, IManageProductRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public IList<Product> GetProductList(ProductCondition condition, DataTablesModel table, out int totalData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = new StringBuilder();

                sqlContent.Append(@"
                    SELECT PRODUCT_ID
                        , PRODUCT_CODE
                        , PRODUCT_NAME
                        , (SELECT BRAND_NAME FROM M_BRAND WHERE M_BRAND.BRAND_ID = PRODUCT.BRAND_ID) BRAND_NAME
                        , (SELECT CATEGORY_NAME FROM M_CATEGORY WHERE M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID) CATEGORY_NAME
                        , STATUS_ID
                        , IMPORT_PRICE
                        , (SELECT MONEY_SIGN FROM M_MONEY WHERE M_MONEY.MONEY_ID = PRODUCT.MONEY_TYPE_ID) MONEY_SIGN
                        , SALE_PRICE
                        , REAL_PRICE
                        , SALE_OFF_PRICE
                        , CLIP_PATH
                        , ROOT_LINK
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = PRODUCT.MODIFIED_USER_ID) MODIFIED_USER
                        , (SELECT TOP 1 FILE_PATH FROM ARTWORK WHERE PRODUCT_ID = PRODUCT.PRODUCT_ID) AS ARTWORK
                    FROM PRODUCT
                    WHERE 1 = 1");

                if (!string.IsNullOrEmpty(condition.PRODUCT_CODE))
                    sqlContent.AppendFormat(" AND PRODUCT_CODE LIKE '%{0}%'", condition.PRODUCT_CODE);

                if (!string.IsNullOrEmpty(condition.PRODUCT_NAME))
                    sqlContent.AppendFormat(" AND PRODUCT_NAME LIKE N'{0}' ESCAPE '\\' ", "%" + this.replaceWildcardCharacters(condition.PRODUCT_NAME) + "%");

                if (condition.FROM.HasValue)
                    sqlContent.AppendFormat(" AND MODIFIED_DATE >= '{0}'", condition.FROM.Value);

                if (condition.TO.HasValue)
                    sqlContent.AppendFormat(" AND MODIFIED_DATE <= '{0}'", condition.TO.Value.ToString("yyyy/MM/dd") + Constant.MAX_TIME_IN_DAY);

                if (!string.IsNullOrEmpty(condition.SEX))
                    sqlContent.AppendFormat(" AND SEX IN ({0})", condition.SEX);

                if (!string.IsNullOrEmpty(condition.BRAND_ID))
                    sqlContent.AppendFormat(" AND BRAND_ID IN ({0})", condition.BRAND_ID);

                if (!string.IsNullOrEmpty(condition.CATEGORY_ID))
                    sqlContent.AppendFormat(" AND CATEGORY_ID IN ({0})", condition.CATEGORY_ID);

                if (!string.IsNullOrEmpty(condition.STATUS_ID)) {
                    sqlContent.AppendFormat(" AND STATUS_ID IN ({0})", condition.STATUS_ID);
                }
                else if (condition.IS_SELECT_DIALOG)
                {
                    // not in status is "cancel" & "out of stock"
                    sqlContent.Append(" AND STATUS_ID NOT IN (1, 4)");
                }

                if (!condition.DELETE_FLAG)
                    sqlContent.Append(" AND DELETE_FLAG = '0'");

                if (0 == table.iSortCol_0)
                    table.sSortDir_0 = "desc";

                var sqlQuery = this.BuildSQLPaging(sqlContent, table);
                IList<Product> productList = new List<Product>();
                totalData = 0;


                sqlConnection.Open();

                productList = sqlConnection.Query<Product>(
                    sqlQuery.ToString()
                ).ToList();

                totalData = this.GetTotalData(sqlConnection, sqlContent, "PRODUCT_ID");

                sqlConnection.Dispose();
                sqlConnection.Close();

                return productList;
            }
        }

        public Product GetProductInfo(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT PRODUCT_ID
                        , PRODUCT_CODE
                        , PRODUCT_NAME
                        , BRAND_ID
                        , CATEGORY_ID
                        , STATUS_ID
                        , SEX
                        , COMPOSITION
                        , DESCRIPTION
                        , IMPORT_PRICE
                        , MONEY_TYPE_ID
                        , EXCHANGE_RATE
                        , WEIGHT_POSTAGE
                        , WAGE
                        , WEIGHT
                        , IMPORT_DATE
                        , REAL_PRICE
                        , SALE_PRICE
                        , SALE_OFF_PRICE
                        , CLIP_PATH
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = PRODUCT.MODIFIED_USER_ID) MODIFIED_USER
                        , (SELECT BRAND_NAME FROM M_BRAND WHERE M_BRAND.BRAND_ID = PRODUCT.BRAND_ID) BRAND_NAME
                        , (SELECT CATEGORY_NAME FROM M_CATEGORY WHERE M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID) CATEGORY_NAME
                        , (SELECT TOP 1 FILE_PATH FROM ARTWORK WHERE PRODUCT_ID = PRODUCT.PRODUCT_ID) AS ARTWORK
                    FROM PRODUCT
                    WHERE PRODUCT_ID = {0}", productID);

                Product productInfo = new Product();

                sqlConnection.Open();

                productInfo = sqlConnection.Query<Product>(
                    sqlQuery.ToString()
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return productInfo;
            }
        }

        public IList<ProductDetail> GetProductDetailList(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT COLOR_ID
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = {0}
                    GROUP BY COLOR_ID"
                    , productID);

                IList<ProductDetail> productDetailList = new List<ProductDetail>();

                sqlConnection.Open();

                productDetailList = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return productDetailList;
            }
        }

        public IList<ProductDetail> GetProductQuantityList(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlQuery = new StringBuilder();

                sqlQuery.AppendFormat(@"
                    SELECT PRODUCT_ID
                        , PRODUCT_DETAIL_ID
                        , COLOR_ID
                        , (SELECT COLOR_NAME FROM M_COLOR WHERE M_COLOR.COLOR_ID = PRODUCT_DETAIL.COLOR_ID) COLOR_NAME
                        , SIZE
                        , QUANTITY
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = {0}", productID);

                IList<ProductDetail> list = new List<ProductDetail>();

                sqlConnection.Open();

                list = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString()
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return list;
            }
        }

        public bool UpdateProductInfo(Product data
            , IList<ProductDetail> dataDetail
            , IList<Artwork> dataArtwork
            , out long newProductID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                newProductID = data.PRODUCT_ID;

                var sqlUpdate = new StringBuilder();

                if (data.PRODUCT_ID == 0)
                {
                    sqlUpdate.AppendFormat(@"
                        INSERT INTO PRODUCT
                            (PRODUCT_CODE
                            , PRODUCT_NAME
                            , BRAND_ID
                            , CATEGORY_ID
                            , STATUS_ID
                            , COMPOSITION
                            , DESCRIPTION
                            , MONEY_TYPE_ID
                            , EXCHANGE_RATE
                            , WEIGHT_POSTAGE
                            , WAGE
                            , IMPORT_PRICE
                            , WEIGHT
                            , SALE_PRICE
                            , SALE_OFF_PRICE
                            , IMPORT_DATE
                            , DELETE_FLAG
                            , CREATED_DATE
                            , CREATED_USER_ID
                            , MODIFIED_DATE
                            , MODIFIED_USER_ID
                            , SEX
                            , REAL_PRICE
                            , ROOT_LINK)
                        VALUES
                            ('{0}', N'{1}', {2}, {3}, {4}, N'{5}', N'{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, '{16}', '{17}', {18}, '{19}', {20}, {21}, {22}, N'{23}');
                        SELECT
                            SCOPE_IDENTITY();"
                    , data.PRODUCT_CODE
                    , data.PRODUCT_NAME
                    , data.BRAND_ID
                    , data.CATEGORY_ID
                    , data.STATUS_ID
                    , data.COMPOSITION
                    , data.DESCRIPTION
                    , data.MONEY_TYPE_ID
                    , data.EXCHANGE_RATE
                    , data.WEIGHT_POSTAGE
                    , data.WAGE
                    , data.IMPORT_PRICE
                    , (data.WEIGHT.HasValue ? data.WEIGHT.Value.ToString() : "NULL")
                    , (data.SALE_PRICE.HasValue ? data.SALE_PRICE.Value.ToString() : "NULL")
                    , (data.SALE_OFF_PRICE.HasValue ? data.SALE_OFF_PRICE.Value.ToString() : "NULL")
                    , (data.IMPORT_DATE.HasValue ? "'" + data.IMPORT_DATE.Value.ToString() + "'" : "NULL")
                    , Constant.DeleteFlag.NON_DELETE
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.SEX
                    , data.REAL_PRICE
                    , data.ROOT_LINK);
                }
                else
                {
                    sqlUpdate.AppendFormat(@"
                        UPDATE PRODUCT
                            SET PRODUCT_NAME = N'{0}'
                            , BRAND_ID = {1}
                            , CATEGORY_ID = {2}
                            , STATUS_ID = {3}
                            , COMPOSITION = N'{4}'
                            , DESCRIPTION = N'{5}'
                            , MONEY_TYPE_ID = {6}
                            , EXCHANGE_RATE = {7}
                            , WEIGHT_POSTAGE = {8}
                            , WAGE = {9}
                            , IMPORT_PRICE = {10}
                            , WEIGHT = {11}
                            , SALE_PRICE = {12}
                            , SALE_OFF_PRICE = {13}
                            , IMPORT_DATE = {14}
                            , DELETE_FLAG = '{15}'
                            , MODIFIED_DATE = '{16}'
                            , MODIFIED_USER_ID = {17}
                            , SEX = {18}
                            , REAL_PRICE = {19}
                            , PRODUCT_CODE = N'{20}'
                            , ROOT_LINK = N'{21}'
                        WHERE PRODUCT_ID = {22}"
                    , data.PRODUCT_NAME
                    , data.BRAND_ID
                    , data.CATEGORY_ID
                    , data.STATUS_ID
                    , data.COMPOSITION
                    , data.DESCRIPTION
                    , data.MONEY_TYPE_ID
                    , data.EXCHANGE_RATE
                    , data.WEIGHT_POSTAGE
                    , data.WAGE
                    , data.IMPORT_PRICE
                    , (data.WEIGHT.HasValue ? data.WEIGHT.Value.ToString() : "NULL")
                    , (data.SALE_PRICE.HasValue ? data.SALE_PRICE.Value.ToString() : "NULL")
                    , (data.SALE_OFF_PRICE.HasValue ? data.SALE_OFF_PRICE.Value.ToString() : "NULL")
                    , (data.IMPORT_DATE.HasValue ? "'" + data.IMPORT_DATE.Value.ToString() + "'" : "NULL")
                    , data.DELETE_FLAG
                    , data.MODIFIED_DATE
                    , data.MODIFIED_USER_ID
                    , data.SEX
                    , data.REAL_PRICE
                    , data.PRODUCT_CODE
                    , data.ROOT_LINK
                    , data.PRODUCT_ID);
                }

                int result = 0;

                sqlConnection.Open();
                if (data.PRODUCT_ID == 0)
                {
                    result = sqlConnection.ExecuteScalar<int>(sqlUpdate.ToString());

                    if (result > 0)
                    {
                        newProductID = result;
                    }
                }
                else
                {
                    result = sqlConnection.Execute(sqlUpdate.ToString());
                }

                if (result > 0)
                {
                    result = UpdateProductDetail(sqlConnection, newProductID, dataDetail);
                }

                if (result > 0)
                {
                    result = UpdateArtwork(sqlConnection, newProductID, dataArtwork, data.MODIFIED_DATE.Value, data.MODIFIED_USER_ID);
                }

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (result > 0);
            }
        }

        private int UpdateArtwork(SqlConnection sqlConnection, long productID, IList<Artwork> dataArtwork, DateTime createdDate, long createdUserID)
        {
            int result = 1;

            if (dataArtwork != null && dataArtwork.Count > 0)
            {
                foreach (var data in dataArtwork)
                {
                    if (data.DELETED.HasValue && data.DELETED.Value)
                    {
                        var sqlDelete = new StringBuilder();

                        sqlDelete.AppendFormat(@"
                            DELETE FROM
                                ARTWORK
                            WHERE
                                PRODUCT_ID = {0}
                                AND ARTWORK_ID = {1}
                        ", productID, data.ARTWORK_ID);

                        result = sqlConnection.Execute(sqlDelete.ToString());
                    }
                    else if (data.CHANGED.HasValue && data.CHANGED.Value)
                    {
                        var sqlUpdate = new StringBuilder();

                        sqlUpdate.AppendFormat(@"
                            UPDATE ARTWORK
                                SET FILE_PATH = N'{0}'
                            WHERE PRODUCT_ID = {1}
                            AND ARTWORK_ID = {2}
                        ", data.FILE_PATH, productID, data.ARTWORK_ID);

                        result = sqlConnection.Execute(sqlUpdate.ToString());
                    }
                    else if (1 > data.ARTWORK_ID && !string.IsNullOrEmpty(data.FILE_PATH))
                    {
                        var sqlInsert = new StringBuilder();

                        sqlInsert.AppendFormat(@"
                            INSERT INTO ARTWORK
                                (PRODUCT_ID
                                , FILE_PATH
                                , CREATED_DATE
                                , CREATED_USER_ID)
                            VALUES
                                ({0}, N'{1}', '{2}', {3});
                            SELECT
                                SCOPE_IDENTITY();
                        ", productID, data.FILE_PATH, createdDate, createdUserID);

                        result = sqlConnection.ExecuteScalar<int>(sqlInsert.ToString());
                    }

                    if (result == 0 && !string.IsNullOrEmpty(data.FILE_PATH))
                        break;
                }
            }

            return result;
        }

        private int UpdateProductDetail(SqlConnection sqlConnection, long productID, IList<ProductDetail> dataDetail)
        {
            int result = 0;

            if (dataDetail != null && dataDetail.Count > 0)
            {
                foreach (var data in dataDetail)
                {
                    if (data.DELETED.HasValue && data.DELETED.Value)
                    {
                        var sqlDelete = new StringBuilder();

                        sqlDelete.AppendFormat(@"
                            DELETE FROM
                                PRODUCT_DETAIL
                            WHERE
                                PRODUCT_ID = {0}
                                AND PRODUCT_DETAIL_ID = {1}
                        ", productID, data.PRODUCT_DETAIL_ID);

                        result = sqlConnection.Execute(sqlDelete.ToString());
                    }
                    else if (data.PRODUCT_DETAIL_ID > 0)
                    {
                        var sqlUpdate = new StringBuilder();

                        sqlUpdate.AppendFormat(@"
                            UPDATE PRODUCT_DETAIL
                                SET COLOR_ID = {0}
                                , SIZE = '{1}'
                                , QUANTITY = {2}
                            WHERE PRODUCT_ID = {3}
                            AND PRODUCT_DETAIL_ID = {4} "
                            , data.COLOR_ID
                            , data.SIZE
                            , data.QUANTITY
                            , productID
                            , data.PRODUCT_DETAIL_ID);

                        result = sqlConnection.Execute(sqlUpdate.ToString());
                    }
                    else if (data.PRODUCT_DETAIL_ID < 0)
                    {
                        var sqlInsert = new StringBuilder();

                        sqlInsert.AppendFormat(@"
                            INSERT INTO PRODUCT_DETAIL
                                (PRODUCT_ID
                                , COLOR_ID
                                , SIZE
                                , QUANTITY)
                            VALUES
                                ({0}, {1}, '{2}', {3});
                            SELECT
                                SCOPE_IDENTITY();
                        ",  productID, data.COLOR_ID, data.SIZE, data.QUANTITY);

                        result = sqlConnection.ExecuteScalar<int>(sqlInsert.ToString());
                    }

                    if (result == 0)
                        break;
                }
            }

            return result;
        }

        public bool UpdateRetail(Product product, IList<Retail> retailList, bool isEdit)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                int result = 0;
                sqlConnection.Open();

                if (isEdit)
                {
                    // xoa data cu di
                    //result = sqlConnection.Execute(sqlDelete.ToString());
                }

                //if (result > 0)
                //{
                    result = InsertRetail(sqlConnection, product, retailList);
                //}

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (result > 0);
            }
        }

        private int InsertRetail(SqlConnection sqlConnection, Product product, IList<Retail> retailList)
        {
            int result = 0;
            string retailCode ="R" + product.MODIFIED_DATE.Value.ToString("yyyyMMddHHmmss");

            foreach (var data in retailList)
            {
                var sqlInsert = new StringBuilder();

                sqlInsert.AppendFormat(@"
                    INSERT INTO RETAIL
                        (RETAIL_CODE
                        , PRODUCT_ID
                        , PRODUCT_DETAIL_ID
                        , COLOR_ID
                        , SIZE
                        , QUANTITY
                        , TOTAL_PRICE
                        , CREATED_DATE
                        , CREATED_USER_ID
                        , MODIFIED_DATE
                        , MODIFIED_USER_ID)
                    VALUES
                        ('{0}', {1}, {2}, {3}, '{4}', {5}, {6}, '{7}', {8}, '{9}', {10})"
                    , retailCode
                    , product.PRODUCT_ID
                    , data.PRODUCT_DETAIL_ID
                    , data.COLOR_ID
                    , data.SIZE
                    , data.QUANTITY
                    , data.TOTAL_PRICE
                    , product.MODIFIED_DATE
                    , product.MODIFIED_USER_ID
                    , product.MODIFIED_DATE
                    , product.MODIFIED_USER_ID);

                result = sqlConnection.Execute(sqlInsert.ToString());

                if (result > 0)
                {
                    var sqlUpdate = new StringBuilder();

                    sqlUpdate.AppendFormat(@"
                        UPDATE PRODUCT_DETAIL
                            SET QUANTITY = (QUANTITY - {0})
                        WHERE PRODUCT_ID = {1}
                        AND PRODUCT_DETAIL_ID = {2}
                        AND COLOR_ID = {3}
                        AND SIZE = '{4}'"
                        , data.QUANTITY
                        , product.PRODUCT_ID
                        , data.PRODUCT_DETAIL_ID
                        , data.COLOR_ID
                        , data.SIZE);

                    result = sqlConnection.Execute(sqlUpdate.ToString());
                }

                if (result == 0)
                    break;
            }

            if (result > 0)
            {
                var sqlCount = new StringBuilder();

                sqlCount.AppendFormat(@"
                    SELECT COUNT(PRODUCT_ID)
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = {0}
                        AND QUANTITY > 0", product.PRODUCT_ID);

                int quantity = sqlConnection.ExecuteScalar<int>(sqlCount.ToString());

                if (quantity == 0) {
                    var sqlUpdate = new StringBuilder();

                    sqlUpdate.AppendFormat(@"
                        UPDATE PRODUCT
                            SET STATUS_ID = 3
                        WHERE PRODUCT_ID = {0}" , product.PRODUCT_ID);

                    result = sqlConnection.Execute(sqlUpdate.ToString());
                }
            }

            return result;
        }

        public bool DeleteProduct(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                int result = 0;
                var sqlUpdate = new StringBuilder();

                sqlUpdate.AppendFormat(@"
                    UPDATE PRODUCT
                    SET DELETE_FLAG = '1'
                    WHERE PRODUCT_ID = {0}", productID);

                sqlConnection.Open();

                result = sqlConnection.Execute(sqlUpdate.ToString());

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (result > 0);
            }
        }
    }
}