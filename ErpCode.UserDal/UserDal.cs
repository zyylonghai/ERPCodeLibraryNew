using ErpCode.Com;
using ErpModels;
using ErpModels.UserTable;
using LibDBContext;
using Library.Core;
using LibraryBaseDal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ErpCode.UserDal
{
    public class UserDal : LibDataBaseDal<UserTableDBContext>
    {
        private List<object> updateData = new List<object>();

        public List<U_TableFieldInfo> U_TableFields { get; set; }
        public UserDal()
            :base ()
        {
            U_TableFields = new List<U_TableFieldInfo>();
        }
        public List<U_TableFieldInfo> GetTableFieldsInfo(string tbnm)
        {
            LibDbParameter[] parameters = new LibDbParameter[2];
            parameters[0] = new LibDbParameter { ParameterNm = "@clientid", DbType = DbType.String, Value = this.UserInfo.ClientId };
            parameters[1] = new LibDbParameter { ParameterNm = "@tbnm", DbType = DbType.String, Value = tbnm };
            var tbs = this.dBContext.Database.ExeStoredProcedureToData("p_GetUserTableFieldInfoBytbnm", parameters).ToList<U_TableFieldInfo>();
            return tbs;
        }
        /// <summary>自定义表数据存储的实际表名（即对应的u_data表）</summary>
        /// <param name="clientid"></param>
        /// <param name="utablenm"></param>
        /// <param name="tablenm"></param>
        /// <returns></returns>
        public U_TableInfo GetUDataNm(string clientid, string utablenm, string tablenm)
        {
            LibDbParameter[] parameters = new LibDbParameter[3];
            parameters[0] = new LibDbParameter { ParameterNm = "@clientid", DbType = DbType.String, Value = this.UserInfo.ClientId };
            parameters[1] = new LibDbParameter { ParameterNm = "@utableNm", DbType = DbType.String, Value = utablenm };
            parameters[2] = new LibDbParameter { ParameterNm = "@tableNm", DbType = DbType.String, Value = tablenm };
            var tbs = this.dBContext.Database.ExeStoredProcedureToData("p_GetUData", parameters).ToList<U_TableInfo>().FirstOrDefault();
            return tbs;
        }

        /// <summary>获取租户自定义表信息的实际存储的表名（即对应的U_TableInfo）</summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public string GetTableInfoNm(string clientid)
        {
            var o = this.GetStorageInfo(clientid);
            if (o == null)
                return string.Empty;
            return o.StorageTableNm;
        }

        private U_TableStorageInfo GetStorageInfo(string clientid)
        {
            return this.dBContext.U_TableStorageInfo.FirstOrDefault(i => i.ClientId == clientid && !i.IsDeleted);
        }

        public override void Update(List<LibClientDatas> clientdata)
        {

            this.ClientDatas = clientdata;
            //当前属于框架开发的租户，并且当前功能是 自定义表设计。
            //if (this.IsDeveloper() && string .Compare(this.ProgNm,"UserDefindTable",true)==0)
            //{
            //    base.Update(clientdata);
            //    return;
            //}
            StringBuilder sql = new StringBuilder();
            bool isudata = true;
            sql.Append(" begin ");
            sql.Append(" begin tran ");
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                //var o = this.dBContext.U_TableStorageInfo.FirstOrDefault(i => i.ClientId == this.UserInfo.ClientId && !i.IsDeleted);
                //if (o != null)
                //{
                //StringBuilder sql = new StringBuilder();
                //sql.Append(" begin ");
                //sql.Append(" begin tran ");
                isudata = false;
                var d = ClientDatas.FirstOrDefault(i => i.TableNm == "U_TableInfo");
                if (d != null)
                {
                    string udatatbnm = string.Empty;
                    if (d.ClientDataInfos.Count > 0 && d.ClientDataInfos[0].clientDataStatus == LibClientDataStatus.Add)
                        udatatbnm = GetUdataTableNm("U_Data");
                    //d.TableNm = o.StorageTableNm;
                    //this.dBContext.Database.ExecuteSql
                    foreach (var clidata in d.ClientDataInfos)
                    {
                        sql.Append(DoGetTBinfosql(clidata, this.UserInfo.U_TBNm,udatatbnm));
                    }
                }
                d = ClientDatas.FirstOrDefault(i => i.TableNm == "U_TableFieldInfo");
                if (d != null)
                {
                    foreach (var clidata in d.ClientDataInfos)
                    {
                        sql.Append(DoGetTBFieldinfosql(clidata, this.UserInfo.U_TBFieldNm));
                    }
                }

                //sql.Append(" if @@error<>0 begin rollback tran return end commit tran");
                //sql.Append(" end ");
                //this.dBContext.Database.ExecuteSql(sql.ToString());
                //this.DoWritDataLog(this.updateData);

                //}
            }
            else
            {
                foreach (var item in ClientDatas)
                {
                    var udataobj= this.GetUDataNm(this.UserInfo.ClientId, this.UserInfo.U_TBNm, item.TableNm);
                    var storagetbnm = udataobj.DataTBNm;
                    if (udataobj.TableKind == Com.Enums.LibTableKind.Virtual) continue;
                    foreach (var d in item.ClientDataInfos)
                    {
                        d.TableNm = item.TableNm;
                        sql.Append(DoGetUDataSQL(d, storagetbnm, item.TableNm));
                    }
                }
            }

            sql.Append(" if @@error<>0 begin rollback tran return end commit tran");
            sql.Append(" end ");
            this.dBContext.Database.ExecuteSql(sql.ToString());
            this.DoWritDataLog(this.updateData, isudata);

        }

        public override List<object> FillSearchData(Dictionary<string, object> mstKeys, string tbnm)
        {
            //List<object> result = new List<object>();

            string tb = string.Empty;
            if (string.Compare(this.ProgNm, AppConstManage.appUserDefindTable, true) == 0)
            {
                if (string.Compare(tbnm, "U_TableInfo", false) == 0)
                {
                    //tb = storage.StorageTableNm;
                    tb = this.UserInfo.U_TBNm;
                }
                else if (string.Compare(tbnm, "U_TableFieldInfo", false) == 0)
                {
                    //tb = storage.StorageTableFieldNm;
                    tb = this.UserInfo.U_TBFieldNm;
                }
                if (!mstKeys.ContainsKey("ClientId"))
                    mstKeys.Add("ClientId", this.UserInfo.ClientId);
                return base.FillSearchData(mstKeys, tb);
            }
            else
            {
                string sql = string.Empty;
                int index = 0;
                var udataobj= this.GetUDataNm(this.UserInfo.ClientId, this.UserInfo.U_TBNm, tbnm);
                tb = udataobj.DataTBNm;
                List<LibSearchCondition> conds = new List<LibSearchCondition>();
                foreach (var item in mstKeys)
                {
                    conds.Add(new LibSearchCondition { FieldNm = item.Key, valu1 = item.Value.ToString(), Symbol = SmodalSymbol.Equal });
                }
                sql = SearchConditionHelper.AnalyzeSearchConditionforUData(conds, tbnm, this.UserInfo.ClientId, tb);
                //string aslinm = string.Empty;
                //foreach(var item in mstKeys)
                //{
                //    aslinm = string.Format("v{0}", index);
                //    if (sql.Length <= 0)
                //    {
                //        sql=string .Format("select {1}.FieldNm,{1}.FieldValue, {1}.app_logid from {0} {1} where {1}.FieldNm='{2}' and {1}.FieldValue='{3}' and {1}.TableNm='{4}' and {1}.ClientId='{5}'",tb, aslinm,item.Key ,item .Value,tbnm,this.UserInfo .ClientId);
                //    }
                //    else
                //    {
                //        sql=string .Format("select {1}.FieldNm,{1}.FieldValue, {1}.app_logid from {0} {1} inner join ({2}) {3} on {1}.app_logid={3}.app_logid", tb, aslinm, sql.ToString(), (char)(64 + index));

                //    }
                //    index++;
                //}
                //sql=string.Format("select xx.FieldNm,xx.FieldValue, xx.app_logid from {0} xx inner join({1}) {2} on xx.app_logid={2}.app_logid",tb,sql.ToString (), (char)(64 + index));
                var data = this.dBContext.Database.SqlQuery(sql).ToList<U_Data>();
                return ToDictionCollection(data);
            }
        }

        public List<object> SearchUData(List<LibSearchCondition> conds, string custbnm)
        {
            StringBuilder sql = new StringBuilder();
            //List<object> result = new List<object>();
            var udataobj = this.GetUDataNm(this.UserInfo.ClientId, this.UserInfo.U_TBNm, custbnm);
            string udatatbnm = udataobj.DataTBNm;
            if (conds == null || conds.Count == 0)
            {
                sql.AppendFormat(" EXEC sp_executesql N'select FieldNm,FieldValue,app_logid from {0} where TableNm=@TableNm and ClientId=@ClientId  '", udatatbnm);
                sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15)',");
                sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}'", custbnm, this.UserInfo.ClientId);
            }
            else
            {
                sql.Append(SearchConditionHelper.AnalyzeSearchConditionforUData(conds, custbnm, this.UserInfo.ClientId, udatatbnm));
            }
            var data = this.dBContext.Database.SqlQuery(sql.ToString()).ToList<U_Data>();
            return ToDictionCollection(data);
            //var group = data.GroupBy(i => i.app_logid);
            //foreach (var item in group)
            //{
            //    Dictionary<string, object> row = new Dictionary<string, object>();
            //    foreach (var col in item)
            //    {
            //        row.Add(col.FieldNm, col.FieldValue);
            //    }
            //    row.Add(AppConstManage.applogid, item.Key);
            //    result.Add(row);
            //}
            //return result;
        }

        #region
        private string DoGetTBinfosql(LibClientDataInfo model, string tableNm,string udatanm)
        {
            StringBuilder sql = new StringBuilder();
            U_TableInfo o = model.Datas as U_TableInfo;
            o.DataTBNm = udatanm;
            this.updateData.Add(o);
            if (model.clientDataStatus == LibClientDataStatus.Add)
            {
                o.LibModelStatus = LibModelStatus.Add;
                o.CreateDT = DateTime.Now;
                o.Creater = this.UserInfo.UserNm;
                sql.AppendFormat(" EXEC sp_executesql N'insert into {0}(TableNm,ClientId,TableDesc,DataSourceNm,DataTBNm,TableKind,IsDeleted,CreateDT,Creater,app_logid) ", tableNm);
                sql.Append(" values(@TableNm,@ClientId,@TableDesc,@DataSourceNm,@DataTBNm,@TableKind, @IsDeleted,@CreateDT,@Creater,@app_logid)'");
                sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15),@TableDesc nvarchar(50),@DataSourceNm nvarchar(30),@DataTBNm nvarchar(30),@TableKind int, @IsDeleted bit,@CreateDT datetime,@Creater nvarchar(30),@app_logid nvarchar(50)',");
                sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@TableDesc='{2}',@DataSourceNm='{3}',@DataTBNm='{4}',@IsDeleted='{5}', @CreateDT='{6}', @Creater='{7}', @app_logid='{8}',@TableKind={9}",
                                              o.TableNm, this.UserInfo.ClientId, o.TableDesc, o.DataSourceNm, o.DataTBNm, o.IsDeleted, o.CreateDT.Value.ToString("yyyy-MM-dd HH:mm:ss:fff"), o.Creater, o.app_logid,(int)o.TableKind);
            }
            else if (model.clientDataStatus == LibClientDataStatus.Edit)
            {
                o.LibModelStatus = LibModelStatus.Edit;
                o.LastModifyDT = DateTime.Now;
                o.LastModifier = this.UserInfo.UserNm;
                U_TableInfo old = model.OldDatas as U_TableInfo;
                sql.AppendFormat(" EXEC sp_executesql N'update {0} set TableNm=@TableNm,ClientId=@ClientId,TableDesc=@TableDesc,DataSourceNm=@DataSourceNm,TableKind=@TableKind,LastModifyDT=@LastModifyDT,LastModifier=@LastModifier ", tableNm);
                sql.Append(" where TableNm=@oldTableNm and ClientId=@ClientId '");
                //sql.Append(" values(@TableNm,@ClientId,@TableDesc,@DataSourceNm,@DataTBNm,@IsDeleted,@CreateDT,@Creater,@app_logid)'");
                sql.Append(",N'@TableNm nvarchar(30),@oldTableNm nvarchar(30), @ClientId nvarchar(15),@TableDesc nvarchar(50),@DataSourceNm nvarchar(30),@TableKind int,@LastModifyDT datetime,@LastModifier nvarchar(30)',");
                sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@TableDesc='{2}',@DataSourceNm='{3}',@LastModifyDT='{4}',@LastModifier='{5}',@oldTableNm='{6}',@TableKind={7}",
                                              o.TableNm, this.UserInfo.ClientId, o.TableDesc, o.DataSourceNm, o.LastModifyDT.Value.ToString("yyyy-MM-dd HH:mm:ss:fff"), o.LastModifier, (old == null ? o.TableNm : old.TableNm),(int)o.TableKind);
            }
            return sql.ToString();
        }

        private string DoGetTBFieldinfosql(LibClientDataInfo model, string tableNm)
        {
            StringBuilder sql = new StringBuilder();
            U_TableFieldInfo o = model.Datas as U_TableFieldInfo;
            this.updateData.Add(o);
            if (model.clientDataStatus == LibClientDataStatus.Add)
            {
                o.LibModelStatus = LibModelStatus.Add;
                sql.AppendFormat(" EXEC sp_executesql N'insert into {0}(TableNm,ClientId,FieldNm,FieldDesc,DataType,IsPrimaryKey,DataLength,PointLength,IsDeleted, CreateDT,Creater,app_logid,MaxLength,FromDataSource,FromFieldNm,FromFieldDescNm,IsVirtual)", tableNm);
                sql.Append(" values(@TableNm,@ClientId,@FieldNm,@FieldDesc,@DataType,@IsPrimaryKey,@DataLength,@PointLength, @IsDeleted,@CreateDT,@Creater,@app_logid,@MaxLength,@FromDataSource,@FromFieldNm,@FromFieldDescNm,@IsVirtual)'");
                sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15),@FieldNm nvarchar(30),@FieldDesc nvarchar(50),@DataType int,@IsPrimaryKey bit,@DataLength int,@PointLength int, @IsDeleted bit,@CreateDT datetime,@Creater nvarchar(30),@app_logid nvarchar(50),@MaxLength int,@FromDataSource nvarchar(30),@FromFieldNm nvarchar(30),@FromFieldDescNm nvarchar(30),@IsVirtual bit ',");
                sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@FieldNm='{2}',@FieldDesc='{3}',@DataType={4},@IsPrimaryKey='{5}', @DataLength={6},@PointLength={7}, @IsDeleted='{8}', @CreateDT='{9}', @Creater='{10}', @app_logid='{11}',@MaxLength={12},@FromDataSource='{13}',@FromFieldNm='{14}',@FromFieldDescNm='{15}',@IsVirtual='{16}'",
                                              o.TableNm, this.UserInfo.ClientId, o.FieldNm, o.FieldDesc, (int)o.DataType, o.IsPrimaryKey, o.DataLength, o.PointLength, o.IsDeleted, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), this.UserInfo.UserNm, o.app_logid,o.MaxLength ,o.FromDataSource,o.FromFieldNm,o.FromFieldDescNm,o.IsVirtual);
            }
            else if (model.clientDataStatus == LibClientDataStatus.Edit)
            {
                o.LibModelStatus = LibModelStatus.Edit;
                U_TableFieldInfo old = model.OldDatas as U_TableFieldInfo;
                sql.AppendFormat(" EXEC sp_executesql N'update {0} set TableNm=@TableNm,ClientId=@ClientId,FieldNm=@FieldNm,FieldDesc=@FieldDesc,DataType=@DataType,IsPrimaryKey=@IsPrimaryKey,DataLength=@DataLength,PointLength=@PointLength,LastModifyDT=@LastModifyDT,LastModifier=@LastModifier,MaxLength=@MaxLength,FromDataSource=@FromDataSource,FromFieldNm=@FromFieldNm,FromFieldDescNm=@FromFieldDescNm,IsVirtual=@IsVirtual ", tableNm);
                sql.Append(" where TableNm=@oldTableNm and ClientId=@ClientId and FieldNm=@oldFieldNm '");
                //sql.Append(" values(@TableNm,@ClientId,@FieldNm,@FieldDesc,@DataType,@IsPrimaryKey,@DataLength,@PointLength, @IsDeleted,@CreateDT,@Creater,@app_logid)'");
                sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15),@FieldNm nvarchar(30),@FieldDesc nvarchar(50),@DataType int,@IsPrimaryKey bit,@DataLength int,@PointLength int, @LastModifyDT datetime,@LastModifier nvarchar(30),@oldTableNm  nvarchar(30),@oldFieldNm nvarchar(30),@MaxLength int,@FromDataSource nvarchar(30),@FromFieldNm nvarchar(30),@FromFieldDescNm nvarchar(30),@IsVirtual bit ',");
                sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@FieldNm='{2}',@FieldDesc='{3}',@DataType={4},@IsPrimaryKey='{5}', @DataLength={6},@PointLength={7}, @LastModifyDT='{8}', @LastModifier='{9}',@oldTableNm='{10}',@oldFieldNm='{11}',@MaxLength={12},@FromDataSource='{13}',@FromFieldNm='{14}',@FromFieldDescNm='{15}',@IsVirtual='{16}' ",
                                              o.TableNm, this.UserInfo.ClientId, o.FieldNm, o.FieldDesc, (int)o.DataType, o.IsPrimaryKey, o.DataLength, o.PointLength, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), this.UserInfo.UserNm, (old == null ? o.TableNm : old.TableNm), (old == null ? o.FieldNm : old.FieldNm), o.MaxLength, o.FromDataSource, o.FromFieldNm, o.FromFieldDescNm, o.IsVirtual);
            }
            return sql.ToString();
        }

        private string DoGetUDataSQL(LibClientDataInfo model, string storagetableNm, string custableNm)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> o = model.Datas as Dictionary<string, object>;
            this.updateData.Add(model);
            if (model.clientDataStatus == LibClientDataStatus.Add)
            {
                model.LogAction = 1;
                foreach (var f in o)
                {
                    if (f.Key == AppConstManage.applogid) continue;
                    if (this.U_TableFields.FirstOrDefault(i => i.TableNm == custableNm && i.FieldNm == f.Key && i.IsVirtual) != null) continue;
                    sql.AppendFormat(" EXEC sp_executesql N'insert into {0}(TableNm,ClientId,FieldNm,FieldValue,IsDeleted,CreateDT,Creater,app_logid) ", storagetableNm);
                    sql.Append(" values(@TableNm,@ClientId,@FieldNm,@FieldValue,@IsDeleted,@CreateDT,@Creater,@app_logid)'");
                    sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15),@FieldNm nvarchar(30),@FieldValue nvarchar(max),@IsDeleted bit,@CreateDT datetime,@Creater nvarchar(30),@app_logid nvarchar(50)',");
                    sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@FieldNm='{2}',@FieldValue='{3}',@IsDeleted='{4}', @CreateDT='{5}', @Creater='{6}', @app_logid='{7}'",
                                                 custableNm, this.UserInfo.ClientId, f.Key, f.Value, false, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), this.UserInfo.UserNm, o[AppConstManage.applogid]);
                }
            }
            else if (model.clientDataStatus == LibClientDataStatus.Edit)
            {
                model.LogAction = 2;
                foreach (var f in o)
                {
                    if (f.Key == AppConstManage.applogid) continue;
                    sql.AppendFormat(" EXEC sp_executesql N'update {0} set FieldValue=@FieldValue ", storagetableNm);
                    sql.Append(" where ClientId=@ClientId and TableNm=@TableNm and FieldNm=@FieldNm and app_logid=@app_logid '");
                    sql.Append(",N'@TableNm nvarchar(30),@ClientId nvarchar(15),@FieldNm nvarchar(30),@FieldValue nvarchar(max),@LastModifyDT datetime,@LastModifier nvarchar(30),@app_logid nvarchar(50)',");
                    sql.AppendFormat("  @TableNm='{0}',@ClientId='{1}',@FieldNm='{2}',@FieldValue='{3}', @LastModifyDT='{4}', @LastModifier='{5}', @app_logid='{6}'",
                                                 custableNm, this.UserInfo.ClientId, f.Key, f.Value, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), this.UserInfo.UserNm, o[AppConstManage.applogid]);
                }
            }
            else if (model.clientDataStatus == LibClientDataStatus.Delete)
            {
                
            }
            return sql.ToString();
        }

        private List<object> ToDictionCollection(List<U_Data> data)
        {
            List<object> result = new List<object>();
            var group = data.GroupBy(i => i.app_logid);
            foreach (var item in group)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (var col in item)
                {
                    row.Add(col.FieldNm, col.FieldValue);
                }
                row.Add(AppConstManage.applogid, item.Key);
                result.Add(row);
            }
            return result;
        }

        private string GetUdataTableNm(string dynamictbnm)
        {
            LibDbParameter[] parameters = new LibDbParameter[2];
            parameters[0] = new LibDbParameter { ParameterNm = "@dynamicTableNm", DbType = DbType.String, Value = dynamictbnm };
            parameters[1] = new LibDbParameter { ParameterNm = "@storagetbnm", DbType = DbType.String, Size = 35, Direction = ParameterDirection.Output, Value = string.Empty };
           this.dBContext.Database.ExeStoredProcedure("p_AddUDataTable", parameters);
            return parameters[1].Value.ToString();
        }

        //private string DoCreatesql(object model, string tableNm)
        //{

        //}

        //private string DoGetTableFieldSql(string tbnm)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.Append(" EXEC sp_executesql N'select [TableNm],[ClientId],[FieldNm],[FieldDesc],[DataType],[IsPrimaryKey],[DataLength],[PointLength],[IsDeleted],[CreateDT],[Creater],[LastModifyDT],[LastModifier],[app_logid] '");
        //    sql.AppendFormat(" from {0}", tbnm);

        //}
        #endregion 
    }
}
