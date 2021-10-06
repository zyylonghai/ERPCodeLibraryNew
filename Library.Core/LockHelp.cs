using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core
{
    public class LockHelp<T>
        where T : LibLock
    {
        //private static 
        static Dictionary<string, List<T>> LockContainer { get; set; }

        public static void AddLock(T libLock)
        {
            List<T> val = null;
            if (LockContainer == null) LockContainer = new Dictionary<string, List<T>>();
            if (!LockContainer.TryGetValue(libLock.Key, out val))
            {
                val = new List<T>();
                LockContainer.Add(libLock.Key, val);
            }
            libLock.Lock();
            val.Add(libLock);
        }
        /// <summary>
        /// 创建锁，并加入锁容器中
        /// </summary>
        /// <param name="key">锁的钥匙</param>
        /// <param name="args">创建锁需要的参数(一般是构造函数参数)</param>
        public static void AddLock(string key, params object[] args)
        {
            T l = (T)Activator.CreateInstance(typeof(T), args);
            l.Key = key;
            AddLock(l);

        }
        /// <summary>
        /// 获取该钥匙下的所有锁
        /// </summary>
        /// <param name="key">钥匙</param>
        /// <returns></returns>
        public static List<T> GetLock(string key)
        {
            if (LockContainer == null) return null;
            return LockContainer[key];
        }
        public static void RemoveLock(string key)
        {

        }

        public static void RemoveLock(T libLock)
        {
            if (LockContainer != null)
            {
                List<T> val = null;
                if (LockContainer.TryGetValue(libLock.Key, out val))
                {
                    val.Remove(libLock);
                }
            }
        }

        public static void RemoveLock(string key, params object[] args)
        {

        }
    }
}
