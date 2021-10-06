function PageObj() {
    this.progNm="";
    this.progDesc="";
    this.progPackage="";
    this.controllerNm="";
    this.CustomPage = "";
    this.mastTable = "";
    this.IsMenu = true;
    this.HasAddbtn = true;
    this.Haseditbtn = true;
    this.Hasdeletebtn = true;
    this.HasCopybtn = true;
    this.HasSearchbtn = true;
    this.HasLogSearchbtn = true;
}
function ControlObj(id, title) {
    this.progNm;
    this.ID = id;
    this.ControlType;
    this.title = title;
    this.DataSourceNm;
    this.TableNm;
    this.btalismn = "";
    this.OrderNo=0;
}

function FieldObj() {
    this.ID;
    this.Field;
    this.Fieldtitle = "";
    this.IsHide = false;
    this.OnlyRead = false;
    this.IsOnline = false;
    this.ElemType=1;
    this.DefaultValue = "";
    this.OrderNo = 0;
    this.Onchange = false;
    this.Onclick = false;
    this.Onfocus = false;
    this.Onblur = false;
    this.OnKeydown = false;
    this.IsRequired = false;
    this.IsNumber = false;
    this.ClickFunc = "";
    this.FieldLength = 30;
    this.RelateFieldExpress = "";
    this.FromSourceTB="";
    this.ValidExpress = "";
    this.RptColId = "";
    this.Remark="";
}

function LibClientDatas() {
    this.TableNm = "";
    this.DataSource = "";
    this.ClientDataInfos = [];
}
function LibClientDataInfo() {
    this.Datas;
    this.clientDataStatus; //状态 1：新增，2：编辑，3：删除
}

function LibEventHandle() {
    this.EventSource;
    this.EventType;

}
function LibEventSource() {
    this.ControlID = "";
    this.FieldNm = "";
    this.ElemType;
    this.FieldValue;
}
function LibForms(ctrlobj) {
    this.CtrlObj = ctrlobj;
    this.verifyPass = false;
}

function LibSearchConditon() {
    this.FieldNm = "";
    this.Symbol;
    this.valu1;
    this.valu2;
    this.Logic;
}

function LibAppFieldOption() {
    this.TableNm = "";
    this.Field = "";
    this.Options = [];
}


