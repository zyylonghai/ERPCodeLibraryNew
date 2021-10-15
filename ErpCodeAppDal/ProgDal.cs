using Library.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ViewModels.AppViewModel;
using ErpModels.Author;
using LibDBContext;
using ErpModels.Appsys;
using Microsoft.EntityFrameworkCore;
using ErpCode.Com.Enums;
using ViewModels.AuthorViewModel;
using ErpCode.Com;

namespace ErpCode.AppDal
{
    public class ProgDal: AppsysDal
    {
        public ProgModels GetProgInfoData(string prognm)
        {
            ProgModels result = null;
            var progdatas = (from a in dBContext.ProgInfo
                             join b in this.dBContext.ProgControlInfo on a.progNm equals b.ProgNm into ab
                             from x in ab.DefaultIfEmpty ()
                             join c in this.dBContext.ProgFieldInfo on a.progNm equals c.ProgNm into ac
                             from  y in ac.DefaultIfEmpty ()
                             where a.progNm == prognm && a.IsMenu 
                             select new
                             {
                                 a,
                                 x,
                                 y
                             }).ToList();
            if (progdatas == null || progdatas.Count == 0)
            {
                this.AddMessage(string.Format("未找到功能{0}", prognm),LibMessageType.Error);
                return null;
            }
            var entitys= dBContext.ChangeTracker.Entries();
            foreach (var enty in entitys)
            {
                enty.State = EntityState.Detached;
            }
            result = new ProgModels();
            var proginfo = progdatas.First().a;
            //该功能属于框架开发者的功能，需要验证是否有授权。
            if (proginfo .ClientId !=this.UserInfo .ClientId && proginfo.ClientId == AppConstManage.appDeveloper)
            {
                var o = dBContext.ProgAuthor.FirstOrDefault(i => i.TenantID == this.UserInfo.ClientId && i.ProgNm == prognm);
                if (o == null)
                {
                    //msg000000012 功能{0},您未经授权使用。
                    this.AddMessage(this.AppGetMessageDesc(12, proginfo.progDesc), LibMessageType.Error);
                    return null;
                }
            }
            proginfo.IsDeveloper = this.IsDeveloper();
            result.progInfo = proginfo;
             
            var progcontrols = progdatas.OrderBy(i=>(i.x==null ?1: i.x.OrderNo)).GroupBy(i => i.x).Select(i => i.Key).ToList();
            result.progControlInfos = progcontrols;
             
            var progfields = progdatas.OrderBy(i=>i.y ==null ?1: i.y.OrderNo ).GroupBy(i => i.y).Select(i => i.Key).ToList();
            foreach (var item in progfields)
            {
                if (item == null) continue;
                var c = progcontrols.FirstOrDefault(i => i.ID == item.ID);
                item.Title = AppGetFieldDesc(prognm, c.TableNm, item.Field);
            }
             
            result.progFieldInfos = progfields;
 
            return result;

        }

        public JoleAuthorityModels CheckAuthorityObjs()
        {
            JoleAuthorityModels models = null;
            using (AuthorDBContext authdb = new AuthorDBContext())
            {
                var authobjs =(from a in authdb.UserRole
                               join c in authdb .Jole on a.JoleId equals c.JoleId into cx
                               from y in cx.DefaultIfEmpty ()
                               join e in authdb.JoleD on y.JoleId equals e.JoleId into ex
                               from h in ex.DefaultIfEmpty ()
                               join b in authdb.AuthorityObj on a.JoleId equals b.JoleId into x
                               from  d in x.DefaultIfEmpty ()
                               where a.AccountId == this.UserInfo.UserId && y.Status ==LibStatus.Enable 
                               select new {y,h,d }).ToList();
                models = new JoleAuthorityModels();
                //var j = authobjs.FirstOrDefault(i => i.y.IsAdminJole).y;
                models.Joles = authobjs.GroupBy(i => i.y).Select(i => i.Key).ToList();
                models.JoleDs = authobjs.GroupBy(i => i.h).Select(i => i.Key).ToList();
                models.AuthorityObjs = authobjs.GroupBy(i => i.d).Select(i => i.Key).ToList();
                return models;

            }
        }
        public bool HasAdminJole()
        {
            using (AuthorDBContext authdb = new AuthorDBContext())
            {
                var o = (from a in authdb.UserRole
                         join b in authdb.Jole on a.JoleId equals b.JoleId
                         where a.AccountId==this.UserInfo.UserId && b.Status ==LibStatus.Enable 
                         select new { b.IsAdminJole }).ToList();
                return o.FirstOrDefault(i => i.IsAdminJole) != null;
            }
        }

        public ProgModels GetRptProgInfoData(string prognm)
        {
            ProgModels result = null;
            var progdatas = (from a in dBContext.ProgInfo
                             join r in dBContext.RptHtmlInfo on a.progNm equals r.progNm into ar
                             from rx in ar.DefaultIfEmpty ()
                             join b in this.dBContext.ProgControlInfo on a.progNm equals b.ProgNm into ab
                             from x in ab.DefaultIfEmpty()
                             join c in this.dBContext.ProgFieldInfo on a.progNm equals c.ProgNm into ac
                             from y in ac.DefaultIfEmpty()
                             where a.progNm == prognm && a.IsMenu
                             select new
                             {
                                 a,
                                 rx,
                                 x,
                                 y
                             }).ToList();
            if (progdatas == null || progdatas.Count == 0)
            {
                this.AddMessage(string.Format("未找到功能{0}", prognm), LibMessageType.Error);
                return null;
            }
            var entitys = dBContext.ChangeTracker.Entries();
            foreach (var enty in entitys)
            {
                enty.State = EntityState.Detached;
            }
            result = new ProgModels();
            var proginfo = progdatas.First().rx;
            result.RptHtmlInfo = proginfo;
            result.progInfo = progdatas.First().a;

            var progcontrols = progdatas.OrderBy(i => (i.x == null ? 1 : i.x.OrderNo)).GroupBy(i => i.x).Select(i => i.Key).ToList();
            result.progControlInfos = progcontrols;

            var progfields = progdatas.OrderBy(i => i.y == null ? 1 : i.y.OrderNo).GroupBy(i => i.y).Select(i => i.Key).ToList();
            foreach (var item in progfields)
            {
                if (item == null) continue;
                var c = progcontrols.FirstOrDefault(i => i.ID == item.ID);
                item.Title = AppGetFieldDesc(prognm, c.TableNm, item.Field);
            }

            result.progFieldInfos = progfields;
            return result;
        }

        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            //foreach (LibClientDatas datas in ClientDatas)
            //{
            if (string.Compare(this.ProgNm,"ProgInfo",true )==0)
            {
                var o = ClientDatas.FirstOrDefault(i => i.TableNm == "ProgInfo");
                if (o.ClientDataInfos.Count > 0 && o.ClientDataInfos[0].clientDataStatus ==LibClientDataStatus.Edit)
                {
                    ProgInfo prog = LibAppUtils.JobjectToType<ProgInfo>(o.ClientDataInfos[0]);
                   var progfields= this.dBContext.ProgFieldInfo.Where(i => i.ProgNm == prog .progNm).ToList();
                    this.DeleteDetailsHandle(progfields);
                }

            }
            //}
        }
    }
}
