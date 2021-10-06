using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;

namespace Library.Core
{
    public class CachHelp
    {
        ObjectCache cache = MemoryCache.Default;

        public CachHelp()
        {
           
        }

        public void AddCachItem(string key, object val, DateTimeOffset dtoffset, LibChangeMonitorBase monitor)
        {
            var exist = cache[key];
            if (exist == null)
            {
                var policy = new CacheItemPolicy() { AbsoluteExpiration = dtoffset };
                policy.UpdateCallback = monitor.updatecallback;
                policy.ChangeMonitors.Add(monitor);
                cache.Set(key, val, policy);
            }

        }

        public void AddCachItem(string key, object val, DateTimeOffset dtoffset)
        {
            var exist = cache[key];
            if (exist == null)
            {
                cache.Set(key, val, dtoffset);
            }
        }

        public object GetCach(string key)
        {
            return cache[key];
        }

        public void RemoveCache(string key)
        {
            cache.Remove(key);
        }
        public void ClearCache()
        {
            List<string> keys = new List<string>();
            foreach (var item in cache)
            {
                keys.Add(item.Key);
            }
            foreach (string key in keys)
            {
                cache.Remove(key);
            }
        }
    }
    public abstract class LibChangeMonitorBase : ChangeMonitor
    {
        protected string _uniqueid = string.Empty;
        public CacheEntryUpdateCallback updatecallback = null;

        public LibChangeMonitorBase()
        {
            updatecallback = new CacheEntryUpdateCallback(CacheRemoveCallBack);
        }
        public override string UniqueId { get { return this._uniqueid; } }


        public abstract void CacheRemoveCallBack(CacheEntryUpdateArguments arg);

        protected override void Dispose(bool disposing)
        {
            base.Dispose();
        }
    }
}
