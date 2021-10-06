using Library.Core.FileUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Core
{
    public class LogHelp
    {
        ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        delegate void CompressionFileDelegate(string content);
        delegate void DeleteFileDelegate(List<string> filenm);
        string _configpath = string.Empty;
        LogConfig LogConfig = null;
        string msgflag = "Message:";
        static string _currentlogid = string.Empty;
        static int _currentlogidCount = 0;
        static object _locker = new object();//定义对象
        #region

        #endregion
        public LogHelp(string configpath)
        {
            this._configpath = configpath;
            FileOperation FileOperation = new FileOperation();
            FileOperation.Encoding = LibEncoding.UTF8;
            FileOperation.FilePath = string.Format(@"{0}AppLog.config", configpath);
            string configxml = FileOperation.ReadFile();
            LogConfig = SerializerUtils.XMLDeSerialize<LogConfig>(configxml);
        }

        public void WriteLog(string head, string content)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("[{0}]{1}", System.DateTime.Now, head));
            builder.AppendLine(msgflag);
            builder.Append(content);
            DoWriteFile(builder.ToString());
        }
        public void WriteLog(string head, Exception exception)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("[{0}]{1}", System.DateTime.Now, head));
            content.AppendLine(msgflag);
            content.Append(exception.Message);
            content.AppendLine();
            content.Append(exception.StackTrace);
            DoWriteFile(content.ToString());
            //AnsyWriteLog(content.ToString());
        }

        public List<LogInfo> GetLogInfos()
        {
            List<LogInfo> Loginfos = new List<LogInfo>();
            string filenm = System.DateTime.Now.ToString(LogConfig.FileNmDateFormatter);
            string filepath = string.Format(@"{0}\{1}\", this._configpath, LogConfig.FilePath);
            string content = string.Empty;
            FileOperation FileOperation = new FileOperation();
            FileOperation.Encoding = LibEncoding.UTF8;
            FileOperation.FilePath = filepath;
            var infos = FileOperation.SearchAllFileInfo();
            if (infos != null && infos.Count > 0)
            {
                LogInfo log = null;
                foreach (var item in infos)
                {
                    try
                    {
                        log = new LogInfo();
                        log.FileNm = item.FullFileName;
                        FileOperation.FilePath = item.Path;
                        content = FileOperation.ReadFile();
                        int index = content.IndexOf(msgflag);
                        log.Head = content.Substring(0, index);
                        log.DateTime = Convert.ToDateTime(log.Head.Split('[', ']')[1]);
                        log.Head = log.Head.Split('[', ']')[2];
                        Loginfos.Add(log);
                    }
                    catch { }

                }
            }
            return Loginfos;
        }

        //public void DeleteLogFile(string filenm)
        //{
        //    DeleteFileDelegate deleteFileDelegate = new DeleteFileDelegate(DoDeleteFile);
        //    AsyncCallback callback = new AsyncCallback(DeletCallBackMethod);
        //    IAsyncResult iar = deleteFileDelegate.BeginInvoke(filenm, callback, deleteFileDelegate);
        //}

        public void DeleteLogFileBatch(List<string> files)
        {
            DoDeleteFileBatch(files);
            //DeleteFileDelegate deleteFileDelegate = new DeleteFileDelegate(DoDeleteFileBatch);
            //AsyncCallback callback = new AsyncCallback(DeletCallBackMethod);
            //IAsyncResult iar = deleteFileDelegate.BeginInvoke(files, callback, deleteFileDelegate);
        }

        public string ReadLogFile(string filenm)
        {
            FileOperation FileOperation = new FileOperation();
            FileOperation.Encoding = LibEncoding.UTF8;
            FileOperation.FilePath = string.Format(@"{0}\{1}\{2}", this._configpath, LogConfig.FilePath, filenm);
            return FileOperation.ReadFile();
        }

        //private void AnsyWriteLog(string content)
        //{
        //    CompressionFileDelegate compressfile = new CompressionFileDelegate(DoWriteFile);
        //    AsyncCallback callback = new AsyncCallback(CallBackMethod);
        //    IAsyncResult iar = compressfile.BeginInvoke(content, callback, compressfile);
        //}

        private async Task DoDeleteFileBatch(List<string> files)
        {
            if (files == null) return;
            await Task.Run(() => {
                FileOperation FileOperation = new FileOperation();
                FileOperation.Encoding = LibEncoding.UTF8;
                foreach (string f in files)
                {
                    FileOperation.FilePath = string.Format(@"{0}\{1}\{2}", this._configpath, LogConfig.FilePath, f);
                    FileOperation.DeleteFile();
                }
            });
        }

        private async Task DoWriteFile(string content)
        {
            await Task.Run(() => {
                string filenm = System.DateTime.Now.ToString(LogConfig.FileNmDateFormatter);
                string filepath = string.Format(@"{0}\{1}\{2}.txt", this._configpath, LogConfig.FilePath, filenm);
                FileOperation FileOperation = new FileOperation();
                FileOperation.Encoding = LibEncoding.UTF8;
                FileOperation.FilePath = filepath;
                FileOperation.CreateFile(false);
                FileOperation.WritText(content);
            });
        }

        private void CallBackMethod(IAsyncResult ar)
        {

        }

        private void DeletCallBackMethod(IAsyncResult ar)
        {

        }

        /// <summary>产生日志ID</summary>
        /// <returns></returns>
        public static string GenerateLogId()
        {
            string id = System.DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            lock (_locker)
            {
                if (id == _currentlogid)
                {
                    _currentlogidCount += 1;
                    return string.Format("{0}{1}", id, _currentlogidCount);
                }
            }
            _currentlogid = id;
            return id;
        }
    }

    /// <summary>
    /// 日志配置
    /// </summary>
    [Serializable]
    public class LogConfig
    {
        /// <summary>文件名日期格式化</summary>
        public string FileNmDateFormatter { get; set; }
        /// <summary>文件存放位置</summary>
        public string FilePath { get; set; }
    }
    public class LogInfo
    {
        public string FileNm { get; set; }
        public string Head { get; set; }
        public DateTime DateTime { get; set; }
    }
}
