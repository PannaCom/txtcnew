using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThueXeToanCau.Models
{
    public class DBContext
    {
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

        public static List<list_car_type> getCarTypes()
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

        public static string addUpdateDriver(driver dri)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    db.drivers.Add(dri);
                    db.SaveChanges();
                }
                return "Đăng ký thành công";
            }
            catch (Exception ex)
            {
                return "Đăng ký thất bại: " + ex.Message;
            }
        }
    }
}