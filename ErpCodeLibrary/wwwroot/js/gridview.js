function appgrid(id) {
    this.$table = {
        ElemtableID: id,
        url: "",
        data:[],
        toolbarid: "",
        title: "",
        page: true,
        totalRow: false,//是否开启合计行区域
        width: -1,
        height: undefined,
        limit: 10,//每页显示的条数（默认：10）
        limits: [10, 20, 30, 40, 50, 60, 70, 80, 90],
        hasSelectcol: true,//是否有选择列
        isSimple: true,//是否单选
        columns: [],
        where: {}
    };
    this.column = function (fieldid,title) {
        this.field = fieldid,
            this.title = title,
            this.width = 120,
            this.minWidth = 60,//单元格的最小宽度（默认：60）
            this.type = "",//设定列类型。可选值有：normal（常规列，无需设定）checkbox（复选框列）radio（单选框列，layui 2.4.0 新增）numbers（序号列）space（空列）
            this.fixed = "",//固定列。可选值有：left（固定在左）、right（固定在右）
            this.hide = false,//是否初始隐藏列，默认：false
            this.totalRow = false,//是否开启该列的自动合计功能，默认：false。
            this.totalRowText = "",//用于显示自定义的合计文本
            this.sort = false,//是否允许排序（默认：false）
            this.unresize = false,//是否禁用拖拽列宽（默认：false）
            this.edit = "",//单元格编辑类型（默认不开启）目前只支持：text（输入框）
            this.event = "",//自定义单元格点击事件名，以便在 tool 事件中完成对该单元格的业务处理
            this.style = "",//自定义单元格样式。即传入 CSS 样式
            this.align = "",//单元格排列方式。可选值有：left（默认）、center（居中）、right（居右）
            this.templet = "",//自定义列模板，模板遵循 laytpl 语法
            this.toolbar = "",//绑定工具条模板。
            this.templet//自定义列模板 模板遵循 laytpl 语法
    };
    this.$fields = [];
}
appgrid.prototype = {
    constructor: appgrid,
    libtable: null,
    initialTable: function (toolevent) {
        let othis = this;
        if (othis.$table.hasSelectcol) {
            let operatcol = new othis.column(null, null);
            operatcol.fixed = "left";
            if (othis.$table.isSimple) {
                operatcol.type = "radio";
            }
            else {
                operatcol.type = "checkbox";
            }
            othis.$table.columns.unshift(operatcol);
        }
        layui.use('table', function () {
            othis.libtable = layui.table;
            if (othis.$table.url != "" && othis.$table.url != undefined) {
                othis.libtable.render({
                    elem: '#' + othis.$table.ElemtableID
                    , url: othis.$table.url
                    , toolbar: othis.$table.toolbarid == '' ? true : '#' + othis.$table.toolbarid
                    , title: othis.$table.title
                    , totalRow: othis.$table.totalRow
                    , cols: [othis.$table.columns]
                    , page: othis.$table.page
                    , height: othis.$table.height
                    , limit: othis.$table.limit
                    , where: othis.$table.where
                    , done: function (res, curr, count) {
                        othis.DataLoad(res, curr, count);
                    }
                });
            }
            else {
                othis.libtable.render({
                    elem: '#' + othis.$table.ElemtableID
                    , data: othis.$table.data
                    , toolbar: othis.$table.toolbarid == '' ? true : '#' + othis.$table.toolbarid
                    , title: othis.$table.title
                    , totalRow: othis.$table.totalRow
                    , cols: [othis.$table.columns]
                    , page: othis.$table.page
                    , height: othis.$table.height
                    , limit: othis.$table.limit
                    , where: othis.$table.where
                    , done: function (res, curr, count) {
                        othis.DataLoad(res, curr, count);
                    }
                });
            }

            //工具栏事件
            othis.libtable.on('toolbar(' + othis.$table.ElemtableID+')', function (obj) {
                var checkStatus = othis.libtable.checkStatus(obj.config.id);
                if (toolevent != null && toolevent != undefined) {
                    toolevent(checkStatus, obj.event);
                }
                //switch (obj.event) {
                //    case 'getCheckData':
                //        var data = checkStatus.data;
                //        layer.alert(JSON.stringify(data));
                //        break;
                //    case 'getCheckLength':
                //        var data = checkStatus.data;
                //        layer.msg('选中了：' + data.length + ' 个');
                //        break;
                //    case 'isAll':
                //        layer.msg(checkStatus.isAll ? '全选' : '未全选')
                //        break;
                //};
            });
            othis.libtable.on('rowDouble(' + othis.$table.ElemtableID + ')', function (row) {
                othis.DbClick(row);
            });
        });

    },
    refreshData: function () {
        if (this.libtable == null || this.libtable == undefined) this.libtable = layui.table;
        //this.initialTable();
        //this.ReSetOptions();
        this.libtable.reload(this.$table.ElemtableID);
    },
    getSelectData: function () {
        if (this.libtable == null || this.libtable == undefined) this.libtable = layui.table;
        return this.libtable.checkStatus(this.$table.ElemtableID);
        
    },
    DbClick: function (row) {
        //this.libtable.on('rowDouble(' + this.$table.ElemtableID + ')', function (row) {
        //    func(row);
        //});
    },
    DataLoad: function (res, curr, count) {

    },
    ReSetOptions: function () {
        let othis = this;
        //layui.use('table', function () {
        //    othis.libtable = layui.table;
            if (othis.$table.url != "" && othis.$table.url != undefined) {
                othis.libtable.render({
                    elem: '#' + othis.$table.ElemtableID
                    , url: othis.$table.url
                    , toolbar: othis.$table.toolbarid == '' ? true : '#' + othis.$table.toolbarid
                    , title: othis.$table.title
                    , totalRow: othis.$table.totalRow
                    , cols: [othis.$table.columns]
                    , page: othis.$table.page
                    , height: othis.$table.height
                    , limit: othis.$table.limit
                    , where: othis.$table.where
                    , done: function (res, curr, count) {
                        othis.DataLoad(res, curr, count);
                    }
                });
            }
            else {
                othis.libtable.render({
                    elem: '#' + othis.$table.ElemtableID
                    , data: othis.$table.data
                    , toolbar: othis.$table.toolbarid == '' ? true : '#' + othis.$table.toolbarid
                    , title: othis.$table.title
                    , totalRow: othis.$table.totalRow
                    , cols: [othis.$table.columns]
                    , page: othis.$table.page
                    , height: othis.$table.height
                    , limit: othis.$table.limit
                    , where: othis.$table.where
                    , done: function (res, curr, count) {
                        othis.DataLoad(res, curr, count);
                    }
                });
            }
            othis.libtable.on('rowDouble(' + othis.$table.ElemtableID + ')', function (row) {
                othis.DbClick(row);
            });
        //});
    }
}

