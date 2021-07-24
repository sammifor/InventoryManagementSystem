using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.PaymentProviderModels
{
    public class MPG
    {
        private string _tradeInfo;
        private string _tradeSha;

        public string MerchantID { get; set; }
        public string Version { get; set; }

        public string TradeInfo { get => _tradeInfo; }
        public string TradeSha { get => _tradeSha; }

        public void SetTradeInfo(string infoQueryString, string hashKey, string hashIV)
        {
            _tradeInfo = EncryptAES256(infoQueryString, hashKey, hashIV);
            _tradeSha = GetHashSha256($"HashKey={hashKey}&{TradeInfo}&HashIV={hashIV}");
        }

        private string EncryptAES256(string source, string hashKey, string hashIV)//加密 
        {
            byte[] sourceBytes = AddPKCS7Padding(Encoding.UTF8.GetBytes(source), 32);
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(hashKey);
            aes.IV = Encoding.UTF8.GetBytes(hashIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            ICryptoTransform transform = aes.CreateEncryptor();

            return ByteArrayToHex(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length)).ToLower();
        }

        public static string DecryptAES256(string encryptData, string hashKey, string hashIV)//解密 
        {
            var encryptBytes = HexStringToByteArray(encryptData.ToUpper());
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(hashKey);
            aes.IV = Encoding.UTF8.GetBytes(hashIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            ICryptoTransform transform = aes.CreateDecryptor();

            return Encoding.UTF8.GetString(RemovePKCS7Padding(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length)));
        }

        private byte[] AddPKCS7Padding(byte[] data, int iBlockSize)
        {
            int iLength = data.Length;
            byte cPadding = (byte)(iBlockSize - (iLength % iBlockSize));
            var output = new byte[iLength + cPadding];
            Buffer.BlockCopy(data, 0, output, 0, iLength);
            for(var i = iLength; i < output.Length; i++)
                output[i] = (byte)cPadding;
            return output;
        }

        private static byte[] RemovePKCS7Padding(byte[] data)
        {
            int iLength = data[data.Length - 1];
            var output = new byte[data.Length - iLength];
            Buffer.BlockCopy(data, 0, output, 0, output.Length);
            return output;
        }

        private string ByteArrayToHex(byte[] barray)
        {
            char[] c = new char[barray.Length * 2];
            byte b;
            for(int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }

            return new string(c);
        }

        private static byte[] HexStringToByteArray(string hexString)
        {
            int hexStringLength = hexString.Length;
            byte[] b = new byte[hexStringLength / 2];
            for(int i = 0; i < hexStringLength; i += 2)
            {
                int topChar = (hexString[i] > 0x40 ? hexString[i] - 0x37 : hexString[i] - 0x30) << 4;
                int bottomChar = hexString[i + 1] > 0x40 ? hexString[i + 1] - 0x37 : hexString[i + 1] - 0x30;
                b[i / 2] = Convert.ToByte(topChar + bottomChar);
            }
            return b;
        }

        private string GetHashSha256(string text)
        {
            using(SHA256 mySHA256 = SHA256.Create())
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashedText = mySHA256.ComputeHash(textBytes);
                StringBuilder builder = new StringBuilder();
                foreach(byte hashedByte in hashedText)
                {
                    builder.AppendFormat("{0:X2}", hashedByte);
                }

                return builder.ToString();
            }
        }
    }

    public class TradeInfo
    {
        public string MerchantID { get; set; }
        public string RespondType { get; set; }
        public string Version { get; set; }
        public int LoginType { get; set; }
        public string TimeStamp { get; set; }
        public string MerchantOrderNo { get; set; }
        public int Amt { get; set; }
        public string ItemDesc { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
        public string ClientBackURL { get; set; }
        public string Email { get; set; }

        public string ToQueryString()
        {
            var keyValuePairs = new Dictionary<string, string>();

            PropertyInfo[] properties = typeof(TradeInfo).GetProperties();
            foreach(PropertyInfo p in properties)
            {
                string key = p.Name;
                string value = p.GetValue(this)?.ToString();
                if(!string.IsNullOrEmpty(value))
                    keyValuePairs.Add(key, value);
            }

            var resultPairs = new QueryBuilder(keyValuePairs).ToQueryString().Value.Substring(1);

            return resultPairs;
        }
    }
}
