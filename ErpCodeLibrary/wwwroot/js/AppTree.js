function AppTree(elemid) {
    this.Elemid = elemid;
    this.id = elemid;
    this.showCheckbox = true;
    this.isJump = false;//是否允许点击节点时弹出新窗口跳转
    this.showLine = false;  //是否开启连接线
    this.onlyIconControl = true; //是否仅允许节点左侧图标控制展开收缩
    this.edit = [];//操作节点的图标 ['add', 'update', 'del']
    this.data = [];
}
AppTree.prototype = {
    constructor: AppTree,
    libTree: null,
    initialTree: function () {
        let othis = this;
        layui.use(['tree'], function () {
            var tree = layui.tree;
            othis.libTree = tree;
            //开启节点操作图标
            tree.render({
                elem: '#' + othis.Elemid
                , id: othis.id
                , data: othis.data
                , edit: othis.edit//操作节点的图标
                , showCheckbox: othis.showCheckbox
                , isJump: othis.isJump
                , onlyIconControl: othis.onlyIconControl
                , showLine: othis.showLine
                , click: function (obj) {
                    //layer.msg(JSON.stringify(obj.data));
                    othis.NodeClick(obj.data);
                }
            });
        });
    },
    NodeClick: function (nodedata) {

    },
    Reload: function () {
        this.libTree.reload(this.id, this.data);
    }

}