function grid_add(obj) {
    let gridid = $(obj).attr("data-gridid");
    let ds = $(obj).attr("data-ds");
    let tbnm = $(obj).attr("data-tbnm");
    let currgd, title = tbnm + '新增';
    let ctrobj = { ID: gridid + "_tablemodel", TableNm: tbnm, DataSource:ds, Title: '关键信息' };
    let result = -1;
    let rowdata;
    grids.forEach(function (gd) {
        if (gd.$table.ElemtableID == gridid + "_gridvm") {
            currgd = gd;
        }
    });
    if (currgd != null && currgd != undefined && currgd.$fields != undefined && currgd.$fields != null) {
        renderModeldialoghtml(currgd, ctrobj);
        form.on('submit(' + ctrobj.ID + "_btnsubmit" + ')', function (data) {
            let c = new LibClientDatas();
            c.TableNm = tbnm;
            c.DataSource = ds;
            let isexist = false;
            $.each(data.field, function (n, v) {
                if (rowdata[n] != undefined) {
                    rowdata[n] = v;
                }
            });
            let clientdatainfo = new LibClientDataInfo();
            clientdatainfo.clientDataStatus = 1;
            clientdatainfo.Datas = rowdata;
            c.ClientDataInfos.push(clientdatainfo);
            $.ajax({
                type: 'post',
                url: getUrl(proginfo.progPackage, proginfo.controllerNm, "TableAction"),
                data: JSON.stringify(c),
                success: function (respon) {
                    let data = respon.ClientDatas;
                    currgd.refreshData();
                },
                error: function (msg) {
                    //alert(msg.responseText);
                }
            });
            layer.closeAll();
            return false;
        });

        let clidata = new LibClientDatas();
        clidata.TableNm = tbnm;
        clidata.DataSource = ds;

        let clientdatainfo = new LibClientDataInfo();
        clientdatainfo.clientDataStatus = 1;
        clientdatainfo.Datas = form.val(ctrobj.ID + "_" + ctrobj.TableNm);
        clidata.ClientDataInfos.push(clientdatainfo);
        $.ajax({
            type: 'post',
            url: getUrl(proginfo.progPackage, proginfo.controllerNm, "GetTableAction"),
            data: JSON.stringify(clidata),
            success: function (respon) {
                let data = respon.ClientDatas;
                rowdata = data[0].ClientDataInfos[0].Datas;
                form.val(ctrobj.ID + "_" + ctrobj.TableNm, JSON.parse(JSON.stringify(data[0].ClientDataInfos[0].Datas)));
            },
            error: function (msg) {
            }
        });
    }
    appComfirmBox(title, 'appgrid_tableModel', true, function () {
        let formbtn = ctrobj.ID + "_btnsubmit";
        $('#' + formbtn).trigger("click");
        return false;
    });
    return false;
}

