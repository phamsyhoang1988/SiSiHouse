
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SiSiHouse.Common
{
    /// <summary>
    /// A description of the outline of ConfigurationKeys
    /// </summary>
    public sealed class ConfigurationKeys
    {
        public static readonly string SMTP_SERVER = "smtpServer";

        public static readonly string ENABLE_SSL = "EnableSsl";

        public static readonly string SMTP_PORT = "smtpPort";

        public static readonly string SMTP_USER = "smtpUser";

        public static readonly string SMTP_PASS = "smtpPass";

        public static readonly string SMTP_SUPPORT = "smtpSupport";

        public static readonly string SAVE_PICTURE = "basePicture";

        public static readonly string SAVE_CLIP = "baseClip";

        public static readonly string EXPORT_FILE = "exportFile";

        public static readonly string LIST_ITEMS_PER_PAGE = "list_items_per_page";
    }
}