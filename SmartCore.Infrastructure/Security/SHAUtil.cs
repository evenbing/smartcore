using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCore.Infrastructure.Security
{
    public class SHAUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
        public static string Sha256Compute(string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            //if (string.IsNullOrEmpty(key))
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}
            if (string.IsNullOrEmpty(key))
            {
                using (var hmacSha256 = new HMACSHA256())
                {
                    var hsah = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                    return BitConverter.ToString(hsah).Replace("-", "");
                }
            }
            else
            {
                using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    var hsah = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                    return BitConverter.ToString(hsah).Replace("-", "");
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HashSaltModel Sha512Compute(string data)
        {
            HashSaltModel hashSaltModel = new HashSaltModel();
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }
                using (var hmacSha512 = new System.Security.Cryptography.HMACSHA512())
                {
                hashSaltModel.Salt =hmacSha512.Key;// StringUtil.HashByteToString(hmacSha512.Key);
                hashSaltModel.HashData = hmacSha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data)); //StringUtil.HashByteToString(hmacSha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data)));
                }
            return hashSaltModel;
        }

        public static bool VerifyHash(string data, byte[] storedHash, byte[] storedSalt)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            //if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            //if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
    public class HashSaltModel
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] HashData { get; set; }

        public byte[] Salt { get; set; }
    }
}
