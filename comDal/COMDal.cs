using ErpCode.Com.Enums;
using ErpModels.Appsys;
using ErpModels.Author;
using ErpModels.Com;
using LibDBContext;
using Library.Core;
using LibraryBaseDal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErpCode.comDal
{
    public class COMDal : LibDataBaseDal<ComDBContext>
    {
        public List<UserMenu> GetUserMenus()
        {
            var data = this.dBContext.UserMenu.Where(i => i.UserId == this.UserInfo.UserId).ToList();
            return data;
        }
        public List<UserMenu> GetUserprogMenusByPid(int pid)
        {
            return this.dBContext.UserMenu.Where(i => i.UserId == this.UserInfo.UserId && i.PmenuId == pid && i.MenuType == LibMenuType.Prog).ToList();
        }

        public List<JoleD> GeUserJoleProgs()
        {
            using (AuthorDBContext authdb = new AuthorDBContext())
            {
                var authobjs = (from a in authdb.UserRole
                                join b in authdb.Jole on a.JoleId equals b.JoleId
                                join c in authdb.JoleD on b.JoleId equals c.JoleId
                                where a.AccountId == this.UserInfo.UserId && b.Status == LibStatus.Enable
                                select c).ToList();
                return authobjs;

            }
        }

        public bool HasAdminJole()
        {
            using (AuthorDBContext authdb = new AuthorDBContext())
            {
                var o = (from a in authdb.UserRole
                         join b in authdb.Jole on a.JoleId equals b.JoleId
                         where a.AccountId == this.UserInfo.UserId && b.Status == LibStatus.Enable
                         select new { b.IsAdminJole }).ToList();
                return o.FirstOrDefault(i => i.IsAdminJole) != null;
            }
        }

        public UserMenu GetUserMenuByUserCode(string code)
        {
            var data = this.dBContext.UserMenu.FirstOrDefault(i => (i.UserMenuCode.ToUpper() == code.ToUpper() || i.ProgNm.ToUpper() == code.ToUpper()) 
                                     && i.UserId ==this.UserInfo .UserId && i.ClientId ==this.UserInfo .ClientId && !i.IsDeleted);
            if (data == null)
            {
                using (AppDBContext appdb = new AppDBContext())
                {
                    var pg = appdb.ProgInfo.FirstOrDefault(i => i.progNm.ToUpper() == code.ToUpper() && i.ClientId == this.UserInfo.ClientId && !i.IsDeleted);
                    if (pg==null)
                    {
                        pg = appdb.ProgInfo.FirstOrDefault(i => i.progNm.ToUpper() == code.ToUpper() && i.ClientId == AppConstManage.appDeveloper && !i.IsDeleted);
                    }
                    return new UserMenu { ProgNm = pg.progNm, MenuName = pg.progDesc };
                }
            }
            return data;
        }
    }
}
