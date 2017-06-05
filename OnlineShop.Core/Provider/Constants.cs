using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Core.Provider
{
    public static class Constants
    {
        #region Setting currency

        public const string SettingCurrencyUnit = "$";
        #endregion
        #region base setting fields
        public const string SettingLogo = "LogoImage";
        public const string SettingWebsiteName = "WebsiteName";
        //public const string SettingLogo = "Administrator";

        #endregion
        public const string PasswordHash = "P@@Sw0rd";
        public const string SaltKey = "S@LT&KEY";
        public const string VIKey = "@1B2c3D4e5F6g7H8";
        public const string AreaAdmin = "Administrator";
        public const string DefaultImg = "/Content/images/default-img.png";
        public const string NotifyMessage = "MessageError";
        public const string UserLayOut = "~/Views/Shared/_Layout.cshtml";
        public const string UserSplitLayOut = "~/Views/Shared/_SplitLayout.cshtml";
        public const string CookieCart = "cart";
        public const string ImageStore = "~/Upload/Product";
        public const string ImagePath = "/Upload/Product";
        public const string DateFormat = "{0:MM/dd/yyyy}";
        public static readonly Dictionary<int, string> ProductStatusDictionary = new Dictionary<int, string>()
        {

            {(int)EnumProductStatus.InComming,"In Comming"},
            {(int)EnumProductStatus.Stocking,"Stocking"},
            {(int)EnumProductStatus.OutOfStock,"Out Of Stock"}
        };
        public static readonly Dictionary<int, string> TypeDictionary = new Dictionary<int, string>()
        {

            {(int)EnumType.String,"String"},{(int)EnumType.Bool,"Boolean"},{(int)EnumType.Int,"Integer"},{(int)EnumType.Double,"Double"}
        };
        public static readonly Dictionary<int, string> StatusDictionary = new Dictionary<int, string>()
        {

            {(int)EnumStatus.Active,"Active"},
            {(int)EnumStatus.InActive,"Inactive"},
        };
        public enum EnumType
        {
            String = 1,
            Bool,
            Int,
            Double
        }

        public enum EnumProductStatus
        {
            Stocking = 1,
            InComming,
            OutOfStock,
        }
        public enum EnumOrderStatus
        {
            Pending = 0,
            Solve
        }
        public enum EnumStatus
        {
            InActive = 0,
            Active
        }
    }
}