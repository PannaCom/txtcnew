using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using ThueXeToanCau.Models;
using Newtonsoft.Json;
namespace ThueXeToanCau.Controllers
{
    public class Api2Controller : ApiController
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
        public string Api(string status, Dictionary<string,string> field, string message)
        {
            string data = "[{";
            for (int i = 0; i < field.Count; i++)
            {
                data += "\"" + field.ElementAt(i).Key + "\":\"" + field.ElementAt(i).Value + "\",";
            }
            if (data.EndsWith(",")) data = data.Substring(0, data.Length - 1);
            data +="}]";
            string temp = "{\"status\":\"" + status + "\",\"data\":" + data + ",\"message\":\"" + message + "\"}";
            return temp;
        }
        //Tài xế đăng ký hoặc cập hật thông tin
        //1.Đăng ký: Trả về status là success nếu thành công, data là các trường trả về, message là dòng thông báo
        //2.Cập nhật: Trả về status là success nếu thành công, data là các trường trả về, message là dòng thông báo
        //3.Trả về lỗi status:error,data là các trường trả về , message là dòng thông báo
        //Các tham số đã biết ở API cũ, có thêm tham số driver_type gửi lên =0 nếu là đường dài, =1 nếu là taxi nội thành
        [HttpPost]
        public string driverRegister(int? id, string name, string phone, string pass, string car_made, string car_model, int car_size, int car_year, string car_number, string car_type, string card_identify, string license, string regId, int? os, int? driver_type)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                
                if (id == null)
                {
                    driver r = new driver();
                    r.name = name;
                    r.phone = phone;
                    MD5 md5Hash = MD5.Create();
                    string hash = Config.GetMd5Hash(md5Hash, pass);
                    r.pass = hash;
                    r.car_made = car_made;
                    r.car_model = car_model;
                    r.car_size = car_size;
                    r.car_years = car_year;
                    r.car_number = car_number;
                    r.car_type = car_type;
                    r.date_time = DateTime.Now;
                    r.card_identify = card_identify;
                    r.license = license;
                    r.reg_id = regId;
                    r.os = os;
                    r.total_moneys = 1000000;
                    r.status = 0;
                    r.driver_type = driver_type;
                    db.drivers.Add(r);
                    db.SaveChanges();
                    notify nt = new notify();
                    nt.os = os;
                    nt.reg_id = regId;
                    nt.tobject = 1;
                    db.notifies.Add(nt);
                    db.SaveChanges();
                    field.Add("id", r.id.ToString());
                    return Api("success", field, "Đăng ký tài xế thành công, đợi kích hoạt!");
                }
                else
                {
                    driver r = db.drivers.Find(id);
                    db.Entry(r).State = EntityState.Modified;
                    r.name = name;
                    r.phone = phone;
                    r.car_made = car_made;
                    r.car_model = car_model;
                    r.car_size = car_size;
                    r.car_years = car_year;
                    r.car_number = car_number;
                    r.car_type = car_type;
                    r.date_time = DateTime.Now;
                    r.card_identify = card_identify;
                    r.license = license;
                    r.reg_id = regId;
                    r.os = os;
                    r.driver_type = driver_type;
                    db.SaveChanges();
                    field.Add("id", id.ToString());
                    return Api("success", field, "Cập nhật tài xế thành công!");
                }

               
            }
            catch (Exception ex)
            {
                field.Add("id", "0");
                return Api("error", field, "Cập nhật lỗi sql!");
            }
        }
        //C1.
        //Khách hàng đăng ký hoặc cập nhật thông tin
        //Đăng ký: Trả về status là success nếu thành công, data trả về là id cùng với token mới của khách hàng mới đăng ký, message là dòng thông báo
        //Cập nhật: Trả về status là success nếu thành công, data trả về là token của khách hàng vừa cập nhật, message là dòng thông báo
        //Trả về lỗi status:error,data là token rỗng , message là dòng thông báo lỗi chi tiết
        //Các tham số bên dưới, regId là để thông báo sau này, os là hệ điều hành =1 là android, =2 là ios, =3 là đăng ký qua web, do vậy khi login bằng điện thoại cần cập nhật lại OS
        //(Nhắc để nhớ lại:....Hàm đó là hàm API cũ gọi ở cú pháp sau API/login(string phone,string pass,int? os,string regId)...)
        [HttpPost]
        public string customRegister(long? id,string custom_phone,string custom_email,string custom_name,string regId,int? os)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            string token = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
            try
            {
                
                if (id == null || id==0)
                {
                    bool duplicate = db.customers.Any(o => o.custom_email == custom_email || o.custom_phone == custom_phone);
                    if (duplicate)
                    {
                        field.Add("token", "");
                        return Api("failed", field, "Đã tồn tại email hoặc số điện thoại này rồi");
                    }
                    customer r = new customer();
                    r.custom_email = custom_email;
                    r.custom_name = custom_name;
                    r.custom_phone = custom_phone;
                    r.date_reg = DateTime.Now;
                    r.last_update = DateTime.Now;
                    r.os = os;
                    r.regId = regId;
                    r.token = token;                   
                    db.customers.Add(r);
                    db.SaveChanges();
                    notify nt = new notify();
                    nt.os = os;
                    nt.reg_id = regId;
                    nt.tobject = 1;
                    db.notifies.Add(nt);
                    db.SaveChanges();
                    field.Add("id", r.id.ToString());
                    field.Add("token", token);
                    return Api("success", field, "Đăng ký tài xế thành công, đợi kích hoạt");
                }
                else
                {
                    customer r = db.customers.Find(id);
                    db.Entry(r).State = EntityState.Modified;
                    r.custom_email = custom_email;
                    r.custom_name = custom_name;
                    r.custom_phone = custom_phone;
                    r.last_update = DateTime.Now;
                    r.os = os;
                    r.regId = regId;                    
                    db.SaveChanges();
                    field.Add("token", token);
                    return Api("success", field, "Cập nhật tài xế thành công!");
                }

            }
            catch (Exception ex)
            {
                field.Add("token", "");
                return Api("error", field, "Cập nhật lỗi server! " + ex.ToString());
            }
        }
        //C2
        //1.Trả về data là token, và id của user này và success, dòng thông báo đăng nhập nếu tồn tại email và phone, đồng thời cập nhật regId và os cho customer này trường hợp khách đổi máy vẫn luôn có regId và os mới (os là hệ điều hành)
        //2.Trả về data là token và failed, dòng thông báo không tồn tại phone hoặc email nếu không tồn tại thông tin 
        //3.Trả về data là token rỗng và error nếu server lỗi, kèm lỗi chi tiết server trả về trường hợp debug xem còn biết sửa API
        //4. Trả về cả chuyến xe đặt gần nhất booking_data dạng json nếu có, dựa vào chuyến xe mới đặt gần nhất này mà client quyết định có nên chuyển đến màn hình đợi hay không, nếu không có thì trả về rỗng
        [HttpPost]
        public string customerLogin(string custom_phone, string custom_email,string regId, int? os)
        {
           Dictionary<string, string> field = new Dictionary<string, string>();
           string token = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
           try 
           {
               if (db.customers.Any(o => (o.custom_phone == custom_phone || o.custom_email == custom_email)))// && o.regId==regId
                {
                    db.Database.ExecuteSqlCommand("update customer set regId=N'" + regId + "',os=" + os + ",token=N'" + token + "' where (custom_email=N'" + custom_email + "' or custom_phone=N'" + custom_phone + "')");// and regId=N'" + regId + "'
                    field.Add("token", token);
                    long user_id = 0;
                    try
                    {
                        var ui = db.customers.Where(o => o.custom_phone == custom_phone || o.custom_email == custom_email).FirstOrDefault();
                        user_id = ui.id;
                        field.Add("user_id", ui.id.ToString());
                        field.Add("custom_email", ui.custom_email);
                        field.Add("custom_phone", ui.custom_phone);
                        field.Add("custom_name", ui.custom_name);
                    }catch{

                    }
                    try
                    {
                        var bk = db.bookingnois.Where(o => o.user_id == user_id && o.status_booking==0 && o.status_payment==0).LastOrDefault();
                        field.Add("booking_data", JsonConvert.SerializeObject(bk));
                    }
                    catch
                    {
                        field.Add("booking_data", "");
                    }
                    //field.Add("user_id", user_id.ToString());

                    return Api("success", field, "Đăng nhập thành công!");
                }
                else
                {
                    field.Add("token", token);
                    return Api("failed", field, "Không tồn tại số phone hoặc email này!");
                }
            }
            catch (Exception ex)
            {
                field.Add("token", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C3.
        //1. nếu tồn tại token này thì trả về success,1 và thông báo có user này, cập nhật regId và os cho user có token này
        //2. nếu không tồn tại token này thì trả về failed, 0 và thông báo không có user này
        //3. nếu lỗi server trả error, token rỗng và lỗi chi tiết
        [HttpPost]
        public string customerCheckToken(string token,string regId, int? os)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                if (db.customers.Any(o => o.token == token))
                {
                    db.Database.ExecuteSqlCommand("update customer set regId=N'" + regId + "',os=" + os + " where token=N'" + token + "'");
                    field.Add("token", "1");
                    var us = db.customers.Where(o => o.token == token).FirstOrDefault();
                    field.Add("user_id", us.id.ToString());
                    field.Add("custom_email", us.custom_email);
                    field.Add("custom_phone", us.custom_phone);
                    field.Add("custom_name", us.custom_name);
                    return Api("success", field, "Đăng nhập thành công!");
                }
                else
                {
                    field.Add("token", "0");
                    return Api("failed", field, "Không tồn tại token này!");
                }
            }
            catch (Exception ex)
            {
                field.Add("token", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C4
        //1. nếu tồn tại token này thì trả về success,trường token là 1 và thông báo logout thành công, xóa token hiện tại
        //2. nếu không tồn tại token này thì trả về failed, token là 0 và thông báo logout không thành công
        //3. nếu lỗi server trả error, token là rỗng và lỗi chi tiết
        [HttpPost]
        public string customerLogOut(string token)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                if (db.customers.Any(o => o.token == token))
                {
                    db.Database.ExecuteSqlCommand("update customer set token=N'' where token=N'" + token + "'");
                    field.Add("token", "1");
                    return Api("success", field, "logout thành công!");
                }
                else
                {
                    field.Add("token", "0");
                    return Api("failed", field, "logout không thành công!");
                }
            }
            catch (Exception ex)
            {
                field.Add("token", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C5
        //1. Gửi lên hỏi server là với ô tô dạng 4,5,8 chỗ (car_size) đi nội thành hoặc đường dài (car_type=0 là đường dài, car_type=1 là nội thành, với đường dài thì trả về cả giá 1 chiều và cả giá khứ hồi
        //2. Server trả về giá price/1km để client tính toán nhân với km trên bản đồ client đo được
        //3. nếu lỗi server trả error, price là 0 và lỗi chi tiết
        //[HttpPost]
        public string getPostage(int car_size,int car_type,int? way)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                if (car_type == 0)
                {
                    int? price = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().price;
                    int? factor = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().multiple;
                    int? price2 = price;
                    price2 = (int)((factor * price) / 100);// Nếu đi 1 chiều thì nhân hệ số, giá gốc là đi khứ hồi
                    field.Add("price2Way", price.ToString());
                    field.Add("price1Way", price2.ToString());
                    return Api("success", field, "Giá đi đường dài với số chỗ là " + car_size.ToString());
                }
                else
                {
                    int? price = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().price2;
                    field.Add("price", price.ToString());
                    return Api("success", field, "Giá đi nội thành với số chỗ là " + car_size.ToString());
                }
            }
            catch (Exception ex)
            {
                field.Add("price", "0");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C6
        // Gửi đặt xe với user_id là id của khách, số phone custom_phone của khách, loại xe car_type 4,5,8 chỗ,  điểm đi start_point_name, danh sách điểm đến list_end_point_name, tọa độ lat,lon điểm đi start_point ví dụ cấu trúc start_point=lat,lon, danh sách tọa độ lat,lon điểm đến theo cấu trúc list_end_point=lat1,lon1_lat2,lon2_lat3,lon3...
        //isOneWay=0 là hai chiều, =1 là 1 chiều, isMineTrip=0 là đặt hộ, =1 là đặt riêng, estimated_price là ước lượng giá, estimated_distance là ước lượng khoảng cách, start_time là giờ đi, nếu đi ngay thì là null, come_back_time là giờ về nếu có, custom_note là ghi chú của khách
        //Giá trị trả về là id của chuyến xe được đặt này date_time là thời gian đặt
        [HttpPost]
        public string bookingGrab(long user_id,string custom_phone, int car_type, string start_point_name, string list_end_point_name, string start_point, string list_end_point, byte? isOneWay, byte? isMineTrip, double estimated_price, double estimated_distance, DateTime? start_time, DateTime? come_back_time, string custom_note)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                int date_id = Config.datetimeid();
                //if (db.bookingnois.Any(o => o.book_date_id >= date_id && o.custom_phone == custom_phone))
                //{
                //    DateTime? dt = db.bookingnois.Where(o => o.book_date_id >= date_id && o.custom_phone == custom_phone).LastOrDefault().book_time;

                //}
                bookingnoi bn = new bookingnoi();
                if (come_back_time!=null) bn.back_time = come_back_time.Value.TimeOfDay;
                bn.book_date_id = date_id;
                bn.book_time = DateTime.Now;
                bn.car_type = car_type;
                bn.custom_phone = custom_phone;
                bn.distance = estimated_distance;
                bn.is_mine_trip = isMineTrip;
                bn.is_one_way = isOneWay;
                //bn.list_end_point = list_end_point;
                string[] arr_list_end_point = list_end_point.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Split('_');
                bn.list_end_point_lat = float.Parse(arr_list_end_point[0].Split(',')[0]);
                bn.list_end_point_lon = float.Parse(arr_list_end_point[0].Split(',')[1]);
                bn.list_end_point = JsonConvert.SerializeObject(arr_list_end_point);
                bn.list_end_point_name = list_end_point_name;
                bn.note = custom_note;
                bn.price = estimated_price;
                string[] arr_start_point = start_point.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Split('_');
                bn.start_point_lat = float.Parse(arr_start_point[0].Split(',')[0]);
                bn.start_point_lon = float.Parse(arr_start_point[0].Split(',')[1]);
                bn.start_point_name = start_point_name;
                if (start_time != null) bn.start_time = start_time.Value.TimeOfDay;
                bn.status_booking = 0;
                bn.status_payment = 0;
                bn.user_id = user_id;
                db.bookingnois.Add(bn);
                db.SaveChanges();
                field.Add("id_booking", bn.id.ToString());
                field.Add("date_time", DateTime.Now.ToString());      
                return Api("success", field, "Đặt xe thành công");
            }
            catch (Exception ex)
            {
                field.Add("id_booking", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        [HttpPost]
        public string template()
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                field.Add("id_booking", "");
                field.Add("date_time", DateTime.Now.ToString());
                return Api("success", field, "Đặt xe thành công");
            }
            catch (Exception ex)
            {
                field.Add("id_booking", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C8
        //1. Hủy chuyến xe đã đặt với id_booking, cancel_reason là lý do đặt, còn custom_phone để làm gì nhỉ?
        //2. Nếu không tìm thấy id_booking này thì báo không có chuyến này và id_booking rỗng
        //3. Nếu lỗi báo lỗi server id_booking rỗng
        // Cập nhật trạng thái chuyến xe bị hủy, status_booking=0 là vừa đặt, =1 là có tài xế đón rôi, =-1 là bị hủy
        [HttpPost]
        public string cancelTrip(long id_booking, string custom_phone, string cancel_reason)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                if (db.bookingnois.Any(o => o.id == id_booking))
                {
                    db.Database.ExecuteSqlCommand("update bookingnoi set status_booking=-1 where id=" + id_booking);
                    field.Add("id_booking", id_booking.ToString());
                    field.Add("date_time", DateTime.Now.ToString());
                    return Api("success", field, "Hủy đặt xe thành công");
                }
                else
                {
                    field.Add("id_booking", "");
                    return Api("failed", field, "Không tìm thấy chuyến xe này để hủy");
                }
            }
            catch (Exception ex)
            {
                field.Add("id_booking", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
    }
}
