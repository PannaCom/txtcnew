using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ThueXeToanCau
{
    public class Config
    {
        public static int factor1 = 130;
        public static int factor2 = 130;
        public static int factor3 = 130;
        public static int factor4 = 130;
        public static int factor5 = 130;
        public static int factorHoliday1 = 120;
        public static int factorHoliday2 = 130;
        public static int factorHoliday3 = 140;
        public static int factorHoliday4 = 150;
        public static int factorHoliday5 = 160;
        public static int price1 = 6000;
        public static int price2 = 8000;
        public static int price3 = 12000;
        public static int price4 = 16000;
        public static int price5 = 18000;
        public static int reduct1 = 5;
        public static int reduct2 = 10;
        public static int reduct3 = 15;
        public static int reduct4 = 20;
        
        //convert longitude latitude to geography
        public static DbGeography CreatePoint(double? latitude, double? longitude)
        {
            if (latitude == null || longitude == null) return null;
            latitude = (double)latitude;
            longitude = (double)longitude;
            return DbGeography.FromText(String.Format("POINT({1} {0})", latitude, longitude));
        }
        public static bool isHoliDay(DateTime? date){
            return false;
        }
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}