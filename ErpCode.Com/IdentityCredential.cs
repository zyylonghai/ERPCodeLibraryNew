using IdentityModel.Client;
using Library.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ErpCode.Com
{

    public class IdentityHelp
    {
        public IdentityHelp()
        { }
        /// <summary>
        /// 根据用户id生成Tick
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string GenerateTick(string userid)
        {
            string tick = string.Empty;
            string md5 = DM5Help.Md5Encrypt(userid);
            foreach (char c in md5)
            {
                tick += ((Int32)c).ToString();
            }
            return tick;
        }
        /// <summary>
        /// 比较tick是否属于当前用户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tick"></param>
        /// <returns></returns>
        public static bool CompareTick(string userid, string tick)
        {
            string newtick = GenerateTick(userid);
            return newtick == tick;
        }

        /// <summary>
        /// 加密认证数据
        /// </summary>
        /// <param name="authenData"></param>
        /// <returns></returns>
        public static string AuthenDataEncrypt(LibAuthenData authenData,string privatekey)
        {
            string jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(authenData);

            return RSAHelp.EncryptPrivekey(jsonstr, privatekey);
        }
        /// <summary>
        /// 解密认证数据
        /// </summary>
        /// <param name="authenDatastr"></param>
        /// <param name="publickey"></param>
        /// <returns></returns>
        public static LibAuthenData AuthenDataDecrypt(string authenDatastr, string publickey)
        {
            string key = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", publickey);
            string jsonstr = RSAHelp.DecryptPublickey(authenDatastr, key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LibAuthenData>(jsonstr);
        }

        public static async Task<string> Certification(string clientid,string clientsecret, LibAuthenData authenData)
        {
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(IConfigurationManager.Configuration["IdentityServer:url1"]);
            if (disco.IsError)
            {
                //Console.WriteLine(disco.Error);
                return string.Empty;
            }

            // request token
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientid,
                ClientSecret = clientsecret,

                UserName = authenData.Authenticator,
                Password = authenData.Password,
                Scope = OAuthConfig.UserApi.Scope
            }) ;
            if (tokenResponse.IsError)
            {
                //Console.WriteLine(tokenResponse.Error);
                return string .Empty ;
            }
            //var apiClient = new HttpClient();
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            //var response = await apiClient.GetAsync("https://localhost:44319/Author/Account/GetAccountInfo/Account");
            return tokenResponse.AccessToken;
        }

        //public static string CertificationWithWebClient(string clientid, string clientsecret, LibAuthenData authenData)
        //{
        //    WebClient webClient = new WebClient();
        //    // 向服务器发送POST数据
        //    string responseArray = webClient.UploadString (@"http://localhost:60831",string.Empty);
        //    //string response = Encoding.UTF8.GetString(responseArray);
        //    //returns = response;
        //    return string.Empty;
        //}
    }
    /// <summary>
    /// 身份凭证
    /// </summary>
    public  class IdentityCredential
    {
        /// <summary>
        /// 凭证号
        /// </summary>
        public string CertificateID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>

        public string UserNm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public bool HasAdminRole { get; set; }
    }

    /// <summary>
    /// 认证数据
    /// </summary>
    public class LibAuthenData
    {
        /// <summary>
        /// 认证ID
        /// </summary>
        public string AuthenticateID { get; set; }
        /// <summary>
        /// 认证者
        /// </summary>
        public string Authenticator { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDT { get; set; }
        /// <summary>
        /// 有效账号数量
        /// </summary>
        public int Validaccounts { get; set; }

    }
}
