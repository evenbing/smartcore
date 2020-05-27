using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCore.Infrastructure.Security
{
    /// <summary>
    /// var content = "123";
    ///var privateKey = RSAHelper.ByteToHexStr(Convert.FromBase64String("MIIBVQIBADANBgkqhkiG9w0BAQEFAASCAT8wggE7AgEAAkEA051JxQSrpN2cgI/fbCFjsALy7G055ichin5FF9qZ6VcdOa4/+V80FMLhR6ifRD2Sb/4qR0pMLnfkJadKBFM/QwIDAQABAkBMV3MUk6HEoXpjWwQUQ1tuVTIEH0eDA1zzVKhieaeK6Q1q4CiqJJ3fMkSTxgQZc6Wy11USJa6cRkYul4hsssddccBAiEA9Iiu7kxwbUE3DNnPzYi7st++fyo2ch9Wh2jF9BQB0YMCIQDdiXK/Y7673ucqBZdVpECJgp3DKCKlJPtfpuRmbSIvQQIhAM0IBdSclu+kbKoDvu7QpMCYRbuOA1Sw3fZvbPr4A4ZNAiBxcakpCNLrMcH+as6MNIg34oMXJL5ZAw8WdEgRi2EuAQIhALx6SB/hoTg91dGPd/Ql6pvRQaEG+HWda2yrW8fd41ot"));
    ///var publicKey = RSAHelper.ByteToHexStr(Convert.FromBase64String("MFwwDQYJKoZIhvcNAQEBBQADSwAweniiO2ndANOdScUEq6TdnICP32whY7AC8uxtOeYnIYp+RRfamelXHTmuP/lfNBTC4Ueon0Q9km/+KkdKTC535CWnSgRTP0MCAwEAAQ=="));
    ///var signData = RSAHelper.RsaSign(content, privateKey);
    ///var result = RSAHelper.VerifySign(content, publicKey, signData);
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="content">数据</param>
        /// <param name="privateKey">RSA密钥</param>
        /// <returns></returns>
        public static string RsaSign(string content, string privateKey)
        {
            var signer = SignerUtilities.GetSigner("SHA1withRSA");
            //将java格式的rsa密钥转换成.net格式
            var privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(StrToToHexByte(privateKey));
            signer.Init(true, privateKeyParam);
            var plainBytes = Encoding.UTF8.GetBytes(content);
            signer.BlockUpdate(plainBytes, 0, plainBytes.Length);
            var signBytes = signer.GenerateSignature();
            return ByteToHexStr(signBytes);
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="publicKey">RSA公钥</param>
        /// <param name="signData">签名字段</param>
        /// <returns></returns>
        public static bool VerifySign(string content, string publicKey, string signData)
        {
            try
            {
                var signer = SignerUtilities.GetSigner("SHA1withRSA");
                var publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(StrToToHexByte(publicKey));
                signer.Init(false, publicKeyParam);
                var signBytes = StrToToHexByte(signData);
                var plainBytes = Encoding.UTF8.GetBytes(content);
                signer.BlockUpdate(plainBytes, 0, plainBytes.Length);
                var ret = signer.VerifySignature(signBytes);
                return ret;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
