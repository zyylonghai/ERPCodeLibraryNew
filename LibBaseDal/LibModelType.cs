using ErpModels.Appsys;
using ErpModels.Author;
using ErpModels.Com;
using ErpModels.UserTable;
using ErpModels.WMS.RawMaterial;
using ErpRptModels.Com;
using LibDBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryBaseDal
{
    public partial class LibDataBaseDal<TDBContext>
        where TDBContext : BaseDBContext
    {
        public Type GetModelType(string nm)
        {
            switch (nm)
            {
                case "CheckBillMap":
                    return typeof(CheckBillMap);
                case "ProgInfo":
                    return typeof(ProgInfo);
                case "ProgControlInfo":
                    return typeof(ProgControlInfo);
                case "ProgFieldInfo":
                    return typeof(ProgFieldInfo);
                case "LanguageField":
                    return typeof(LanguageField);
                case "CheckBill":
                    return typeof(CheckBill);
                case "Account":
                    return typeof(Account);
                case "Materials":
                    return typeof(Materials);
                case "Jole":
                    return typeof(Jole);
                case "JoleD":
                    return typeof(JoleD);
                case "AuthorityObj":
                    return typeof(AuthorityObj);
                case "UserRole":
                    return typeof(UserRole);
                case "CodeRule":
                    return typeof(CodeRule);
                case "CodeRuleD":
                    return typeof(CodeRuleD);
                case "CodeRuleConfig":
                    return typeof(CodeRuleConfig);
                case "UserMenu":
                    return typeof(UserMenu);
                case "RptHtmlInfo":
                    return typeof(RptHtmlInfo);
                case "CheckRptHeard":
                    return typeof(CheckRptHeard);
                case "CheckRptData":
                    return typeof(CheckRptData);
                case "U_TableInfo":
                    return typeof(U_TableInfo);
                case "U_TableFieldInfo":
                    return typeof(U_TableFieldInfo);
                case "U_DataSourceInfo":
                    return typeof(U_DataSourceInfo);
                default:
                    if (nm.StartsWith("U_TableInfo", StringComparison.OrdinalIgnoreCase))
                    {
                        return typeof(U_TableInfo);
                    }
                    else if (nm.StartsWith("U_TableFieldInfo", StringComparison.OrdinalIgnoreCase))
                    {
                        return typeof(U_TableFieldInfo);
                    }
                    else
                        return null;
            }
        }
    }
}
