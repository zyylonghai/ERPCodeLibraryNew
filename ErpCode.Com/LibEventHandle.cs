using System;
using System.Collections.Generic;
using System.Text;

namespace ErpCode.Com
{
    public class LibEventHandle
    {
        public LibEventSource EventSource { get; set; }
        public LibEventType EventType { get; set; }
    }
    /// <summary>事件源</summary>
    public class LibEventSource
    {
        public string ControlID { get; set; }
        public string FieldNm { get; set; }
        public string FieldValue { get; set; }

    }
    public enum LibEventType
    {
        OnChange=1,//值改变事件
        OnClick=2,//点击事件
        Onfocus=3,//聚焦点事件
        Onblur=4,//
        OnKeydown=5//
    }
}
