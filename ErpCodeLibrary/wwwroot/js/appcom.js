var laytpl, form, element, layer, transfer, util;
var applogid = "app_logid";
//var form;
//var element;
//var layer;
function InitialLayui() {
    
}
function $load(func) {
    layui.use(['element', 'layer', 'laytpl', 'form', 'layedit', 'transfer', 'laydate', 'util'], function () {
        element = layui.element;
        layer = layui.layer;
        laytpl = layui.laytpl;
        form = layui.form;
        transfer = layui.transfer;
        //tree = layui.tree;
        util = layui.util;
        func();
    });
}
function RenderTplHtml(tplid, elemid,data,isappend) {
    var gettpl = document.getElementById(tplid).innerHTML;
    let elemobj = document.getElementById(elemid);
    DoRenderTplHtml(tplid, elemobj, data, isappend);
    //laytpl(gettpl).render(data, function (html) {
    //    //得到的模板渲染到html
    //    if (isappend)
    //        document.getElementById(elemid).innerHTML += html;
    //    else {
    //        document.getElementById(elemid).innerHTML = html;
    //    }
    //});
    //element.init();
}
function DoRenderTplHtml(tplid, elemobj, data, isappend) {
    var gettpl = document.getElementById(tplid).innerHTML;

    laytpl(gettpl).render(data, function (html) {
        //得到的模板渲染到html
        if (isappend)
            elemobj.innerHTML += html;
        else {
            elemobj.innerHTML = html;
        }
    });
    element.init();
}
function appComfirmBox(title, contentid,hasmax, okfunc,successfunc) {
    var windwidth = $(window).width(), windheight = $(window).height();
    var layerwidth = windwidth - 350;
    var index= layer.open({
        type: 1
        , title: title
        , area: ['' + layerwidth + 'px', '500px']
        , maxmin: hasmax
        , offset: 'auto' //具体配置参考：http://www.layui.com/doc/modules/layer.html#offset
        , id: 'cmfbox_' + contentid //防止重复弹出
        , content: $('#' + contentid)
        //, btnAlign: 'c' //按钮居中
        , shade: 0 //不显示遮罩
        , btn: ['确定', '取消'] //只是为了演示
        , zIndex: layer.zIndex //重点1
        , yes: function (index, layero) {
            let result = true;
            if (okfunc != null && okfunc != undefined)
                result = okfunc();
            if (result || result == undefined || result == null)
                layer.close(index);
        }
        , success: function (layero, index) {
            if (successfunc != null && successfunc != undefined)
                successfunc();
        }
    });
    return index;
}
function appMessageComfirmBox(title, contentid, okfunc, successfunc) {
    var index = layer.open({
        type: 1
        , title: title
        //, area: ['' + layerwidth + 'px', '500px']
        //, maxmin: hasmax
        , offset: 'auto' //具体配置参考：http://www.layui.com/doc/modules/layer.html#offset
        , id: 'cmfbox_' + contentid //防止重复弹出
        , content: $('#' + contentid)
        //, btnAlign: 'c' //按钮居中
        , shade: 0 //不显示遮罩
        , btn: ['确定', '取消'] //只是为了演示
        , zIndex: layer.zIndex //重点1
        , yes: function (index, layero) {
            let result = true;
            if (okfunc != null && okfunc != undefined)
                result = okfunc();
            if (result || result == undefined || result == null)
                layer.close(index);
        }
        , success: function (layero, index) {
            if (successfunc != null && successfunc != undefined)
                successfunc();
        }
    });
    return index;
}

function appMessageBox(msg, iconval) {
     //icon值 1:打钩的绿色图标，2：打叉的红色图标，3：问号黄色图标，4：锁灰色图标，
     //       5：哭脸红色图标，6：笑脸绿色图标，7：黄色感叹号图标，
    layer.msg(msg, { icon: iconval, time: 3000});
}

function getQueryVariable(param) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == param) { return pair[1]; }
    }
    return null;
}

function getUrl(pkg, ctrlNm, action) {
    let url;
    if (ctrlNm == null || ctrlNm == undefined || ctrlNm == '') {
        url = "/" + pkg + "/" + action;
    }
    else {
        url = "/" + pkg + "/" + ctrlNm + "/" + action;
    }
    return url;
}

