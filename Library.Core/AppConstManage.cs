using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core
{
    public class AppConstManage
    {
        #region 特殊字符
        public const string DBInfoSeparator = "&&";
        public const string DBInfovalSeparator = "::";
        public const char DBInfoArraySeparator = '[';
        public const char DBInfoArraySeparator2 = ']';
        public const char ColonChar = ':';
        public const char Underline = '_';
        public const char Comma = ',';
        public const char Asterisk = '*';
        public const char Point = '.';
        public const char SingleQuotes = '\'';
        public const char DollarSign = '$';

        #endregion

        #region erpcode
        public const string applogid = "app_logid";
        public const string appuserinfo = "app_UserInfo";
        public const string _pwdkeyEncrykey = "ErpcodepwdAccount";
        public const string _cliidEncrykey = "erpcode-?|@crypte~by zyy";

        /// <summary>
        /// 框架开发的租户
        /// </summary>
        public const string appDeveloper = "100000";
        /// <summary>自定义表设计功能</summary>
        public const string appUserDefindTable = "UserDefindTable";
        //public const string applanguage = "app_language";
        //public const string app_user = "";
        #endregion 
    }
}
