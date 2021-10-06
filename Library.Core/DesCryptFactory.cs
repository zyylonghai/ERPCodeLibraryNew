using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Library.Core
{
    public class DesCryptFactory
    {
        // 创建Key
        public static string GenerateKey()
        {

            TripleDES desCrypto = TripleDES.Create();
            string key = ASCIIEncoding.ASCII.GetString(desCrypto.Key);
            
            while (key.Contains(AppConstManage.DBInfoArraySeparator) || key.Contains(AppConstManage.DBInfoArraySeparator2) || key.Contains("'"))
            {
                desCrypto = TripleDES.Create();
                key = ASCIIEncoding.ASCII.GetString(desCrypto.Key);
            }
            return key;
        }
        // 加密字符串
        public static string EncryptString(string sInputString, string sKey)
        {

            byte[] data = ASCIIEncoding.ASCII.GetBytes(sInputString);
            TripleDES DES = TripleDES.Create();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            //DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            //DES.Key = StrinTobyte(sKey);
            //DES.IV = StrinTobyte(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            DES.Clear();
            return BitConverter.ToString(result);
        }
        // 解密字符串
        public static string DecryptString(string sInputString, string sKey)
        {
            string[] sInput = sInputString.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            //DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }
        /// <summary>
        /// 3des加密
        /// </summary>
        /// <param name="aStrString"></param>
        /// <param name="aStrKey"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Encrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = StrinTobyte(aStrKey),
                    Mode = mode
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = StrinTobyte(aStrKey);
                }
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = StrinTobyte(aStrString);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public static byte[] Encrypt3Destobyte(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = StrinTobyte(aStrKey),
                    Mode = mode
                };
                if (mode == CipherMode.CBC)
                {
                    des.IV = StrinTobyte(aStrKey);
                }
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = StrinTobyte(aStrString);
                return desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 3des解密
        /// </summary>
        /// <param name="aStrString"></param>
        /// <param name="aStrKey"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Decrypt3Des(string aStrString, string aStrKey, CipherMode mode = CipherMode.ECB)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = StrinTobyte(aStrKey),
                    Mode = mode,
                    //Padding = PaddingMode.PKCS7
                };
                //if (mode == CipherMode.CBC)
                //{
                //    des.IV = StrinTobyte(aStrKey);
                //}
                var desDecrypt = des.CreateDecryptor();
                var result = "";
                byte[] buffer = Convert.FromBase64String(aStrString);
                result = Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                return result;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            keyArray = GetAesKey(keyArray, key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            keyArray = GetAesKey(keyArray, key);
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 128位处理key 
        /// </summary>
        /// <param name="keyArray">原字节</param>
        /// <param name="key">处理key</param>
        /// <returns></returns>
        private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            byte[] newArray = new byte[16];
            if (keyArray.Length < 16)
            {
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }

        private static byte[] StrinTobyte(string str)
        {
            byte[] bytearray = new byte[str.Length / 2];
            int index = 0;
            for (int i = 0; i < str.Length; i += 2)
            {
                bytearray[index] = byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                index++;
            }
            return bytearray;
        }

        public static void test()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            //ass.GetName().KeyPair;
        }
    }
}