function RedirectLogin() {
    window.location.href = "/pages/Author/Loginpage.html";
}
function RedirectUrl(url) {
    window.location.href = url;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

function ShowImgFile(obj) {
    let id = $(obj).attr("data-id");
    $('#' + id).click();
}

function LoadImgToUI(obj) {
    var imgRead = new FileReader();
    var imgfilter = ""
    var imgfile = $('#' + obj.id).get(0).files[0];
    imgRead.readAsDataURL(imgfile);
    imgRead.onload = function (et) {
        $('#app_img_' + obj.id).attr("src", et.target.result);
    }
}

function SetPageDisabled() {
    DoSetPageStatus(true);
}
function RemovePageDisabled() {
    DoSetPageStatus(false);
}

function DoSetPageStatus(ispreview) {
    $('#apppagecontent').find('input').each(function (i, o) {
        dosetElementdisable(o, ispreview);
        //$(o).attr("disabled", ispreview);
        //if (ispreview) {
        //    $(o).addClass("layui-disabled");
        //}
        //else {
        //    $(o).removeClass("layui-disabled");
        //}
    });
    $('#apppagecontent').find('textarea').each(function (i, o) {
        dosetElementdisable(o, ispreview);
        //$(o).attr("disabled", ispreview);
        //if (ispreview) {
        //    $(o).addClass("layui-disabled");
        //}
        //else {
        //    $(o).removeClass("layui-disabled");
        //}
    });
    $('#apppagecontent').find('select').each(function (i, o) {
        dosetElementdisable(o, ispreview);
        form.render('select');
        //$(o).attr("disabled", ispreview);
        //$(o).addClass("layui-disabled");
    });
    $('#apppagecontent').find('a').each(function (i, o) {
        $(o).attr("disabled", ispreview);
    });
    $('#apppagecontent').find('button').each(function (i, o) {
        dosetElementdisable(o, ispreview);
        //$(o).attr("disabled", ispreview);
        //$(o).addClass("layui-disabled");
    });
}
function dosetElementdisable(o, ispreview) {
    $(o).attr("disabled", ispreview);
    if (ispreview) {
        $(o).addClass("layui-disabled");
    }
    else {
        $(o).removeClass("layui-disabled");
    }
}

function AppGetFieldDesc(prognm, tbnm, fieldnm) {
    let v;
    $.ajax({
        type: 'get',
        url: '/AppSys/GetFieldDesc',
        data: { prognm: prognm, tbnm: tbnm, fieldnm: fieldnm },
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        success: function (returndata) {
            v = returndata.Data;
        },
        error: function (msg) {
             
        }
    });
    return v;
}

function loadScript(url, callback) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    if (typeof (callback) != "undefined") {
        if (script.readyState) {
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            };
        } else {
            script.onload = function () {
                callback();
            };
        }
    };
    script.src = url;
    document.body.appendChild(script);
}
function LoadPage(url, func) //加载局部html页面。 url为任意服务器文件名
{
    var xhr = new XMLHttpRequest();
    xhr.open("GET", url, true); //异步模式
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 304)) { //304代表页面无修改可以使用本地缓存
            func(xhr.responseText); //自定义func函数来处理接收的内容
        }
    };
    xhr.send(null);
}

function LoadGridPart(prognm,ctrlid, elemid, func) {
    let url = "appcontrols/_gridControl.html?prog=" + prognm + "";
    var xhr = new XMLHttpRequest();
    xhr.open("GET", url, true); //异步模式
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 304)) { //304代表页面无修改可以使用本地缓存
            $('#' + elemid).append('<input type="hidden" id="_gridprognm" value="' + prognm + '"/>');
            $('#' + elemid).append('<input type="hidden" id="_elemId" value="' + elemid + '"/>');
            $('#' + elemid).append('<input type="hidden" id="_ctrlid" value="' + ctrlid + '"/>');
            $('#' + elemid).append(xhr.responseText);
            func(xhr.responseText); //自定义func函数来处理接收的内容
        }
    };
    xhr.send(prognm);
}


function getBracketStr(text) {
    let result = ''
    if (isObjEmpty(text))
        return result;
    let regex = /\{(.+?)\}/g;
    let options = text.match(regex)
    if (!isObjEmpty(options)) {
        let option = options[0]
        if (!isObjEmpty(option)) {
            result = option.substring(1, option.length - 1);
        }
    }
    return result;
}
function getsmallBracketStr(text) {
    let result = ''
    if (isObjEmpty(text))
        return result;
    let regex = /\((.+?)\)/g;
    let options = text.match(regex)
    if (!isObjEmpty(options)) {
        let option = options[0]
        if (!isObjEmpty(option)) {
            result = option.substring(1, option.length - 1);
        }
    }
    return result;
}
function isObjEmpty(text) {
    if (text == undefined || text == '')
        return true;
    return false;
}

function PrintPreview() {
    //if (fang < 10) {
    //ClearSelected();
    bdhtml = window.document.body.innerHTML;//获取当前页的html代码
    sprnstr = "<!--startprint-->";//设置打印开始区域
    eprnstr = "<!--endprint-->";//设置打印结束区域
    prnhtml = bdhtml.substring(bdhtml.indexOf(sprnstr) + 18); //从开始代码向后取html
    prnhtml = prnhtml.substring(0, prnhtml.indexOf(eprnstr));//从结束代码向前取html
    window.document.body.innerHTML = prnhtml;
    window.print();
    window.document.body.innerHTML = bdhtml;
}
//function dynamicLoadProgPage(prognm,width,height) {
//    var ifrm = document.createElement("iframe");
//    ifrm.setAttribute("src", "../../pages/funcpage.html?prog=" + prognm + "");
//    if (width == undefined || width == 0 || width == null || width=="100%")
//        ifrm.style.width = "100%";
//    else
//        ifrm.style.width = width + "px";
//    ifrm.style.height = height+"px";
//    ifrm.frameBorder = "0px;"
//    ifrm.scrollIntoView(true);
//    //ifrm.onload = function (e) {
//    //    ifrm.contentWindow.document.getElementById("appfuncbtngroups").remove();
//    //}

//    return ifrm;
//    //document.body.appendChild(ifrm); 
//}

