using ErpCode.Com;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using LibDBContext;
using Library.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace ErpCode.Identity.Server
{
    public class OAuthMemoryData
    {
        #region API资源
        /// <summary>
        /// Api 资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(
                    OAuthConfig.UserApi.Scope,
                    OAuthConfig.UserApi.Scope,
                    new List<string>(){JwtClaimTypes.Role }
                    ),
            };
        }
        #endregion

        public static IEnumerable<LibClient> GetClients(string clid)
        {
            List<LibClient> result = new List<LibClient>();
            using (AppDBContext db = new AppDBContext())
            {
                var data = db.License.Where(i => i.ClientId == clid).ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        result.Add(new LibClient {
                            ClientId = item.ClientId,
                            AllowedGrantTypes = new List<string>()
                            {
                                GrantTypes.ResourceOwnerPassword.FirstOrDefault(),//Resource Owner Password模式
                                                                                          //GrantTypeConstants.ResourceWeixinOpen,
                            },
                            ClientSecrets = { new Secret(item.ClientSecret.Sha256()) },
                            AllowOfflineAccess = true,//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true
                            AllowedScopes = {
                                        OAuthConfig.UserApi.Scope,
                                        StandardScopes.OfflineAccess,
                                    },
                            AccessTokenLifetime = OAuthConfig.ExpireIn,
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 测试的账号和密码
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser()
                {
                     SubjectId = "1",
                     Username = "test",
                     Password = "123456"
                },
            };
        }

        /// <summary>
        /// 为了演示，硬编码了，
        /// 这个方法可以通过DDD设计到底层数据库去查询数据库
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static UserModel GetUserByUserName(string clid, string clientsecret,string userName, string password)
        {
            using (AppDBContext db = new AppDBContext())
            {
                //var keyinfo = db.TenantKeyInfo.FirstOrDefault(i => i.ClientId == clid);
                //if (keyinfo != null)
                //{
                    var data = db.License.FirstOrDefault(i => i.ClientId == clid && i.ClientSecret == clientsecret);
                    if (data != null)
                    {
                        LibAuthenData authenData = JsonConvert.DeserializeObject<LibAuthenData>(RSAHelp.DecryptPublickey(data.AuthenData,data.PublicKey));
                        if (authenData.Authenticator == userName && authenData.Password == password)
                        {
                            return new UserModel
                            {
                                DisplayName = "",
                                MerchantId = 100,
                                Password = authenData.Password,
                                Role = EnumUserRole.Normal,
                                SubjectId = "520",
                                UserId = authenData.AuthenticateID,
                                UserName = authenData.Authenticator,
                                ExpirationDT = authenData.ExpirationDT,
                                Validaccounts = authenData.Validaccounts
                            };
                        }
                    }
                //}
            }
            return null;
        }
    }

    public class LibClient : Client
    {
        public string zyynam { get; set; }
    }
}