function grid_edit(obj) {
    let gridid = $(obj).attr("data-gridid");
    let ds = $(obj).attr("data-ds");
    let tbnm = $(obj).attr("data-tbnm");
    let currgd, title = tbnm + '编辑';
    let ctrobj = { ID: gridid + "_tablemodel", TableNm: tbnm, DataSource: ds, Title: '关键信息' };
    grids.forEach(function (gd) {
        if (gd.$table.ElemtableID == gridid + "_gridvm") {
            currgd = gd;
        }
    });
    var editdata = currgd.getSelectData();
    if (editdata == null || editdata.data == null || editdata == undefined || editdata.data == undefined || editdata.data.length==0) {
        appMessageBox("请先选择一行", 5);
        return false;
    }
    if (editdata.data.length > 1) {
        appMessageBox("只能选择一行进行编辑", 5);
        return false;
    }
    appComfirmBox(title, 'appgrid_tableModel', true, function () {
        let formbtn = ctrobj.ID + "_btnsubmit";
        $('#' + formbtn).trigger("click");
        return false;
    });
    if (currgd != null && currgd != undefined && currgd.$fields != undefined && currgd.$fields != null) {
        renderModeldialoghtml(currgd, ctrobj);
        form.val(ctrobj.ID + "_" + ctrobj.TableNm);
        form.val(ctrobj.ID + "_" + ctrobj.TableNm, JSON.parse(JSON.stringify(editdata.data[0])));

        form.on('submit(' + ctrobj.ID + "_btnsubmit" + ')', function (data) {
            let c = new LibClientDatas();
            c.TableNm = tbnm;
            c.DataSource = ds;

            let clientdatainfo = new LibClientDataInfo();
            clientdatainfo.clientDataStatus =2;
            clientdatainfo.Datas = data.field;
            c.ClientDataInfos.push(clientdatainfo);

            $.ajax({
                type: 'post',
                url: getUrl(proginfo.progPackage, proginfo.controllerNm, "TableAction"),
                data: JSON.stringify(c),
                success: function (respon) {
                    //let data = respon.clientDatas;
                    currgd.refreshData();
                },
                error: function (msg) {
                    //alert(msg.responseText);
                }
            });
            layer.closeAll();
            return false;
        });
    }
    return false;
}
function grid_delete(obj) {
    let gridid = $(obj).attr("data-gridid");
    let ds = $(obj).attr("data-ds");
    let tbnm = $(obj).attr("data-tbnm");
    let currgd, title = tbnm + '行删除';
    let ctrobj = { ID: gridid + "_tablemodel", TableNm: tbnm, DataSource: ds, Title: '' };
    grids.forEach(function (gd) {
        if (gd.$table.ElemtableID == gridid + "_gridvm") {
            currgd = gd;
        }
    });
    var editdata = currgd.getSelectData();
    if (editdata == null || editdata.data == null || editdata == undefined || editdata.data == undefined || editdata.data.length == 0) {
        appMessageBox("请先选择行", 5);
        return false;
    }
    appMessageComfirmBox(title, 'appgrid_deletmsg', function () {
        let c = new LibClientDatas();
        c.TableNm = tbnm;
        c.DataSource = ds;
        $.each(editdata.data, function (i, o) {
            let clientdatainfo = new LibClientDataInfo();
            clientdatainfo.clientDataStatus = 3;
            clientdatainfo.Datas = o;
            c.ClientDataInfos.push(clientdatainfo);
        });
        $.ajax({
            type: 'post',
            url: getUrl(proginfo.progPackage, proginfo.controllerNm, "TableAction"),
            data: JSON.stringify(c),
            success: function (respon) {
                //let data = respon.clientDatas;
                currgd.refreshData();
            },
            error: function (msg) {
                //alert(msg.responseText);
            }
        });
        return true;
    });
    //appMessageBox('功能正在研发中。。。。', 6);
    return false;
}
function grid_copy(obj) {
    appMessageBox('功能正在研发中。。。。', 6);
    return false;
}

