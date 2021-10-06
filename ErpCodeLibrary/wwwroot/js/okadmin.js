
var objOkTab = "";
layui.use(["element", "layer", "okUtils", "okTab", "okLayer", "jQContextMenu"], function () {
  var okUtils = layui.okUtils;
  var $ = layui.jquery;
  var layer = layui.layer;
  var okLayer = layui.okLayer;

  var okTab = layui.okTab({
    // 菜单请求路径
    url: "data/navs.json",
    // 允许同时选项卡的个数
    openTabNum: 30,
    // 如果返回的结果和navs.json中的数据结构一致可省略这个方法
    parseData: function (data) {
      return data;
    }
  });
  objOkTab = okTab;

  /**
   * 左侧导航渲染完成之后的操作
   */
  okTab.render(function () {
    /**tab栏的鼠标右键事件**/
    $("body .ok-tab").contextMenu({
      width: 'auto',
      itemHeight: 30,
      menu: [
        {
          text: "定位所在页",
          icon: "ok-icon ok-icon-location",
          callback: function () {
            okTab.positionTab();
          }
        },
        {
          text: "关闭当前页",
          icon: "ok-icon ok-icon-roundclose",
          callback: function () {
            okTab.tabClose(1);
          }
        },
        {
          text: "关闭其他页",
          icon: "ok-icon ok-icon-roundclose",
          callback: function () {
            okTab.tabClose(2);
          }
        },
        {
          text: "关闭所有页",
          icon: "ok-icon ok-icon-roundclose",
          callback: function () {

            okTab.tabClose(3);
          }
        }
      ]
    });
  });

  /**
   * 添加新窗口
   */
  $("body").on("click", "#navBar .layui-nav-item a, #userInfo a", function () {
    // 如果不存在子级
    if ($(this).siblings().length == 0) {
      okTab.tabAdd($(this));
    }
    // 关闭其他展开的二级标签
    $(this).parent("li").siblings().removeClass("layui-nav-itemed");
    if (!$(this).attr("lay-id")) {
      var topLevelEle = $(this).parents("li.layui-nav-item");
      var childs = $("#navBar > li > dl.layui-nav-child").not(topLevelEle.children("dl.layui-nav-child"));
      childs.removeAttr("style");
    }
  });

  /**
   * 左侧菜单展开动画
   */
  $("#navBar").on("click", ".layui-nav-item a", function () {
    if (!$(this).attr("lay-id")) {
      var superEle = $(this).parent();
      var ele = $(this).next('.layui-nav-child');
      var height = ele.height();
      ele.css({"display": "block"});
      // 是否是展开状态
      if (superEle.is(".layui-nav-itemed")) {
        ele.height(0);
        ele.animate({height: height + "px"}, function () {
          ele.css({height: "auto"});
        });
      } else {
        ele.animate({height: 0}, function () {
          ele.removeAttr("style");
        });
      }
    }
  });

  /**
   * 左边菜单显隐功能
   */
  $(".ok-menu").click(function () {
    $(".layui-layout-admin").toggleClass("ok-left-hide");
    $(this).find("i").toggleClass("ok-menu-hide");
    localStorage.setItem("isResize", false);
    setTimeout(function () {
      localStorage.setItem("isResize", true);
    }, 1200);
  });

  /**
   * 移动端的处理事件
   */
  $("body").on("click", ".layui-layout-admin .ok-left a[data-url], .ok-make", function () {
    if ($(".layui-layout-admin").hasClass("ok-left-hide")) {
      $(".layui-layout-admin").removeClass("ok-left-hide");
      $(".ok-menu").find('i').removeClass("ok-menu-hide");
    }
  });

  /**
   * tab左右移动
   */
  $("body").on("click", ".okNavMove", function () {
    var moveId = $(this).attr("data-id");
    var that = this;
    okTab.navMove(moveId, that);
  });

  /**
   * 刷新当前tab页
   */
    $("body").on("click", ".ok-refresh", function () {
        var prog = $('#appprognm').val().toUpperCase();
        if (prog == "" || prog == undefined) return;
        let token = getCookie("apptick");
        $.ajax({
            type: "Get",
            url: 'SYS/GetUserMenuByUserCode/sys',
            data: { code: prog },
            dataType: "Json",
            beforeSend: function (request) {
                let o = { Parameter: token, Scheme: "Bearer" };
                request.setRequestHeader("Authorization", "Bearer " + token);
            },
            success: function (respon) {
                if (respon.IsSuccess) {
                    //if (respon.Data != null && respon.Data != undefined && respon.Data.ProgNm != '') {
                        //prog = respon.Data.ProgNm.toUpperCase();
                    //}
                    prog = respon.Data.ProgNm.toUpperCase();
                    var a = document.createElement("a");
                    a.setAttribute("lay-id", prog);
                    a.setAttribute("data-url", "pages/funcpage.html?prog=" + prog + "");
                    //a.setAttribute("class", "ok-refresh");

                    var i = document.createElement("i");
                    i.setAttribute("class", "layui-icon");
                    a.append(i);

                    var cite = document.createElement("cite");
                    cite.textContent = respon.Data.MenuName;
                    a.append(cite);

                    okTab.tabAdd($(a));
                }
            }
        });
        //okTab.refresh(this);
  });

  /**
   * 关闭tab页
   */
  $("body").on("click", "#tabAction a", function () {
    var num = $(this).attr("data-num");
    okTab.tabClose(num);
  });

  /**
   * 全屏/退出全屏
   */
  $("body").on("keydown", function (event) {
    event = event || window.event || arguments.callee.caller.arguments[0];
    // 按 Esc
    if (event && event.keyCode === 27) {
      console.log("Esc");
      $("#fullScreen").children("i").eq(0).removeClass("okicon-screen-restore");
    }
    // 按 F11
    if (event && event.keyCode == 122) {
      $("#fullScreen").children("i").eq(0).addClass("okicon-screen-restore");
    }
  });

  $("body").on("click", "#fullScreen", function () {
    if ($(this).children("i").hasClass("okicon-screen-restore")) {
      screenFun(2).then(function () {
        $(this).children("i").eq(0).removeClass("okicon-screen-restore");
      });
    } else {
      screenFun(1).then(function () {
        $(this).children("i").eq(0).addClass("okicon-screen-restore");
      });
    }
  });

  /**
   * 全屏和退出全屏的方法
   * @param num 1代表全屏 2代表退出全屏
   * @returns {Promise}
   */
  function screenFun(num) {
    num = num || 1;
    num = num * 1;
    var docElm = document.documentElement;

    switch (num) {
      case 1:
        if (docElm.requestFullscreen) {
          docElm.requestFullscreen();
        } else if (docElm.mozRequestFullScreen) {
          docElm.mozRequestFullScreen();
        } else if (docElm.webkitRequestFullScreen) {
          docElm.webkitRequestFullScreen();
        } else if (docElm.msRequestFullscreen) {
          docElm.msRequestFullscreen();
        }
        break;
      case 2:
        if (document.exitFullscreen) {
          document.exitFullscreen();
        } else if (document.mozCancelFullScreen) {
          document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
          document.webkitCancelFullScreen();
        } else if (document.msExitFullscreen) {
          document.msExitFullscreen();
        }
        break;
    }

    return new Promise(function (res, rej) {
      res("返回值");
    });
  }

  /**
   * 系统公告
   */
  //$(document).on("click", "#notice", noticeFun);
  !function () {
    //var notice = sessionStorage.getItem("notice");
    //if (notice != "true") {
    //  noticeFun();
    //}
  }();

  function noticeFun() {
    var srcWidth = okUtils.getBodyWidth();
    layer.open({
      type: 0, title: "系统公告", btn: "我知道啦", btnAlign: 'c', content: getContent(),
      yes: function (index) {
        if (srcWidth > 800) {
          layer.tips('公告跑到这里去啦', '#notice', {
            tips: [1, '#000'],
            time: 2000
          });
        }
        sessionStorage.setItem("notice", "true");
        layer.close(index);
      },
      cancel: function (index) {
        if (srcWidth > 800) {
          layer.tips('公告跑到这里去啦', '#notice', {
            tips: [1, '#000'],
            time: 2000
          });
        }
      }
    });
  }

  ///**
  // * 捐赠作者
  // */
  //$(".layui-footer button.donate").click(function () {
  //  layer.tab({
  //    area: ["330px", "350px"],
  //    tab: [{
  //      title: "支付宝",
  //      content: "<img src='images/zfb.jpg' width='200' height='300' style='margin: 0 auto; display: block;'>"
  //    }, {
  //      title: "微信",
  //      content: "<img src='images/wx.jpg' width='200' height='300' style='margin: 0 auto; display: block;'>"
  //    }]
  //  });
  //});

  ///**
  // * QQ群交流
  // */
  //$("body").on("click", ".layui-footer button.communication, #noticeQQ", function () {
  //  layer.tab({
  //    area: ["auto", "370px"],
  //    tab: [{
  //      title: "QQ群2",
  //      content: "<img src='images/qq2.png' width='200' height='300' style='margin: 0 auto; display: block;'/>"
  //    }, {
  //      title: "QQ群1（已满）",
  //      content: "<img src='images/qq1.png' width='200' height='300' style='margin: 0 auto; display: block;'/>"
  //    }]
  //  });
  //});

  ///**
  // * 弹窗皮肤
  // */
  //$("#alertSkin").click(function () {
  //  okLayer.open("皮肤动画", "pages/system/alertSkin.html", "50%", "45%", function (layero) {
  //  }, function () {
  //  });
  //});

  /**
   * 退出操作
   */
  $("#logout").click(function () {
      okLayer.confirm("确定要退出吗？", function (index) {
          localStorage.clear();
          $.ajax({
              type: 'get',
              url: '/Author/LoginOut/prog',
              data: {},
              success: function (response) {
                  RedirectLogin();
              },
              error: function (msg) {
                  
              }
          });
          
    });
  });

  
});
