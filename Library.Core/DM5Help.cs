using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library.Core
{
    public class DM5Help
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string Md5Encrypt(string strSource)
        {
            byte[] result = DoEncrypt(strSource);
            string strOut = System.Convert.ToBase64String(result);
            return strOut;
        }

        /// <summary>
        /// MD5解密
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string Md5Decrypt(string Source)
        {
            //将解密字符串转换成字节数组  
            byte[] bytIn = System.Convert.FromBase64String(Source);
            //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同  
            byte[] iv = { 101, 15, 95, 153, 78, 4, 218, 32 };//定义偏移量  
            byte[] key = { 26, 113, 228, 76, 56, 79, 127, 23 };//定义密钥  
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = iv;
            mobjCryptoService.IV = key;
            //实例流进行解密  
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader strd = new StreamReader(cs, Encoding.Default);
            return strd.ReadToEnd();
        }

        public static string MD5Encrypt2(string strSource)
        {
            byte[] result = DoEncrypt(strSource);
            return System.Convert.ToBase64String(result).Replace('+', '%');
        }

        public static string MD5Decrypt2(string strSource)
        {
            strSource = strSource.Replace('%', '+');
            return Md5Decrypt(strSource);
        }
        private static byte[] DoEncrypt(string strSource)
        {
            //把字符串放到byte数组中  
            byte[] bytIn = System.Text.Encoding.Default.GetBytes(strSource);
            //建立加密对象的密钥和偏移量          
            byte[] iv = { 101, 15, 95, 153, 78, 4, 218, 32 };//定义偏移量  
            byte[] key = { 26, 113, 228, 76, 56, 79, 127, 23 };//定义密钥  
            //实例DES加密类  
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = iv;
            mobjCryptoService.IV = key;
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            //实例MemoryStream流加密密文件  
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}
