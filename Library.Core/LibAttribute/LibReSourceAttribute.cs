using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.LibAttribute
{
    public class LibReSourceAttribute : Attribute
    {
        private string _reSource;
        public LibReSourceAttribute(string reSource)
            : base()
        {
            this._reSource = reSource;
        }
        public string Resource
        {
            get { return _reSource; }
        }
    }
}