function renderModeldialoghtml(currgd, ctrobj){
    let dtfields = [];
    let dttimes = [];
    let events = [];
    let appchkboxs = [], appsearchfields = [];
    $('#appgrid_tableModel').html("");
    RenderTplHtml('collaTpl', 'appgrid_tableModel', ctrobj, true);
    currgd.$fields.forEach(function (f) {
        if (!f.IsHide) {
            //let s = ""; s.substr(0, 1).toLowerCase()
            //f.Field = f.field.substr(0, 1).toLowerCase() + f.field.substr(1);
            if (f.ElemType == 1) {//singleinputTpl
                RenderTplHtml('singleinputTpl', ctrobj.ID, f, true);
            }
            else if (f.ElemType == 2) {//MultinputTpl
                RenderTplHtml('MultinputTpl', ctrobj.ID, f, true);
            }
            else if (f.ElemType == 4 || f.ElemType == 5) {//dateTpl
                RenderTplHtml('dateTpl', ctrobj.ID, f, true);
                if (f.ElemType == 4)
                    dtfields.push(f.ID + "_" + f.Field);
                else if (f.ElemType == 5) {
                    dttimes.push(f.ID + "_" + f.Field);
                }
            }
            else if (f.ElemType == 3) {//selectTpl
                RenderTplHtml('selectTpl', ctrobj.ID, f, true);
                form.render('select');
                getFieldOptions(f.ID, ctrobj.DataSource, ctrobj.TableNm, f.Field, function () {
                    let editdata = currgd.getSelectData();
                    $.each(editdata.data[0], function (n, v) {
                        if (n == f.Field) {
                            let d = {};
                            d[n] = v;
                            form.val(ctrobj.ID + "_" + ctrobj.TableNm,d);
                        }
                    });
                   
                });
            }
            else if (f.ElemType == 8) {//imgtpl
                RenderTplHtml('imgtpl', ctrobj.ID, f, true);
            }
            else if (f.ElemType == 10) {//searchinputTpl
                RenderTplHtml('searchinputTpl', ctrobj.ID, f, true);
                appsearchfields.push(f);
            }
            else if (f.ElemType == 6) {//checkboxTpl
                RenderTplHtml('checkboxTpl', ctrobj.ID, f, true);
                appchkboxs.push(f.Field);
            }

            //事件处理
            let esource = new LibEventSource();
            esource.ControlID = f.ID;
            esource.FieldNm = f.Field;
            escape.ElemType = f.ElemType;
            let handle = new LibEventHandle();
            handle.EventSource = esource;
            if (f.Onchange) {
                handle.EventType = 1;
                events.push(handle);
            }
            if (f.Onclick) {
                handle.EventType = 2;
                events.push(handle);
            }
            if (f.Onfocus) {
                handle.EventType = 3;
                events.push(handle);
            }

        }
    });
    RenderTplHtml('applogidTpl', ctrobj.ID, { Field: applogid}, true);
    let ctrlelem = document.getElementById(ctrobj.ID);
    ctrlelem.innerHTML = "<div class=\"layui-form-item\">" + ctrlelem.innerHTML + "</div>";
    dtfields.forEach(function (dt) {
        layui.laydate.render({
            elem: '#' + dt
            , format: 'yyyy-MM-dd'
        });
    });
    dttimes.forEach(function (dt) {
        layui.laydate.render({
            elem: '#' + dt
            , type: 'datetime'
            , format: 'yyyy-MM-dd HH:mm:ss'
        });
    });
    element.init();
}