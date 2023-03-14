using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace POS.Lib.Configuration
{
    public class P1EncryptionHandler
    {
        public P1EncryptionHandler()
        {

        }

        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
        /// <summary>
        /// Decrypt the text
        /// </summary>
        /// <param name="outputText"> A Encrypted Text </param>
        /// <returns>Decrypted Text</returns>
        public string DecryptText(string outputText)
        {

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(outputText));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Use this method to test the encryption and get values 
        /// </summary>
        /// <param name="outputText"></param>
        /// <returns></returns>
        public string EncryptText(string outputText)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(outputText);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
    }
}
