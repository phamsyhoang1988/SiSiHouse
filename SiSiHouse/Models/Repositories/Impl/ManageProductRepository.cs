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

        #region Manage
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
                        , (SELECT TOP 1 FILE_PATH FROM PICTURE WHERE PICTURE.PRODUCT_ID = PRODUCT.PRODUCT_ID AND PICTURE.DISPLAY_FLAG = '1') AS PICTURE
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

                if (!string.IsNullOrEmpty(condition.STATUS_ID))
                {
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

                sqlQuery.Append(@"
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
                        , ROOT_LINK
                        , DELETE_FLAG
                        , MODIFIED_DATE
                        , (SELECT FULL_NAME FROM M_USER WHERE M_USER.USER_ID = PRODUCT.MODIFIED_USER_ID) MODIFIED_USER
                        , (SELECT BRAND_NAME FROM M_BRAND WHERE M_BRAND.BRAND_ID = PRODUCT.BRAND_ID) BRAND_NAME
                        , (SELECT CATEGORY_NAME FROM M_CATEGORY WHERE M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID) CATEGORY_NAME
                        , (SELECT TOP 1 FILE_PATH FROM PICTURE WHERE PICTURE.PRODUCT_ID = PRODUCT.PRODUCT_ID AND PICTURE.DISPLAY_FLAG = '1') AS PICTURE
                    FROM PRODUCT
                    WHERE PRODUCT_ID = @PRODUCT_ID");

                Product productInfo = new Product();

                sqlConnection.Open();

                productInfo = sqlConnection.Query<Product>(
                    sqlQuery.ToString(),
                    new
                    {
                        PRODUCT_ID = productID
                    }
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

                sqlQuery.Append(@"
                    SELECT COLOR_ID
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = @PRODUCT_ID
                    GROUP BY COLOR_ID");

                IList<ProductDetail> productDetailList = new List<ProductDetail>();

                sqlConnection.Open();

                productDetailList = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString(),
                    new
                    {
                        PRODUCT_ID = productID
                    }
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

                sqlQuery.Append(@"
                    SELECT PRODUCT_ID
                        , PRODUCT_DETAIL_ID
                        , COLOR_ID
                        , (SELECT COLOR_NAME FROM M_COLOR WHERE M_COLOR.COLOR_ID = PRODUCT_DETAIL.COLOR_ID) COLOR_NAME
                        , SIZE
                        , QUANTITY
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = @PRODUCT_ID");

                IList<ProductDetail> list = new List<ProductDetail>();

                sqlConnection.Open();

                list = sqlConnection.Query<ProductDetail>(
                    sqlQuery.ToString(),
                    new
                    {
                        PRODUCT_ID = productID
                    }
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return list;
            }
        }

        public bool UpdateProductInfo(Product data
            , IList<ProductDetail> dataDetail
            , IList<Picture> dataPicture
            , out long newProductID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                newProductID = data.PRODUCT_ID;

                var sqlUpdate = new StringBuilder();

                if (data.PRODUCT_ID == 0)
                {
                    sqlUpdate.Append(@"
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
                            (@PRODUCT_CODE, @PRODUCT_NAME, @BRAND_ID, @CATEGORY_ID, @STATUS_ID, @COMPOSITION, @DESCRIPTION
                            , @MONEY_TYPE_ID, @EXCHANGE_RATE, @WEIGHT_POSTAGE, @WAGE, @IMPORT_PRICE, @WEIGHT, @SALE_PRICE
                            , @SALE_OFF_PRICE, @IMPORT_DATE, @DELETE_FLAG, @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID
                            , @SEX, @REAL_PRICE, @ROOT_LINK);
                        SELECT
                            SCOPE_IDENTITY();");
                }
                else
                {
                    sqlUpdate.Append(@"
                        UPDATE PRODUCT
                            SET PRODUCT_CODE = @PRODUCT_CODE
                            , PRODUCT_NAME = @PRODUCT_NAME
                            , BRAND_ID = @BRAND_ID
                            , CATEGORY_ID = @CATEGORY_ID
                            , STATUS_ID = @STATUS_ID
                            , COMPOSITION = @COMPOSITION
                            , DESCRIPTION = @DESCRIPTION
                            , MONEY_TYPE_ID = @MONEY_TYPE_ID
                            , EXCHANGE_RATE = @EXCHANGE_RATE
                            , WEIGHT_POSTAGE = @WEIGHT_POSTAGE
                            , WAGE = @WAGE
                            , IMPORT_PRICE = @IMPORT_PRICE
                            , WEIGHT = @WEIGHT
                            , SALE_PRICE = @SALE_PRICE
                            , SALE_OFF_PRICE = @SALE_OFF_PRICE
                            , IMPORT_DATE = @IMPORT_DATE
                            , DELETE_FLAG = @DELETE_FLAG
                            , MODIFIED_DATE = @MODIFIED_DATE
                            , MODIFIED_USER_ID = @MODIFIED_USER_ID
                            , SEX = @SEX
                            , REAL_PRICE = @REAL_PRICE
                            , ROOT_LINK = @ROOT_LINK
                        WHERE PRODUCT_ID = @PRODUCT_ID");
                }

                int result = 0;

                sqlConnection.Open();
                if (data.PRODUCT_ID == 0)
                {
                    result = sqlConnection.ExecuteScalar<int>(
                        sqlUpdate.ToString(),
                        new
                        {
                            PRODUCT_CODE = data.PRODUCT_CODE,
                            PRODUCT_NAME = data.PRODUCT_NAME,
                            BRAND_ID = data.BRAND_ID,
                            CATEGORY_ID = data.CATEGORY_ID,
                            STATUS_ID = data.STATUS_ID,
                            COMPOSITION = data.COMPOSITION,
                            DESCRIPTION = data.DESCRIPTION,
                            MONEY_TYPE_ID = data.MONEY_TYPE_ID,
                            EXCHANGE_RATE = data.EXCHANGE_RATE,
                            WEIGHT_POSTAGE = data.WEIGHT_POSTAGE,
                            WAGE = data.WAGE,
                            IMPORT_PRICE = data.IMPORT_PRICE,
                            WEIGHT = data.WEIGHT,
                            SALE_PRICE = data.SALE_PRICE,
                            SALE_OFF_PRICE = data.SALE_OFF_PRICE,
                            IMPORT_DATE = data.IMPORT_DATE,
                            DELETE_FLAG = Constant.DeleteFlag.NON_DELETE,
                            CREATED_DATE = data.MODIFIED_DATE,
                            CREATED_USER_ID = data.MODIFIED_USER_ID,
                            MODIFIED_DATE = data.MODIFIED_DATE,
                            MODIFIED_USER_ID = data.MODIFIED_USER_ID,
                            SEX = data.SEX,
                            REAL_PRICE = data.REAL_PRICE,
                            ROOT_LINK = data.ROOT_LINK,
                        }
                    );

                    if (result > 0)
                    {
                        newProductID = result;
                    }
                }
                else
                {
                    result = sqlConnection.Execute(
                        sqlUpdate.ToString(),
                        new
                        {
                            PRODUCT_ID = data.PRODUCT_ID,
                            PRODUCT_CODE = data.PRODUCT_CODE,
                            PRODUCT_NAME = data.PRODUCT_NAME,
                            BRAND_ID = data.BRAND_ID,
                            CATEGORY_ID = data.CATEGORY_ID,
                            STATUS_ID = data.STATUS_ID,
                            COMPOSITION = data.COMPOSITION,
                            DESCRIPTION = data.DESCRIPTION,
                            MONEY_TYPE_ID = data.MONEY_TYPE_ID,
                            EXCHANGE_RATE = data.EXCHANGE_RATE,
                            WEIGHT_POSTAGE = data.WEIGHT_POSTAGE,
                            WAGE = data.WAGE,
                            IMPORT_PRICE = data.IMPORT_PRICE,
                            WEIGHT = data.WEIGHT,
                            SALE_PRICE = data.SALE_PRICE,
                            SALE_OFF_PRICE = data.SALE_OFF_PRICE,
                            IMPORT_DATE = data.IMPORT_DATE,
                            DELETE_FLAG = data.DELETE_FLAG,
                            MODIFIED_DATE = data.MODIFIED_DATE,
                            MODIFIED_USER_ID = data.MODIFIED_USER_ID,
                            SEX = data.SEX,
                            REAL_PRICE = data.REAL_PRICE,
                            ROOT_LINK = data.ROOT_LINK,
                        }
                    );
                }

                if (result > 0)
                {
                    result = UpdateProductDetail(sqlConnection, newProductID, dataDetail);
                }

                if (result > 0)
                {
                    result = UpdatePicture(sqlConnection, newProductID, dataPicture, data.MODIFIED_DATE.Value, data.MODIFIED_USER_ID);
                }

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (result > 0);
            }
        }

        private int UpdatePicture(SqlConnection sqlConnection, long productID, IList<Picture> dataPicture, DateTime createdDate, long createdUserID)
        {
            int result = 1;

            if (dataPicture != null && dataPicture.Count > 0)
            {
                foreach (var data in dataPicture)
                {
                    if (data.DELETED.HasValue && data.DELETED.Value)
                    {
                        var sqlDelete = new StringBuilder();

                        sqlDelete.Append(@"
                            DELETE FROM
                                PICTURE
                            WHERE
                                PRODUCT_ID = @PRODUCT_ID
                                AND PICTURE_ID = @PICTURE_ID");

                        result = sqlConnection.Execute(
                            sqlDelete.ToString(),
                            new
                            {
                                PRODUCT_ID = productID,
                                PICTURE_ID = data.PICTURE_ID
                            }
                        );
                    }
                    else if (1 > data.PICTURE_ID && !string.IsNullOrEmpty(data.FILE_PATH))
                    {
                        var sqlInsert = new StringBuilder();

                        sqlInsert.Append(@"
                            INSERT INTO PICTURE
                                (PRODUCT_ID
                                , FILE_PATH
                                , DISPLAY_FLAG
                                , CREATED_DATE
                                , CREATED_USER_ID)
                            VALUES
                                (@PRODUCT_ID, @FILE_PATH, @DISPLAY_FLAG, @CREATED_DATE, @CREATED_USER_ID);
                            SELECT
                                SCOPE_IDENTITY();");

                        result = sqlConnection.ExecuteScalar<int>(
                            sqlInsert.ToString(),
                            new
                            {
                                FILE_PATH = data.FILE_PATH,
                                DISPLAY_FLAG = string.IsNullOrEmpty(data.DISPLAY_FLAG) ? Constant.DEFAULT_VALUE : data.DISPLAY_FLAG,
                                PRODUCT_ID = productID,
                                CREATED_DATE = createdDate,
                                CREATED_USER_ID = createdUserID,
                            }
                        );
                    }
                    else
                    {
                        var sqlUpdate = new StringBuilder();

                        sqlUpdate.Append(@"
                            UPDATE PICTURE
                                SET DISPLAY_FLAG = @DISPLAY_FLAG
                            WHERE PRODUCT_ID = @PRODUCT_ID
                            AND PICTURE_ID = @PICTURE_ID");

                        result = sqlConnection.Execute(
                            sqlUpdate.ToString(),
                            new
                            {
                                DISPLAY_FLAG = string.IsNullOrEmpty(data.DISPLAY_FLAG) ? Constant.DisplayPicture.MAIN : data.DISPLAY_FLAG,
                                PRODUCT_ID = productID,
                                PICTURE_ID = data.PICTURE_ID
                            }
                        );
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

                        sqlDelete.Append(@"
                            DELETE FROM
                                PRODUCT_DETAIL
                            WHERE
                                PRODUCT_ID = @PRODUCT_ID
                                AND PRODUCT_DETAIL_ID = @PRODUCT_DETAIL_ID");

                        result = sqlConnection.Execute(
                            sqlDelete.ToString(),
                            new
                            {
                                PRODUCT_ID = productID,
                                PRODUCT_DETAIL_ID = data.PRODUCT_DETAIL_ID
                            }
                        );
                    }
                    else if (data.PRODUCT_DETAIL_ID > 0)
                    {
                        var sqlUpdate = new StringBuilder();

                        sqlUpdate.Append(@"
                            UPDATE PRODUCT_DETAIL
                                SET COLOR_ID = @COLOR_ID
                                , SIZE = @SIZE
                                , QUANTITY = @QUANTITY
                            WHERE PRODUCT_ID = @PRODUCT_ID
                            AND PRODUCT_DETAIL_ID = @PRODUCT_DETAIL_ID ");

                        result = sqlConnection.Execute(
                            sqlUpdate.ToString(),
                            new
                            {
                                COLOR_ID = data.COLOR_ID,
                                SIZE = data.SIZE,
                                QUANTITY = data.QUANTITY,
                                PRODUCT_ID = productID,
                                PRODUCT_DETAIL_ID = data.PRODUCT_DETAIL_ID
                            }
                        );
                    }
                    else if (data.PRODUCT_DETAIL_ID < 0 && data.COLOR_ID.HasValue)
                    {
                        var sqlInsert = new StringBuilder();

                        sqlInsert.Append(@"
                            INSERT INTO PRODUCT_DETAIL
                                (PRODUCT_ID
                                , COLOR_ID
                                , SIZE
                                , QUANTITY)
                            VALUES
                                (@PRODUCT_ID, @COLOR_ID, @SIZE, @QUANTITY);
                            SELECT
                                SCOPE_IDENTITY();");

                        result = sqlConnection.ExecuteScalar<int>(
                            sqlInsert.ToString(),
                            new
                            {
                                PRODUCT_ID = productID,
                                COLOR_ID = data.COLOR_ID,
                                SIZE = data.SIZE,
                                QUANTITY = data.QUANTITY
                            }
                        );
                    }
                    else
                    {
                        result++;
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
            string retailCode = "R" + product.MODIFIED_DATE.Value.ToString("yyyyMMddHHmmss");
            var currentDate = DateTime.Now;

            foreach (var data in retailList)
            {
                var sqlInsert = new StringBuilder();

                sqlInsert.Append(@"
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
                        (@RETAIL_CODE, @PRODUCT_ID, @PRODUCT_DETAIL_ID, @COLOR_ID, @SIZE, @QUANTITY, @TOTAL_PRICE
                        , @CREATED_DATE, @CREATED_USER_ID, @MODIFIED_DATE, @MODIFIED_USER_ID)");

                result = sqlConnection.Execute(
                    sqlInsert.ToString(),
                    new
                    {
                        RETAIL_CODE = retailCode,
                        PRODUCT_ID = product.PRODUCT_ID,
                        PRODUCT_DETAIL_ID = data.PRODUCT_DETAIL_ID,
                        COLOR_ID = data.COLOR_ID,
                        SIZE = data.SIZE,
                        QUANTITY = data.QUANTITY,
                        TOTAL_PRICE = data.TOTAL_PRICE,
                        CREATED_DATE = currentDate,
                        CREATED_USER_ID = data.MODIFIED_USER_ID,
                        MODIFIED_DATE = currentDate,
                        MODIFIED_USER_ID = data.MODIFIED_USER_ID
                    }
                );

                if (result > 0)
                {
                    var sqlUpdate = new StringBuilder();

                    sqlUpdate.Append(@"
                        UPDATE PRODUCT_DETAIL
                            SET QUANTITY = (QUANTITY - @QUANTITY)
                        WHERE PRODUCT_ID = @PRODUCT_ID
                        AND PRODUCT_DETAIL_ID = @PRODUCT_DETAIL_ID
                        AND COLOR_ID = @COLOR_ID
                        AND SIZE = @SIZE");

                    result = sqlConnection.Execute(
                        sqlUpdate.ToString(),
                        new
                        {
                            QUANTITY = data.QUANTITY,
                            PRODUCT_ID = product.PRODUCT_ID,
                            PRODUCT_DETAIL_ID = data.PRODUCT_DETAIL_ID,
                            COLOR_ID = data.COLOR_ID,
                            SIZE = data.SIZE
                        }
                    );
                }

                if (result == 0)
                    break;
            }

            if (result > 0)
            {
                var sqlCount = new StringBuilder();

                sqlCount.Append(@"
                    SELECT COUNT(PRODUCT_ID)
                    FROM PRODUCT_DETAIL
                    WHERE PRODUCT_ID = @PRODUCT_ID
                        AND QUANTITY > 0");

                int quantity = sqlConnection.ExecuteScalar<int>(
                    sqlCount.ToString(),
                    new
                    {
                        PRODUCT_ID = product.PRODUCT_ID
                    }
                );

                if (quantity == 0)
                {
                    var sqlUpdate = new StringBuilder();

                    sqlUpdate.Append(@"
                        UPDATE PRODUCT
                            SET STATUS_ID = @STATUS_ID
                        WHERE PRODUCT_ID = @PRODUCT_ID");

                    result = sqlConnection.Execute(
                        sqlUpdate.ToString(),
                        new
                        {
                            STATUS_ID = Convert.ToInt32(Constant.Status.OUT_OF_STOCK),
                            PRODUCT_ID = product.PRODUCT_ID
                        }
                    );
                }
            }

            return result;
        }

        public bool DeleteProduct(long productID, long userID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                int result = 0;
                var sqlUpdate = new StringBuilder();

                sqlUpdate.Append(@"
                    UPDATE PRODUCT
                    SET DELETE_FLAG = @DELETE_FLAG
                        , MODIFIED_DATE = @MODIFIED_DATE
                        , MODIFIED_USER_ID = @MODIFIED_USER_ID
                    WHERE PRODUCT_ID = @PRODUCT_ID");

                sqlConnection.Open();

                result = sqlConnection.Execute(
                    sqlUpdate.ToString(),
                    new
                    {
                        DELETE_FLAG = Constant.DeleteFlag.DELETE,
                        MODIFIED_DATE = DateTime.Now,
                        MODIFIED_USER_ID = userID,
                        PRODUCT_ID = productID
                    }
                );

                sqlConnection.Dispose();
                sqlConnection.Close();

                return (result > 0);
            }
        }
        #endregion

        #region Shop
        private StringBuilder BuildSqlGetCollection(CollectionCondition condition)
        {
            var sql = new StringBuilder();

            sql.Append(@"
                    SELECT PRODUCT.PRODUCT_ID
                        , PRODUCT.IMPORT_DATE
                        , PRODUCT.PRODUCT_NAME
                        , (SELECT TOP 1 FILE_PATH FROM PICTURE WHERE PICTURE.PRODUCT_ID = PRODUCT.PRODUCT_ID AND PICTURE.DISPLAY_FLAG = @DISPLAY_FLAG ORDER BY PICTURE_ID) AS PICTURE_1
						, (SELECT TOP 1 FILE_PATH FROM PICTURE WHERE PICTURE.PRODUCT_ID = PRODUCT.PRODUCT_ID AND PICTURE.DISPLAY_FLAG = @DISPLAY_FLAG ORDER BY PICTURE_ID DESC) AS PICTURE_2
                        --, PRODUCT_CODE
                        --, PRODUCT_NAME
                        --, (SELECT BRAND_NAME FROM M_BRAND WHERE M_BRAND.BRAND_ID = PRODUCT.BRAND_ID) BRAND_NAME
                        --, (SELECT CATEGORY_NAME FROM M_CATEGORY WHERE M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID) CATEGORY_NAME
                        --, STATUS_ID
                        --, SALE_PRICE
                        --, SALE_OFF_PRICE
                        --, CLIP_PATH
                    FROM PRODUCT
                        LEFT JOIN M_CATEGORY
                        ON M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID
                    WHERE PRODUCT.DELETE_FLAG = @DELETE_FLAG");

            if (condition.CATEGORY_TYPE.HasValue)
            {
                sql.Append(" AND M_CATEGORY.TYPE = @CATEGORY_TYPE ");
            }

            if (!string.IsNullOrEmpty(condition.CATEGORY_NAME))
            {
                sql.Append(" AND M_CATEGORY.CATEGORY_NAME = @CATEGORY_NAME ");
            }

            //if (!string.IsNullOrEmpty(condition.STATUS_ID))
            //{
            //    sqlContent.AppendFormat(" AND STATUS_ID IN ({0})", condition.STATUS_ID);
            //}

            return sql;
        }

        public int CountProduct(CollectionCondition condition)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sql = new StringBuilder();

                sql.Append(@"SELECT COUNT(PRODUCT_ID) FROM (
                        SELECT PRODUCT.PRODUCT_ID
                        FROM PRODUCT
                            LEFT JOIN M_CATEGORY
                            ON M_CATEGORY.CATEGORY_ID = PRODUCT.CATEGORY_ID
                        WHERE PRODUCT.DELETE_FLAG = @DELETE_FLAG ");

                if (condition.CATEGORY_TYPE.HasValue)
                {
                    sql.Append(" AND M_CATEGORY.TYPE = @CATEGORY_TYPE ");
                }

                if (!string.IsNullOrEmpty(condition.CATEGORY_NAME))
                {
                    sql.Append(" AND M_CATEGORY.CATEGORY_NAME = @CATEGORY_NAME ");
                }

                sql.Append(" ) tbData ");
                sqlConnection.Open();

                int count = sqlConnection.Query<int>(
                    sql.ToString(),
                    new
                    {
                        DELETE_FLAG = Constant.DeleteFlag.NON_DELETE,
                        DISPLAY_FLAG = Constant.DisplayPicture.MAIN,
                        CATEGORY_TYPE = condition.CATEGORY_TYPE,
                        CATEGORY_NAME = condition.CATEGORY_NAME
                    }
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return count;
            }
        }

        public IList<Product> GetCollection(CollectionCondition condition, DataTablesModel table)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sqlContent = this.BuildSqlGetCollection(condition);
                var sqlQuery = this.BuildSQLPaging(sqlContent, table);
                IList<Product> productList = new List<Product>();

                sqlConnection.Open();

                productList = sqlConnection.Query<Product>(
                    sqlQuery.ToString(),
                    new
                    {
                        DELETE_FLAG = Constant.DeleteFlag.NON_DELETE,
                        DISPLAY_FLAG = Constant.DisplayPicture.MAIN,
                        CATEGORY_TYPE = condition.CATEGORY_TYPE,
                        CATEGORY_NAME = condition.CATEGORY_NAME
                    }
                ).ToList();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return productList;
            }
        }

        public Product GetCollectionItem(long productID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                var sql = new StringBuilder();

                sql.Append(@"
                    SELECT PRODUCT_ID
                        , PRODUCT_CODE
                        , PRODUCT_NAME
                        , STATUS_ID
                        , SALE_PRICE
                        , SALE_OFF_PRICE
                        , DESCRIPTION
                    FROM PRODUCT
                    WHERE DELETE_FLAG = @DELETE_FLAG
                        AND PRODUCT_ID = @PRODUCT_ID");

                IList<Product> productList = new List<Product>();

                sqlConnection.Open();

                Product product = sqlConnection.Query<Product>(
                    sql.ToString(),
                    new
                    {
                        DELETE_FLAG = Constant.DeleteFlag.NON_DELETE,
                        PRODUCT_ID = productID
                    }
                ).FirstOrDefault();

                sqlConnection.Dispose();
                sqlConnection.Close();

                return product;
            }
        }

        #endregion
    }
}