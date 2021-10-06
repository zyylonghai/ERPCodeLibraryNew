using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels.Appsys;
using ErpModels.UserTable;
using LibDBContext;
using Library.Core;
using LibraryBaseDal;
using System;
using System.Linq;

namespace ErpCode.AuthorDal
{
    public class AuthorDal : LibDataBaseDal<AuthorDBContext>
    {
        public License GetLicense(string cliid, string clientdatastr)
        {
            string clientid = DesCryptFactory.DecryptString(cliid, AppConstManage._cliidEncrykey);
            using (AppDBContext app = new AppDBContext())
            {
                var lic = app.License.FirstOrDefault(i => i.ClientId == clientid);
                if (lic != null)
                {
                    string s = RSAHelp.DecryptPublickey(clientdatastr, lic.PublicKey);
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject(s);
                    Newtonsoft.Json.Linq.JObject jobj = (Newtonsoft.Json.Linq.JObject)o;
                    if (string.Compare(jobj.GetValue("Secret").ToString(), lic.ClientSecret,true) == 0)
                        return lic;
                }
            }
            return null;
        }

        public U_TableStorageInfo GetStorageInfo(string clientid)
        {
            using (UserTableDBContext udb = new UserTableDBContext())
            {
                return udb.U_TableStorageInfo.FirstOrDefault(i => i.ClientId == clientid && !i.IsDeleted);
            }
        }

        //public string DecryptClientInfo(string cliid, string clientdatastr)
        //{
        //    string clientid = DesCryptFactory.DecryptString(cliid, AppConstManage._cliidEncrykey);
        //    using (AppDBContext app = new AppDBContext())
        //    {
        //        var o = app.TenantKeyInfo.FirstOrDefault(i => i.ClientId == clientid);
        //        if (o != null)
        //        {
        //            return RSAHelp.DecryptPublickey(clientdatastr, o.PublicKey);
        //        }
        //    }
        //    return string.Empty;
        //}

        public LibLoginInfo Login(string userNm,string pwd,string clientid)
        {
            var exist = this.dBContext.Account.FirstOrDefault(i => i.AccountNm == userNm && i.ClientId ==clientid && i.Status==LibAccountStatus.Activation && !i.IsDeleted);
            LibLoginInfo loginInfo = new LibLoginInfo();
            if (exist != null)
            {
                string pwdkey = DesCryptFactory.AesDecrypt(exist.PasswordKey, AppConstManage._pwdkeyEncrykey);
                string password = DesCryptFactory.DecryptString(exist.Password, pwdkey);
                if (pwd == password)
                {
                    loginInfo.loginResult = 1;
                    loginInfo.AccountDes = exist.AccountDesc;
                    loginInfo.AccountId = exist.AccountId;
                    loginInfo.AccountNm = exist.AccountNm;
                }
                else
                {
                    loginInfo.loginResult = 3;
                }
            }
            else
            {
                loginInfo.loginResult = -1;
            }
            return loginInfo;
        }
    }
}
