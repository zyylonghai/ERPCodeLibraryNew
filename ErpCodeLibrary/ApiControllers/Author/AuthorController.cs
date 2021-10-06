using ErpCode.AuthorDal;
using ErpCode.BaseApiController;
using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels.Appsys;
using Library.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ErpCodeLibrary.ApiControllers.Author
{
    [Route("Author/[action]/{prog}")]
    [ApiController]
    //[Authorize]
    public class AuthorController : DataBaseController<AuthorDal>
    {
        [HttpPost]
        public ResponseMsg Login(AccountInfo info)
        {
            //string clinfo = this.tDal.DecryptClientInfo(info.appclid, info.appcliinfo);
            //var cli= Newtonsoft.Json.JsonConvert.DeserializeObject(clinfo);
            //Newtonsoft.Json.Linq.JObject jobj = (Newtonsoft.Json.Linq.JObject)cli;
            License license = this.tDal.GetLicense(info.appclid, info.appcliinfo);
            LibAuthenData authenData = IdentityHelp.AuthenDataDecrypt(license.AuthenData, license.PublicKey);
            string token = IdentityHelp.Certification(license.ClientId, license.ClientSecret, authenData).Result;
            if (!string.IsNullOrEmpty(token))
            {
                var logininfo = this.tDal.Login(info.userNm, info.password,license .ClientId);
                if (logininfo.loginResult == 1)
                {
                    this.UserInfo = new LibClientUserInfo();
                    this.UserInfo.UserId = logininfo.AccountId;
                    this.UserInfo.UserNm = logininfo.AccountDes;
                    this.UserInfo.SessionId = HttpContext.Session.Id;
                    this.UserInfo.Language = Convert.ToInt32(info.language);
                    this.UserInfo.ClientId = license.ClientId;
                    #region 获取自定义的存储位置
                    var storageinfo = this.tDal.GetStorageInfo(this.UserInfo.ClientId);
                    if (storageinfo != null)
                    {
                        this.UserInfo.U_TBFieldNm = storageinfo.StorageTableFieldNm;
                        this.UserInfo.U_TBNm = storageinfo.StorageTableNm;
                    }
                    else
                    {
                        this.UserInfo.U_TBFieldNm = string .Empty ;
                        this.UserInfo.U_TBNm =string .Empty;
                    }
                    #endregion
                    //this.SessionData.UserInfo = userInfo;
                    HttpContext.Response.Cookies.Append("apptick", token);
                }
                else if (logininfo.loginResult == 3)
                {
                    //msg000000006  登录密码错误
                    this.AddMessage(6);
                }
                else if (logininfo.loginResult == -1)
                {
                    //msg000000011 用户名{0},不存在。
                    this.AddMessage(11,LibMessageType.Error, info.userNm);
                }
            }
            //ResponseMsg.Data = true;
            return ResponseMsg;
        }
        [HttpGet]
        public ResponseMsg LoginOut()
        {
            this.ClientDatas = null;
            HttpContext.Session.Clear();
            this.ClearCach();
            this.ClearSession();
            HttpContext.Response.Cookies.Delete("apptick");
            return ResponseMsg;
        }

        [HttpGet]
        public ResponseMsg GetLanguage()
        {
            var result = LibAppUtils.GetenumFields<LibLanguage>();
            result.ForEach(i => { i.value = AppGetFieldDesc(string.Empty, typeof(LibLanguage).Name, i.value); });
            CachHelp cachHelp = new CachHelp();
            cachHelp.RemoveCache(string.Empty);
            ResponseMsg.Data = result;
            return ResponseMsg;
        }
        [HttpGet]
        public ResponseMsg GetLanguage2()
        {
            var result = LibAppUtils.GetenumFields<LibLanguage>();
            List<object> list = new List<object>();
            result.ForEach(i => { list.Add(new {key=i.value ,value= AppGetFieldDesc(string.Empty, typeof(LibLanguage).Name, i.value) }); });
            CachHelp cachHelp = new CachHelp();
            cachHelp.RemoveCache(string.Empty);
            ResponseMsg.Data = list;
            return ResponseMsg;
        }

    }

    public class AccountInfo
    {
        public string userNm { get; set; }

        public string password { get; set; }
        public string language { get; set; }

        public string appclid { get; set; }
        public string appcliinfo { get; set; }
    }
}