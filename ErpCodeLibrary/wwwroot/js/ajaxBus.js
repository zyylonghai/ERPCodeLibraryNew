var _ajax = $.ajax;
var sdp_haserror = false;
//重写jquery的ajax方法
$.ajax = function (opt) {
    var fn = {
        data: "",
        url:"",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
           
        },
        success: function (data, textStatus)
        {
        },
        complete: function (data) { },
        beforeSend: function (xhr, o) { },
    }
    if (opt.error) {
        fn.error = opt.error;
    }
    if (opt.success) {
        fn.success = opt.success;
    }
    if (opt.complete) {
        fn.complete = opt.complete;
    }
    if (opt.beforeSend) {
        fn.beforeSend = opt.beforeSend;
    }
    //if (opt.data)
    //{
    fn.data = opt.data;
    fn.url = opt.url;
    //fn.url += "/" + $('#app_progNm').val();
    //if (fn.url.indexOf("?") != -1) {
    //    var query = fn.url.split('?')[1];
    //    var vars = query.split("&");
    //    for (var i = 0; i < vars.length; i++) {
    //        var pair = vars[i].split("=");
    //        if (pair[0].trim() == "gridid") {
    //            fn.data.gridid = pair[1];
    //        }
    //        else if (pair[0].trim() == "ds") {
    //            fn.data.ds = pair[1];
    //        }
    //        else if (pair[0].trim() == "tbnm") {
    //            fn.data.tbnm = pair[1];
    //        }
    //    }
    //    fn.url = fn.url.replace(query, '');
    //    fn.url = fn.url.replace("?", '');
    //}
    let routs = fn.url.split('/');
    if ((routs.length > 0 && routs[1] != "AppSys" && routs[1] != "js" && routs[1] != "pages") || (routs[1] == "AppSys" && routs.length >= 4))
        fn.url += "/" + $('#app_progNm').val();
    if (Object.prototype.toString.call(fn.data) != '[object String]') {
        if (Object.prototype.toString.call(fn.data) == '[object FormData]') {
            //fn.data.append("sdp_pageid", $('#bwysdp_progid').val());
            //fn.data.append("sdp_dsid", $('#bwysdp_dsid').val());
        }
        else {
            if (fn.data != undefined && fn.data != null) {
                fn.data.progNm = $('#app_progNm').val();
            }
            //fn.data.sdp_pageid = $('#bwysdp_progid').val();
            //fn.data.sdp_dsid = $('#bwysdp_dsid').val();
        }
    }
    else {
        //let jsonobj = JSON.parse(opt.data);
        //fn.data += "&progNm=" + $('#app_progNm').val() + "";
        //fn.data += "&sdp_dsid=" + $('#bwysdp_dsid').val() + "";
    }
    //扩展增强处理 
    var _opt = $.extend(opt, {
        data: fn.data,
        url: fn.url,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.responseJSON == undefined && XMLHttpRequest.status == 401) {
                RedirectLogin();
            }
            //icon值 1:打钩的绿色图标，2：打叉的红色图标，3：问号黄色图标，4：锁灰色图标，
            //       5：哭脸红色图标，6：笑脸绿色图标，7：黄色感叹号图标，
            layer.msg('您的请求出现异常,信息：' + XMLHttpRequest.responseJSON.message,
                { icon: 2, time: 20000, shade: [0.3, '#000', true], btn: ['确定']});
            //错误方法增强处理 
            fn.error(XMLHttpRequest, textStatus, errorThrown);
        },
        success: function (data, textStatus) {
            //成功回调方法增强处理 
            if (data.IsSuccess != null && data.IsSuccess != undefined) {
                if (data.IsSuccess&& data.showMessageBox) {
                    layer.msg(data.Message, { icon: (data.IsSuccess ? 1 : 2), time: 5000 });
                }
                if (!data.IsSuccess) {
                    layer.msg(data.Message, { icon: (data.IsSuccess ? 1 : 2), time: 20000, offset: '2px' });
                }
            }
            fn.success(data, textStatus);
            //layer.msg('您的请求成功了', { icon: 7, time: 5000, shade: [0.6, '#000', true] });

        },
        complete: function (data) {
            fn.complete(data);
            //hideMask();
        },
        beforeSend: function (xhr, o) {
            //showMask();
            let token = getCookie("apptick");
            xhr.setRequestHeader("Authorization", 'Bearer ' +token);
            fn.beforeSend(xhr, o);

        },
    });
    return _ajax(_opt).done(function (e) {
        //hideMask();
    });
}