using ErpCode.Com;
using ErpModels.Author;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ErpCodeLibrary.ApiControllers.Author
{
    [Route("Author/[controller]/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class AccountController : AuthorController
    {
        public ResponseMsg GetAccountInfo()
        {
            //this.ResponseMsg
            this.ResponseMsg.Data = new {userNm=this.UserInfo.UserNm };
            return this.ResponseMsg;
        }

        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            var account = this.ClientDatas.FirstOrDefault(i => i.DataSource == "AuthorDBContext" && i.TableNm == "Account");
            if (account != null)
            {
                var accdata = ClientDataToModel<Account>(account.ClientDataInfos).FirstOrDefault();
                if (accdata != null)
                {
                    if (account.ClientDataInfos[0].clientDataStatus == LibClientDataStatus.Add)
                    {
                        accdata.AccountId = Guid.NewGuid().ToString();
                        accdata.PasswordKey = DesCryptFactory.GenerateKey();
                        accdata.Password = DesCryptFactory.EncryptString(accdata.Password, accdata.PasswordKey);
                        //string psd = DesCryptFactory.DecryptString(accdata.Password, accdata.PasswordKey);
                        accdata.PasswordKey = DesCryptFactory.AesEncrypt(accdata.PasswordKey, AppConstManage._pwdkeyEncrykey);
                    }
                }

            }
        }
    }
}