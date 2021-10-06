using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ErpCode.Identity.Server
{
    public class ClientStore : IClientStore
    {
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            #region 验证clientid 与clientsecret
            var memoryClients = OAuthMemoryData.GetClients(clientId);
            if (memoryClients.Any(oo => oo.ClientId == clientId))
            {
                return memoryClients.FirstOrDefault(oo => oo.ClientId == clientId);
            }
            #endregion
            return null;
            //#region 通过数据库查询Client 信息
            //return GetClient(clientId);
            //#endregion
        }
    }
}
