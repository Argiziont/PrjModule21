﻿using System.Security.Cryptography;
using System.Text;

namespace TCPIPLib.Misc
{
    public static class Encryptor
    {
        /// <summary>
        ///     Get MD5 hash from string
        /// </summary>
        /// <param name="text">Your string</param>
        /// <returns>Hashed string</returns>
        public static string Md5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            var result = md5.Hash;

            var strBuilder = new StringBuilder();
            foreach (var t in result) strBuilder.Append(t.ToString("x2"));

            return strBuilder.ToString();
        }
    }
}