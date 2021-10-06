using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library.Core
{
   public class RSAHelp
    {
        /// <summary>
        /// 生成密钥对(2048位)
        /// </summary>
        /// <returns>返回密钥对数组，数组下标0为私钥，数组下标1为公钥</returns>
        public static string[] GenerateKeys()
        {
            string[] sKeys = new String[2];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            sKeys[0] = rsa.ToXmlString(true);
            sKeys[1] = SerializerUtils.XMLDeSerialize<RSAKeyValue>(rsa.ToXmlString(false)).Modulus;
            return sKeys;
        }

        #region 公开函数

        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string EncryptPublickey(string data, string publicKey)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentException("Invalid Public Key");
            }
            if (!publicKey.Contains("<RSAKeyValue>"))
                publicKey = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", publicKey);
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(data);//有含义的字符串转化为字节流
                rsaProvider.FromXmlString(publicKey);//载入公钥
                int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
                {
                    while (true)
                    { //分段加密
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encryptedBytes = rsaProvider.Encrypt(temp, false);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                }
            }
        }
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="encryptedInput"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Decryptprivekey(string encryptedInput, string privateKey)
        {
            if (string.IsNullOrEmpty(encryptedInput))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new ArgumentException("Invalid Private Key");
            }

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Convert.FromBase64String(encryptedInput);
                rsaProvider.FromXmlString(privateKey);
                int bufferSize = rsaProvider.KeySize / 8;
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var rawBytes = rsaProvider.Decrypt(temp, false);
                        outputStream.Write(rawBytes, 0, rawBytes.Length);
                    }
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }
        }


        /// <summary>
        /// RSA加密 使用私钥加密
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptPrivekey(string data, string key)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            var privateRsa = GetRSAFromkey(key);
            //转换密钥  下面的DotNetUtilities来自Org.BouncyCastle.Security
            var keyPair = DotNetUtilities.GetKeyPair(privateRsa);

            var c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");

            c.Init(true, keyPair.Private);//取私钥（true为加密）


            int bufferSize = (privateRsa.KeySize / 8) - 11;//单块最大长度
            var buffer = new byte[bufferSize];
            using (MemoryStream inputStream = new MemoryStream(byteData), outputStream = new MemoryStream())
            {
                while (true)
                { //分段加密
                    int readSize = inputStream.Read(buffer, 0, bufferSize);
                    if (readSize <= 0)
                    {
                        break;
                    }

                    var temp = new byte[readSize];
                    Array.Copy(buffer, 0, temp, 0, readSize);
                    //var encryptedBytes = rsaProvider.Encrypt(temp, false);
                    var encryptedBytes = c.DoFinal(temp);
                    outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                }
                return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
            }

        }

        /// <summary>
        /// RSA解密 使用公钥解密
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptPublickey(string data, string key)
        {
            if (!key.Contains("<RSAKeyValue>"))
                key = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", key);
            byte[] byteData = Convert.FromBase64String(data);
            var privateRsa = GetRSAFromkey(key);
            //转换密钥  
            var keyPair = DotNetUtilities.GetRsaPublicKey(privateRsa);

            var c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");

            c.Init(false, keyPair);//取公钥（false为解密）

            using (MemoryStream inputStream = new MemoryStream(byteData), outputStream = new MemoryStream())
            {
                int restLength = byteData.Length;
                while (restLength > 0)
                {
                    int readLength = restLength < privateRsa.KeySize / 8 ? restLength : privateRsa.KeySize / 8;
                    restLength = restLength - readLength;
                    byte[] readBytes = new byte[readLength];
                    inputStream.Read(readBytes, 0, readLength);
                    byte[] append = c.DoFinal(readBytes);
                    outputStream.Write(append, 0, append.Length);
                }
                //注意，这里不一定就是用utf8的编码方式,这个主要看加密的时候用的什么编码方式
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }

        }

        private static string GetHash(string strSource)
        {
            try
            {
                //从字符串中取得Hash描述 
                byte[] Buffer;
                byte[] HashData;
                System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
                Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(strSource);
                HashData = MD5.ComputeHash(Buffer);
                return Convert.ToBase64String(HashData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="p_strKeyPrivate">私钥</param>
        /// <param name="m_strHashbyteSignature">需签名的数据</param>
        /// <returns>签名后的值</returns>
        public static string Sign(string privekey, string data)
        {
            string hash = GetHash(data);
            byte[] rgbHash = Convert.FromBase64String(hash);
            //byte[] rgbHash = Encoding.UTF8.GetBytes(m_strHashbyteSignature);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(privekey);
            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("MD5");
            byte[] inArray = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(inArray);
            //return Encoding.UTF8.GetString(inArray);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="publickey">公钥</param>
        /// <param name="data">待验证的数据</param>
        /// <param name="signstr">签名后的数据</param>
        /// <returns>签名是否符合</returns>
        public static bool DeSign(string publickey, string data, string signstr)
        {
            try
            {
                string hash = GetHash(data);
                byte[] rgbHash = Convert.FromBase64String(hash);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(publickey);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
                deformatter.SetHashAlgorithm("MD5");
                byte[] rgbSignature = Convert.FromBase64String(signstr);
                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        private static RSA GetRSAFromkey(string xmlkey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlkey);
            return rsa;
        }
    }

    [Serializable]
    public class RSAKeyValue
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
    }
}
