/**创建面板 */
function CreateColla(id,nm) {
    let colla = document.createElement("div");
    colla.setAttribute("class", "layui-collapse");
    colla.setAttribute("lay-filter", id);
    colla.id = id;

    let collaitem = document.createElement("div");
    collaitem.setAttribute("class", "layui-colla-item");
    colla.append(collaitem);

    let colltitle = document.createElement("h2");
    colltitle.setAttribute("class", "layui-colla-title");
    colltitle.textContent = nm;
    collaitem.append(colltitle);

    let collContent = document.createElement("div");
    collContent.setAttribute("class", "layui-colla-content layui-show");
    collContent.id = id + "_content";
    //collContent.innerHTML = "<div>sdfsdfsdfsdfs</div>";
    collContent.textContent = "请添加内容";
    collaitem.append(collContent);

    return colla;
}
/**
 * *创建按钮组控件
 * @param {any} id
 */
function CreateBtnGroup(id) {
    let btncontainer = document.createElement("div");
    btncontainer.setAttribute("class", "layui-btn-container");
    btncontainer.id = id;

    let btngp = document.createElement("div");
    btngp.setAttribute("class", "layui-btn-group");
    btngp.id = id + "_group";
    btncontainer.append(btngp);


    return btncontainer;
}



