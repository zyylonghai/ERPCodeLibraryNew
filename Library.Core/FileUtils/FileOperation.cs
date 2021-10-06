using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library.Core.FileUtils
{
    public class FileOperation
    {
        #region 私有变量
        private string _filePath;
        private LibEncoding _Encoding;
        #endregion

        #region 共有属性
        /// <summary>文件路径</summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public LibEncoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        public string ExceptionMessage
        {
            get; set;
        }
        #endregion

        #region 共有方法

        /// <summary>读取文件</summary>
        /// <returns></returns>
        public string ReadFile()
        {
            return DoRead(_filePath);
        }

        public bool WritText(string context)
        {
            try
            {
                File.WriteAllText(_filePath, context, getEncoding());
                return true;
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                return false;
            }
        }
        public bool ExistsFile()
        {
            return File.Exists(_filePath);
        }

        public bool IsDirectory
        {
            get
            {
                return Directory.Exists(_filePath);
            }
        }

        public string SearchAndRead(string filename)
        {
            string filepath = string.Empty;
            string fileContent = string.Empty;
            if (IsDirectory)
            {

                string[] dirpath = Directory.GetDirectories(_filePath);
                foreach (string path in dirpath)
                {
                    filepath = string.Format(@"{0}\{1}", path, filename);
                    if (File.Exists(filepath))
                    {
                        fileContent = DoRead(filepath);
                        break;
                    }
                }
                return fileContent;

            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取子目录下的所有文件
        /// </summary>
        /// <returns></returns>
        public string[] SearchFileNm()
        {
            string filepath = string.Empty;
            List<string> result = new List<string>();
            DirectoryInfo directory = null;
            FileInfo[] files = null;
            if (IsDirectory)
            {
                string[] dirpath = Directory.GetDirectories(_filePath);
                foreach (string path in dirpath)
                {
                    directory = new DirectoryInfo(path);
                    files = directory.GetFiles();
                    foreach (FileInfo info in files)
                    {
                        result.Add(info.Name.Replace(info.Extension, ""));
                    }
                    //result.AddRange(Directory.GetFiles(path));
                }
                return result.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 获取当前目录及子目录下的所有文件（只查找二级目录）
        /// </summary>
        /// <returns></returns>
        public List<LibFileInfo> SearchAllFileInfo()
        {
            string filepath = string.Empty;
            List<LibFileInfo> result = new List<LibFileInfo>();
            DirectoryInfo directory = null;
            FileInfo[] files = null;
            LibFileInfo f = null;
            if (IsDirectory)
            {
                string[] dirpath = Directory.GetDirectories(_filePath);
                string[] folders = null;
                if (dirpath == null || dirpath.Length == 0)
                {
                    directory = new DirectoryInfo(_filePath);
                    files = directory.GetFiles();
                    foreach (FileInfo info in files)
                    {
                        f = new LibFileInfo();
                        f.FullFileName = info.Name;
                        f.Path = info.FullName;
                        f.FileName = info.Name.Replace(info.Extension, "");

                        f.Folder = folders == null ? string.Empty : folders[folders.Length - 1];
                        result.Add(f);
                    }
                }
                foreach (string path in dirpath)
                {
                    folders = path.Split('\\');
                    directory = new DirectoryInfo(path);
                    files = directory.GetFiles();
                    foreach (FileInfo info in files)
                    {
                        f = new LibFileInfo();
                        f.FullFileName = info.Name;
                        f.Path = info.FullName;
                        f.FileName = info.Name.Replace(info.Extension, "");

                        f.Folder = folders == null ? string.Empty : folders[folders.Length - 1];
                        result.Add(f);
                    }
                }
                return result;
            }
            return null;
        }
        public void CreateFile(bool cover)
        {
            FileStream stream = null;
            if (cover)
                stream = File.Create(_filePath);
            else
            {
                if (ExistsFile())
                {
                    //103 文件已经存在
                    throw new LibExceptionBase(103);
                }
                else
                {
                    stream = File.Create(_filePath);
                }
            }
            if (stream != null)
                stream.Close();
        }

        public void DeleteFile()
        {
            if (ExistsFile())
            {
                File.Delete(_filePath);
            }
        }
        //public string 
        #endregion

        #region 私有方法
        private System.Text.Encoding getEncoding()
        {
            System.Text.Encoding encode = System.Text.Encoding.UTF8;
            switch (_Encoding)
            {
                case LibEncoding.ASCII:
                    encode = System.Text.Encoding.ASCII;
                    break;
                case LibEncoding.Unicode:
                    encode = System.Text.Encoding.Unicode;
                    break;
                case LibEncoding.UTF32:
                    encode = System.Text.Encoding.UTF32;
                    break;
                case LibEncoding.UTF7:
                    encode = System.Text.Encoding.UTF7;
                    break;
                case LibEncoding.UTF8:
                    encode = System.Text.Encoding.UTF8;
                    break;
                case LibEncoding.Default:
                    encode = System.Text.Encoding.Default;
                    break;
            }
            return encode;
        }

        private string DoRead(string filePath)
        {
            string _context = string.Empty;

            try
            {
                _context = File.ReadAllText(filePath, getEncoding());
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
            }
            return _context;
        }
        #endregion

    }

    public enum LibEncoding
    {
        Default = 0,
        UTF8 = 1,
        UTF7 = 2,
        UTF32 = 3,
        Unicode = 4,
        ASCII = 5
    }

    public class LibFileInfo
    {
        /// <summary>所在文件夹</summary>
        public string Folder { get; set; }

        /// <summary>文件名(不包含后缀)</summary>
        public string FileName { get; set; }
        /// <summary> 文件名(包含后缀) </summary>
        public string FullFileName { get; set; }
        /// <summary> 完整路径</summary>
        public string Path { get; set; }

    }
}
