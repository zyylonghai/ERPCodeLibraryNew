using ErpModels.Appsys;
using LibDBContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ErpModels.Author;

namespace ErpCode.AuthorDal
{
    public class JoleDal: AuthorDal
    {
        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();
            var auth = this.ClientDatas.FirstOrDefault(i => i.TableNm == "AuthorityObj");
            if (auth != null)
            {
                foreach (var item in auth.ClientDataInfos)
                {
                    //var o = item.Datas as AuthorityObj;
                    this.MastHandle(item.Datas as AuthorityObj);
                }
            }
        }
        public List<ProgControlInfo> GetProgControls(string prognm)
        {
            using (AppDBContext app = new AppDBContext())
            {
               return app.ProgControlInfo.Where(i => i.ProgNm == prognm).ToList();
            }
        }

        public List<object> GetProgFields(string prognm,List<AuthorityObj> olddatas)
        {
            List<object> reuslt = new List<object>();
            using (AppDBContext app = new AppDBContext())
            {
                var query = (from a in app.ProgFieldInfo
                             join b in app.ProgControlInfo on new { a.ProgNm,a.ID } equals new { b.ProgNm, b.ID }
                             where a.ProgNm == prognm && !a.IsHide
                             select new
                             {
                                 a.Field ,
                                 a.ID,
                                 a.IsHide,
                                 a.OnlyRead ,
                                 Title="",
                                 b.TableNm 
                             }).ToList();
                foreach (var item in query)
                {
                    var exis =olddatas ==null ?null : olddatas.FirstOrDefault(i => i.ControlID == item.ID && i.Field == item.Field);
                    reuslt.Add(new { 
                        item.Field, 
                        ControlID = item.ID,
                        IsHide=exis ==null ? item.IsHide:exis .IsHide,
                        OnlyRead=(exis ==null) ? item.OnlyRead:exis .OnlyRead, 
                        OldOnlyRead=item.OnlyRead ,
                        Title = this.AppGetFieldDesc(prognm, item.TableNm, item.Field) });
                }
                return reuslt;
                //return app.ProgFieldInfo
                //    .Where(i => i.ProgNm == prognm && !i.IsHide).ToList();
            }
        }

        public List<AuthorityObj> GetAuthorityObjs(string joleid)
        {
            return this.dBContext.AuthorityObj.Where(i => i.JoleId == joleid).ToList();
        }
    }
}
