using System;

namespace Library.Core
{
    public class LibExceptionBase : Exception
    {
        private string _innerStackTrace = string.Empty;
        private string _message = string.Empty;
        public LibExceptionBase()
            : base()
        {

        }

        public LibExceptionBase(string message)
            : base(message)
        {
            this._message = message;
        }
        public LibExceptionBase(int msgcode, params object[] parms)
            : base()
        {
            //this._message = ResFactory.ResManager.GetResByKey(msgcode.ToString());
            if (parms != null)
            {
                this._message = string.Format(this._message, parms);
            }
            //this._message = ResFactory.ResManager.GetResByKey(msgcode.ToString());
        }

        public LibExceptionBase(string message, string innerStackTrace)
            : base(message)
        {
            this._message = message;
            this._innerStackTrace = innerStackTrace;
        }

        public override string Message
        {
            get
            {
                return this._message;
            }
        }

        public override string StackTrace
        {
            get
            {
                return string.Format("{0}\\n Old StackTrace \\n {1}", base.StackTrace, this._innerStackTrace);
            }
        }

        #region 公开函数

        #endregion
    }

    public interface IlibException
    {
        void BeforeThrow();
    }
}
