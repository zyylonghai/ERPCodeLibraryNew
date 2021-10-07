using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCode.Com;
using ErpCode.Com.Enums;
using ErpModels.Appsys;
using ErpModels.Com;
using Library.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ErpCodeLibrary.ApiControllers.SYS
{
    [Route("SYS/[controller]/[action]/{prog}")]
    [ApiController]
    [Authorize]
    public class AppsettingController : SYSController
    {
        public override string BindDataGrid(string gridid, string ds, string tbnm,int limit, int page)
        {
            //var data = this.tDal.GetUserprogMenusByPid(pid);
            List<UserMenu> data = new List<UserMenu>();
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            long pid =string.IsNullOrEmpty(this.Request.Query["pid"].ToString ())?-1 :Convert.ToInt64(this.Request.Query["pid"]);
            //var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            if (exit != null && exit.ClientDataInfos != null && exit.ClientDataInfos.Count > 0)
            {
                List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
                data = menus.Where(i => i.PmenuId == pid).ToList();
            }
            this.SessionData.AddDataExt("pid",pid);
            return ReturnGridData("", data == null ? 0 : data.Count, data);
            //var result = new { code = 0, msg = "success", count = data == null ? 0 : data.Count, data = data };

            //return JsonConvert.SerializeObject(result);
        }
        protected override void GetTableActionExt(LibClientDatas clientDatas)
        {
            base.GetTableActionExt(clientDatas);
            long pid = this.SessionData.GetDataExt<long>("pid");
            if (pid == -1)
            {
                //msg000000010  请先选择一个节点。
                ThrowErrorException(10);
            }
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            UserMenu menu = clientDatas.ClientDataInfos[0].Datas as UserMenu;

            if (exit != null && exit.ClientDataInfos != null && exit.ClientDataInfos.Count > 0)
            {
                List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
                menu.MenuId = menus.Max(i => i.MenuId) + 1;
            }
            else
            {
                menu.MenuId = 1;
            }
            menu.PmenuId = pid;
            menu.UserId = this.UserInfo.UserId;

        }

        protected override void TableActionExt(LibClientDatas clientDatas)
        {
            base.TableActionExt(clientDatas);
            UserMenu menu = clientDatas.ClientDataInfos[0].Datas as UserMenu;
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
            UserMenu exitv = menus.FirstOrDefault(i => i.app_logid == menu.app_logid);
            if (exitv != null)
                menu.PmenuId = exitv.PmenuId;
        }

        protected override void SetSearchFieldsExt(string tbnm, LibSearchKind kind,ref List<LibFieldInfo> fields)
        {
            base.SetSearchFieldsExt(tbnm, kind,ref fields);
            if (tbnm == "ProgInfo" && kind == LibSearchKind.FromDataSearch)
            {
                //string fs = "CreateDT,Creater,LastModifyDT,LastModifier";
                fields = fields.Where(i => i.fieldNm == "progNm" || i.fieldNm == "progDesc").ToList();
                fields.RemoveAll(delegate(LibFieldInfo f){ 
                    if (f.fieldNm == "progNm" || f.fieldNm == "progDesc") 
                        return false;
                    return true;
                });
            }
        }
        protected override void SearchDataExt(string tbnm, LibSearchKind kind, ref List<object> data)
        {
            base.SearchDataExt(tbnm, kind, ref data);
            if (tbnm == "ProgInfo" && kind == LibSearchKind.FromDataSearch)
            {
                bool isadmin = this.tDal.HasAdminJole();
                if (!isadmin)
                {
                    var joledata = this.tDal.GeUserJoleProgs();

                    List<object> result = new List<object>();
                    foreach (ProgInfo prog in data)
                    {
                        if (joledata.FirstOrDefault(i => i.ProgNm == prog.progNm) != null)
                        {
                            result.Add(prog);
                        }
                    }
                    data = result;

                }
            }
        }

        //protected override void PageLoadExt()
        //{
        //    var data = this.tDal.GetUserMenus();
        //    var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
        //    if (exit != null)
        //    {
        //        exit.ClientDataInfos.Clear();
        //        foreach (var item in data)
        //        {
        //            exit.ClientDataInfos.Add(new LibClientDataInfo { clientDataStatus = LibClientDataStatus.Preview, Datas = item });
        //        }
        //    }
        //    base.PageLoadExt();
        //}

        [HttpGet]
        public ResponseMsg GetUserMenu()
        {
            List<LibTreeObject> result = new List<LibTreeObject>();
            var data = this.tDal.GetUserMenus();
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            if (exit != null)
            {
                exit.ClientDataInfos.Clear();
                foreach (var item in data)
                {
                    exit.ClientDataInfos.Add(new LibClientDataInfo { clientDataStatus = LibClientDataStatus.Preview, Datas = item });
                }
            }

            result = getTreedata(data.Where(i => i.MenuType == LibMenuType.None).ToList());
            return new ResponseMsg { IsSuccess = true, Data = new { direct = result } };
        }

        public ResponseMsg GetAllUserMenu()
        {
            //List<LibTreeObject> result = new List<LibTreeObject>();
            var data = this.tDal.GetUserMenus();
            //result = getTreedata(data);
            return new ResponseMsg { IsSuccess = true, Data = data };
        }

        [HttpGet]
        public ResponseMsg CreateTreeNode(long pid)
        {
            long maxid = 1;
            //LibTreeObject treeObject = new LibTreeObject();
            List<LibTreeObject> result = new List<LibTreeObject>();
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            if (exit != null && exit.ClientDataInfos != null )
            {
                List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
                var m = menus.FirstOrDefault(i => i.MenuId == pid);
                if (m != null) m.Spread = true;
                maxid =menus.Count ==0?0: menus.Max(i => i.MenuId);
                UserMenu menu = new UserMenu
                {
                    MenuId = maxid + 1,
                    PmenuId = pid,
                    MenuName = "菜单节点",
                    MenuType = LibMenuType.None
                };
                exit.ClientDataInfos.Add(new LibClientDataInfo { clientDataStatus = LibClientDataStatus.Add, Datas = menu });
                menus.Add(menu);
                result = getTreedata(menus.Where (i=>i.MenuType==LibMenuType.None ).ToList ());
                result[0].spread = true;
            }
            else if (pid == 0)
            {
                
            }
            return new ResponseMsg { IsSuccess = true, Data = result };
        }
        [HttpGet]
        public ResponseMsg GetMaxMenuid()
        {
            long maxid = 1;
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            if (exit != null && exit.ClientDataInfos != null && exit.ClientDataInfos.Count > 0)
            {
                List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
                maxid = menus.Max(i => i.MenuId);
            }
            return new ResponseMsg { IsSuccess = true, Data = maxid };
        }

        public ResponseMsg SaveMenus()
        {
            var exit = this.ClientDatas.FirstOrDefault(i => i.TableNm == "UserMenu");
            foreach (var item in exit.ClientDataInfos)
            {
                UserMenu m = item.Datas as UserMenu;
                if (string.IsNullOrEmpty(m.UserId))
                {
                    m.UserId = this.UserInfo.UserId;
                }
            }
            this.tDal .Update(new List<LibClientDatas> { exit });
            //msg000000001 保存成功
            this.AddMessage(1, LibMessageType.Prompt);
            List<UserMenu> menus = ClientDataToModel<UserMenu>(exit.ClientDataInfos);
            var respons = this.ResponseMsg;
            respons.Data = getTreedata(menus.Where(i => i.MenuType == LibMenuType.None).ToList());
            return respons;
        }

        private List<LibTreeObject> getTreedata(List<UserMenu> data)
        {
            List<LibTreeObject> result = new List<LibTreeObject>();
            LibTreeObject root = new LibTreeObject { id = 0, pid = 0, title = "我的菜单" };
            result.Add(root);
            if (data != null && data.Count > 0)
            {
                //var menus = data.Where(i => i.MenuType == LibMenuType.None).ToList();

                var direct = LibAppUtils.ConverToTreeObj(data, new LibTreeConfig { TreeIdCol = "MenuId", PTreeIdCol = "PmenuId", TitleCol = "MenuName",SpreadCol= "Spread" });
                root.children = new List<LibTreeObject>();
                foreach (var item in direct)
                {
                    root.children.Add(item);
                }
            }
            return result;
        }
    }
}