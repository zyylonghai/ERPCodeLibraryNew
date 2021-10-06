using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core
{
    public abstract class LibLock
    {
        public string Key { get; set; }

        public LibLockStatus Status { get; set; }

        public abstract void Lock();
        public abstract void UnLock();
    }
    public enum LibLockStatus
    {
        UnLock = 0,
        Lock = 1

    }
}
