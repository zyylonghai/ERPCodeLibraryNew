using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.LibAttribute
{
    /// <summary>
    /// 自增长属性
    /// </summary>
    public class LibIdentityAttribute:Attribute
    {
        public   int _startvalue { get;  set; }
        public   int _increment { get;  set; }
        public int _currvalue { get;  set; }
        public LibIdentityAttribute(int start, int increment=1)
        {
            this._startvalue = start;
            this._increment = increment;
            this._currvalue = 0;
            //this._currvalue = start;
        }
        public LibIdentityAttribute()
        {
            
        }

        public int GetValue()
        {
            //if (this._currvalue == -1) { 
            //    this._currvalue += this._increment; 
            //    return this._startvalue; 
            //}
            this._currvalue += this._increment;
            return this._currvalue;
        }
        public void SetCurrValue(int value)
        {
            this._currvalue = value;
        }
    }


    //public abstract class LibBaseAttribute : Attribute
    //{
    //    public abstract void Process(ModelMetadata modelMetaData);
    //}
}
