using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ThueXeToanCau.Models
{
    public class DBContext
    {
        public static List<car_type> getCarTypes()
        {
            var rs = new List<car_type>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    rs = db.car_type.ToList();
                }
                return rs;
            }
            catch (Exception ex)
            {
                return rs;
            }
        }

        public static List<car_who_hire> getWhoType()
        {
            var rs = new List<car_who_hire>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    rs = db.car_who_hire.ToList();
                }
                return rs;
            }
            catch (Exception ex)
            {
                return rs;
            }
        }

        public static List<car_hire_type> getHireType()
        {
            var rs = new List<car_hire_type>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    rs = db.car_hire_type.ToList();
                }
                return rs;
            }
            catch (Exception ex)
            {
                return rs;
            }
        }

        public static List<list_car> getCars() {
            var cars = new List<list_car>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    cars = db.list_car.ToList();
                }
                return cars;
            }
            catch (Exception ex)
            { 
                return cars;
            }            
        }

        public static List<list_car_model> getCarModels()
        {
            var carModels = new List<list_car_model>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    carModels = db.list_car_model.ToList();
                }
                return carModels;
            }
            catch (Exception ex)
            {
                return carModels;
            }
        }

        public static List<list_car_type> getListCarTypes()
        {
            var carTypes = new List<list_car_type>();
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    carTypes = db.list_car_type.ToList();
                }
                return carTypes;
            }
            catch (Exception ex)
            {
                return carTypes;
            }
        }

        public static string addUpdateBooking(booking obj)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (obj.id == 0)
                    {
                        db.bookings.Add(obj);
                    }
                    else
                    {
                            var existEntity = db.bookings.Find(obj.id);
                            if (existEntity == null) return "Thất bại: Không tìm thấy thông tin lịch đặt";
                            db.Entry(existEntity).CurrentValues.SetValues(obj);
                            db.Entry(existEntity).State = EntityState.Modified;                       
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteBooking(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var booking = new booking() { id = id };
                    db.Entry(booking).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateDriver(driver dri)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (dri.id == 0)
                    {
                        using (MD5 md5Hash = MD5.Create())
                        {
                            string hash = GetMd5Hash(md5Hash, dri.pass);
                            dri.pass = hash;
                            db.drivers.Add(dri);
                        }
                    }
                    else
                    {
                        // Keep old password
                        if (string.IsNullOrEmpty(dri.pass))
                        {
                            var existEntity = db.drivers.Find(dri.id);
                            if (existEntity == null) return "Thất bại: Không tìm thấy thông tin tài xế";
                            //dri.pass = existEntity.pass;
                            db.Entry(existEntity).CurrentValues.SetValues(dri);
                            //db.Entry(existU).State = EntityState.Modified;
                        }
                        else
                        {
                            // change new pass
                            using (MD5 md5Hash = MD5.Create())
                            {
                                string hash = GetMd5Hash(md5Hash, dri.pass);
                                dri.pass = hash;
                                db.Entry(dri).State = EntityState.Modified;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }
        public static string addUpdateDriver2(list_online lo)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (lo.id == 0)
                    {
                        lo.date_time = DateTime.Now;
                        lo.geo = Config.CreatePoint(lo.lat, lo.lon);
                        lo.status = 0;
                        db.list_online.Add(lo);

                    }
                    db.SaveChanges();
                    //list_online lo = new list_online();
                    //lo.car_number = dri.car_number;
                    //lo.date_time = DateTime.Now;
                    //lo.geo = Config.CreatePoint(lat, lon);
                    //lo.lat = lat;
                    //lo.lon = lon;
                    //lo.phone = dri.phone;
                    //lo.status = 0;
                    //db.list_online.Add(lo);
                    //db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }
        public static string deleteDriver(int dId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var cp = new driver() { id = dId };
                    db.Entry(cp).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateCarPrice(car_price cp)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if(cp.ID == 0)
                    {
                        db.car_price.Add(cp);
                    } else
                    {
                        db.Entry(cp).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteCarPrice(int cpId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var cp = new car_price() { ID = cpId };
                    db.Entry(cp).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateCarPriceAirport(car_price_airport cp)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (cp.id == 0)
                    {
                        db.car_price_airport.Add(cp);
                    }
                    else
                    {
                        db.Entry(cp).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteCarPriceAirport(int cpId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var cp = new car_price_airport() { id = cpId };
                    db.Entry(cp).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateNotice(notice n)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (n.id == 0)
                    {
                        db.notices.Add(n);
                    }
                    else
                    {
                        db.Entry(n).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteNotice(int nId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var n = new notice() { id = nId };
                    db.Entry(n).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateCarType(car_type ct)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (ct.id == 0)
                    {
                        db.car_type.Add(ct);
                    }
                    else
                    {
                        db.Entry(ct).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteCarType(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var ct = new car_type() { id = id };
                    db.Entry(ct).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateFactor(factor ft)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (ft.id == 0)
                    {
                        db.factors.Add(ft);
                    }
                    else
                    {
                        db.Entry(ft).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteFactor(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var ft = new factor() { id = id };
                    db.Entry(ft).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateAirportWay(airport_way aw)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (aw.id == 0)
                    {
                        db.airport_way.Add(aw);
                    }
                    else
                    {
                        db.Entry(aw).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteAirportWay(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var aw = new airport_way() { id = id };
                    db.Entry(aw).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateHireType(car_hire_type ht)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (ht.id == 0)
                    {
                        db.car_hire_type.Add(ht);
                    }
                    else
                    {
                        db.Entry(ht).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteHireType(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var ct = new car_hire_type() { id = id };
                    db.Entry(ct).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateWhoType(car_who_hire wh)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (wh.id == 0)
                    {
                        db.car_who_hire.Add(wh);
                    }
                    else
                    {
                        db.Entry(wh).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteWhoType(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var wh = new car_who_hire() { id = id };
                    db.Entry(wh).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }


        public static string addUpdateNationalDay(NationalDay nalDay)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (nalDay.ID == 0)
                    {
                        db.NationalDays.Add(nalDay);
                    }
                    else
                    {
                        db.Entry(nalDay).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteNationalDay(int id)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var nalDay = new NationalDay() { ID = id };
                    db.Entry(nalDay).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string addUpdateUser(user u)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (u.id == 0)
                    {
                        using (MD5 md5Hash = MD5.Create())
                        {
                            string hash = GetMd5Hash(md5Hash, u.pass);
                            u.pass = hash;
                            db.users.Add(u);
                        }                            
                    }
                    else
                    {
                        // Keep old password
                        if (string.IsNullOrEmpty(u.pass))
                        {
                            var existU = db.users.Find(u.id);
                            if (existU == null) return "Thất bại: Không tìm thấy user";
                            existU.name = u.name;
                            db.Entry(existU).State = EntityState.Modified;
                        } else
                        {
                            // change new pass
                            using (MD5 md5Hash = MD5.Create())
                            {
                                string hash = GetMd5Hash(md5Hash, u.pass);
                                u.pass = hash;
                                db.Entry(u).State = EntityState.Modified;
                            }
                        }                        
                    }
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        public static string deleteUser(int uId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var u = new user() { id = uId };
                    db.Entry(u).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Thất bại: " + ex.Message;
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
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

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static user validateLogin(LoginModel model)
        {
            user u = null;
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string hash = GetMd5Hash(md5Hash, model.Password);
                        u = db.users.Where(f => f.name.Equals(model.Username) && f.pass.Equals(hash)).FirstOrDefault();                                                
                    }
                }
                return u;
            } catch (Exception ex)
            {
                return u;
            }
        }
    }
}