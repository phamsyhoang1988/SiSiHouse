using System.Collections.Generic;
using System.Collections.Specialized;

namespace SiSiHouse.Common
{
    /// <summary>
    /// Common constants definition
    /// </summary>
    public class Constant
    {
        /// <summary>The database connection string name</summary>
        public const string CONNECTION_STRING_NAME = "SiSiHouseConnection";

        /// <summary>Key name for login user in session object</summary>
        public const string SESSION_LOGIN_USER = "SESSION_LOGIN_USER";

        // Set string validate password
        public const string REG_PASSWORD = @"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$";

        /// <summary>Max size image</summary>
        public const int MaxContentLength = 1024 * 500; //500Kb;

        /// <summary>Input image</summary>
        public const string Input_Image = "/Content/img/input_image.jpg";

        /// <summary>No Image</summary>
        public const string No_Image = "/Content/img/no_image.jpg";

        public const string DISPLAY_PASSWORD = "••••••••";

        /// <summary>Default value </summary>
        public const string DEFAULT_VALUE = "0";

        public static string[] AllowedFileExtensions = new string[] { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };

        public const string MAX_TIME_IN_DAY = " 23:59:59";

        public static string[] AdminManage = new string[] { "ManageStatistics", "ManageBrand", "ManageCategory", "ManageMoney", "ManageColor" };

        public const int DISPLAY_ITEM_PER_PAGE = 20;

        public class Role
        {
            public const int USER = 0;

            public const int ADMIN = 1;

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { USER, "Thành viên" },
                { ADMIN, "Quản trị" }
            }.AsReadOnly();
        }

        public class Status
        {
            public const string WAITING = "0";

            public const string SELLING = "1";

            public const string SALE_OFF = "2";

            public const string OUT_OF_STOCK = "3";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { WAITING, "Đang nhập" },
                { SELLING, "Đang bán" },
                { SALE_OFF, "Giảm giá" },
                { OUT_OF_STOCK, "Hết hàng" }
            }.AsReadOnly();
        }

        public class Sex
        {
            public const string MALE = "0";

            public const string FEMALE = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { MALE, "Nam" },
                { FEMALE, "Nữ" }
            }.AsReadOnly();
        }

        public class CategoryType
        {
            public const int CLOTHES = 1;

            public const int FOOTWEARS = 2;

            public const int ACCESSORIES = 3;

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { CLOTHES, "Quần áo" },
                { FOOTWEARS, "Giày dép" },
                { ACCESSORIES, "Phụ kiện" }
            }.AsReadOnly();
        }

        public class CategoryName
        {
            public const string CLOTHES = "clothes";
            public const string FOOTWEARS = "footwears";
            public const string ACCESSORIES = "accessories";
            public const string COATS = "coats";
            public const string JACKETS = "jackets";
            public const string SNEAKERS = "sneakers";

        }

        public class ExportRevenue
        {
            public const string TITLE = "Thống kê doanh thu chi tiết tháng {0} năm {1}";
            public const int START_ROW = 6;
            public const int END_COLUMN = 9;
            public const int START_TOTAL_COLUMN = 4;
            public const int END_TOTAL_COLUMN = 8;
        }

        public class DeleteFlag
        {
            public const string NON_DELETE = "0";
            
            public const string DELETE = "1";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { NON_DELETE, "" },
                { DELETE, "Deleted" }
            }.AsReadOnly();
        }

        public class DisplayPicture
        {
            public const string MAIN = "1";

            public const string EXTRA = "0";
        }

        public class WindowName
        {
            public const string COOKIE_NAME = "WindowName";
            
            public const string MAIN = "Main";

            public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { "Main", MAIN }
            }.AsReadOnly();
        }
    }
}