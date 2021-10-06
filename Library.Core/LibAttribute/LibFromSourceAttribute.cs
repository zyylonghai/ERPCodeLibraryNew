using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.LibAttribute
{
    /// <summary>
    /// 来源数据
    /// </summary>
    public class LibFromSourceAttribute : Attribute
    {
        private string _fromTableNm = null;
        private string _fromfieldNm = null;
        private string _desc = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromtb">来源表名</param>
        /// <param name="fromfield">来源字段名</param>
        public LibFromSourceAttribute(string fromtb,string fromfield)
        {
            this._fromTableNm = fromtb;
            this._fromfieldNm = fromfield;
        }
        /// <summary></summary>
        /// <param name="fromtb">来源表名</param>
        /// <param name="fromfield">来源字段名</param>
        /// <param name="desc">来源描述</param>
        public LibFromSourceAttribute(string fromtb, string fromfield,string desc)
        {
            this._fromTableNm = fromtb;
            this._fromfieldNm = fromfield;
            this._desc = desc;
        }

        public string FromTableNm { get { return _fromTableNm; } }

        public string FromFieldNm { get { return _fromfieldNm; } }

        public string Desc { get { return _desc; } }
    }
}
