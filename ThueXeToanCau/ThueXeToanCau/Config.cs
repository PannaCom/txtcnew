using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ThueXeToanCau.Models;
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
        public static int factorBackWay_GoWith = 30;
        public static int pricePerDay = 1200000;
        public static thuexetoancauEntities db = new thuexetoancauEntities();
        //convert longitude latitude to geography
        public static DbGeography CreatePoint(double? latitude, double? longitude)
        {
            if (latitude == null || longitude == null) return null;
            latitude = (double)latitude;
            longitude = (double)longitude;
            return DbGeography.FromText(String.Format("POINT({1} {0})", latitude, longitude));
        }
        public static bool isHoliDay(DateTime? date){
            try{
                var fnd=db.NationalDays.Any(o => o.StartDate <= date && o.EndDate >= date);
                if (fnd == true) { 
                    return fnd;
                }
                else {
                    if (date.Value.DayOfWeek == DayOfWeek.Saturday || date.Value.DayOfWeek == DayOfWeek.Sunday)
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }

            }catch(Exception ex){
                return false;
            }
            //return false;
        }
        public static string getHotLine()
        {
            try
            {
                var p = db.infoes.FirstOrDefault();
                return "<a href=\"tel:" + p.hotline1 + "\" style=\"padding-top:15px;float:left;\">Gọi đặt xe: " + p.hotline1 + "</a>  <a href=\"tel:" + p.hotline2 + "\" style=\"padding-top:15px;float:left;\">&nbsp;hoặc " + p.hotline2 + "</a>";
            }
            catch
            {
                return "Gọi <a href=\"tel:0129968888\" style=\"padding-top:15px;\">Gọi đặt xe: 0129968888</a>  <a href=\"tel:02436888333\" style=\"padding-top:15px;\">&nbsp;hoặc 02436888333</a>";
            }
        }
        public static string getAddress()
        {
            try
            {
                var p = db.infoes.FirstOrDefault();
                return "<p class=\"pull-left\" style=\"width:100%;\">"+ p.address + "</p>";
            }
            catch
            {
                return "";
            }
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
        public static int dateDiff(DateTime date1, DateTime date2)
        {
            try
            {
                if (date1 == null || date2 == null) return 0;
                TimeSpan TS = new System.TimeSpan(date1.Ticks - date2.Ticks);
                return (int)Math.Abs(TS.TotalDays);
            }
            catch (Exception ex)
            {
                return 100;
            }
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = value;
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[v].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}