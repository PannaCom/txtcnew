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
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using System.Data.Entity.SqlServer;
using System.Security.Authentication;
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
        public string ApiArray(string status, Dictionary<string, string> field, string message)
        {
            string data = "[{";
            for (int i = 0; i < field.Count; i++)
            {
                data += "\"" + field.ElementAt(i).Key + "\":" + field.ElementAt(i).Value + ",";
            }
            if (data.EndsWith(",")) data = data.Substring(0, data.Length - 1);
            data += "}]";
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
        //1. Gửi lên hỏi server giá của tất cả các ô tô dạng 4,5,8 chỗ (car_size) 
        //Server trả về hết các bảng giả listprice quy ước như sau
        // price01way (đường dài 1 chiều)
        // price02way (đường dài 2 chiều)
        // price11way(Đường ngắn 1 chiều)       
        //3. nếu lỗi server trả error, listprice là rỗng và lỗi chi tiết
        //[HttpPost]
        public string getPostage()
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                //if (car_type == 0)
                //{
                //    int? price = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().price;
                //    int? factor = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().multiple;
                //    int? price2 = price;
                //    price2 = (int)((factor * price) / 100);// Nếu đi 1 chiều thì nhân hệ số, giá gốc là đi khứ hồi
                //    field.Add("price2Way", price.ToString());
                //    field.Add("price1Way", price2.ToString());
                //    return Api("success", field, "Giá đi đường dài với số chỗ là " + car_size.ToString());
                //}
                //else
                //{
                //    int? price = db.car_price.Where(o => o.car_size == car_size).FirstOrDefault().price2;
                //    field.Add("price", price.ToString());
                //    return Api("success", field, "Giá đi nội thành với số chỗ là " + car_size.ToString());
                //}
                var pr = (from q in db.car_price select new { car_size = q.car_size, price01way = q.price * q.multiple2 / 100, price02way = q.price,price11way=q.price2 }).OrderBy(o => o.car_size).ToList();
                field.Add("listprice", JsonConvert.SerializeObject(pr));
                return ApiArray("success", field, "Tất cả bảng giá xe đi đường dài 1 chiều, 2 chiều, và nội thành 1 chiều");
            }
            catch (Exception ex)
            {
                field.Add("listprice", "");
                return ApiArray("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //C6
        // Gửi đặt xe với user_id là id của khách, số phone custom_phone, tên custom_name của khách, nếu đặt hộ thì gửi thêm guest_phone,guest_name của khách được đặt hộ
        // loại xe car_type 4,5,8.. chỗ,  điểm đi start_point_name, danh sách điểm đến list_end_point_name, tọa độ lat,lon điểm đi start_point ví dụ cấu trúc start_point=lat,lon, danh sách tọa độ lat,lon điểm đến theo cấu trúc list_end_point=lat1,lon1_lat2,lon2_lat3,lon3...
        //isOneWay=0 là hai chiều, =1 là 1 chiều, isMineTrip=0 là đặt hộ, =1 là đặt riêng, estimated_price là ước lượng giá, estimated_distance là ước lượng khoảng cách, start_time là giờ đi, nếu đi ngay thì là null, come_back_time là giờ về nếu có, custom_note là ghi chú của khách
        //Giá trị trả về là id của chuyến xe được đặt này date_time là thời gian đặt
        [HttpPost]
        public string bookingGrab(long user_id,string custom_phone,string custom_name,string guest_phone,string guest_name, int car_type, string start_point_name, string list_end_point_name, string start_point, string list_end_point, byte? isOneWay, byte? isMineTrip, double estimated_price, double estimated_distance, DateTime? start_time, DateTime? come_back_time, string custom_note)
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
                bn.custom_name = custom_name;
                bn.guest_phone = guest_phone;
                bn.guest_name = guest_name;
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
                findDriverSuitable(bn.id);
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
        public const String certificatePass = "txvn";
        public const String certificateHostName = "gateway.sandbox.push.apple.com";
        public const string fcmAppId = "AIzaSyCo-B6yPRG6J5cu2ZK1osxgx5BRu6FXcjg";//"AIzaSyCo-B6yPRG6J5cu2ZK1osxgx5BRu6FXcjg";//"AAAA5m5BKBw:APA91bFRUOhKeAPN5f71MgQLwSGySqQ1yuhuIzXVLzZIniiB0yQu6Il8vd2tDEb0-NHDAjIeFkc3yIUeDK2NtUE0dDT4DZy9PgL0ZLX89OYZDwSWLklfV0RsgxUn_QS5qcgSEWCJUu5u3FysN9zDkcb1hiKwtsyX7g";//"AIzaSyCo-B6yPRG6J5cu2ZK1osxgx5BRu6FXcjg";
        public const string fcmSenderId = "989692241948";//"989692241948";;
        public const Int32 port = 2195;
        public X509Certificate2 clientCertificate;
        public X509Certificate2Collection certificatesCollection;
        public TcpClient client;
        public WebRequest tRequest;
        public SslStream sslStream;
        public int sendNotify(int? os, string regId, string title, string body)
        {
            try
            {
                if (os == 1)
                {
                    InitAuthForAndroid();
                    return PushMessageForAndroid(regId, title, body);
                }
                else
                {
                    InitAuthForIOS();
                    return PushMessageForIOS(regId, title, body);

                }
            }
            catch
            {
                return -1;
            }

        }

        public void InitAuthForIOS()
        {
            String certificateFile = System.Web.Hosting.HostingEnvironment.MapPath("/APNsNew.p12");
            //clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificateFile), certificatePass);
            clientCertificate = new X509Certificate2(certificateFile, certificatePass,
X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet |
X509KeyStorageFlags.PersistKeySet);
            certificatesCollection = new X509Certificate2Collection(clientCertificate);
        }

        public void InitAuthForAndroid()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "POST";
            tRequest.UseDefaultCredentials = true;
            tRequest.PreAuthenticate = true;
            tRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            //định dạng JSON
            tRequest.ContentType = "application/json";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmAppId));
            tRequest.Headers.Add(string.Format("Sender: id={0}", fcmSenderId));
        }

        public int PushMessageForIOS(string deviceId, string title, string body)
        {
            int sended = 0;
            try
            {
                string payload = "{\"aps\" : {\"alert\" : {\"title\" :\"" + title + "\",\"body\" :\"" + body + "\", \"action-loc-key\" : \"PLAY\"}, \"badge\" : 1, \"sound\":\"default\"}}";


                client = new TcpClient(certificateHostName, 2195);
                sslStream = new SslStream(client.GetStream(), false);
                sslStream.AuthenticateAsClient(certificateHostName, certificatesCollection, SslProtocols.Tls, false);

                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                writer.Write(HexStringToByteArray(deviceId.ToUpper()));
                //String payload = "{\"aps\":{\"alert\":\"" + messager + "\",\"badge\":1,\"sound\":\"default\"}}";
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Close();


                sended = 1;
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                client.Close();
            }

            return sended;
        }
        public int PushMessageForAndroid(string regId, string title, string body)
        {
            int sended = 0;
            try
            {
                if (regId != null)
                {
                    //string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + title + ";" + hinhanh + "\",\"id\":\"" + strhethongst + "\"}}"; //"\",\"dsanh\":\"" + dsanh +
                    string RegArr = String.Empty;
                    RegArr = string.Join("\",\"", regId);
                    string postData = "{ \"registration_ids\": [ \"" + RegArr + "\" ],\"data\": {\"message\": \"" + title + "\",\"body\": \"" + body + "\",\"title\": \"" + title + "\",\"collapse_key\":\"" + body + "\"}}";

                    //string postData = Convert.ToBase64String(fileBytes);

                    Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    tRequest.ContentLength = byteArray.Length;

                    Stream dataStream = tRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse tResponse = tRequest.GetResponse();

                    dataStream = tResponse.GetResponseStream();

                    StreamReader tReader = new StreamReader(dataStream);

                    string sResponseFromServer = tReader.ReadToEnd();

                    tReader.Close();
                    dataStream.Close();
                    tResponse.Close();

                    var json = JObject.Parse(sResponseFromServer);
                    var xyz = json["success"].ToString();
                    if (xyz != "0")
                    {
                        sended = 1;
                    }
                }
                return sended;
            }
            catch (Exception ex)
            {
                return sended;
            }
        }
        public static byte[] HexStringToByteArray(string hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            byte[] bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception : " + ex.Message);
            }
            //return the byte array
            return bs;
        }
        //Tìm tài xế đường ngắn gần nhất
        public void findDriverSuitable(long id)
        {
            //,string start_point_name, string list_end_point_name, string custom_phone, string custom_name, double? estimated_price, double? estimated_distance,float lat, float lon
            try
            {
                int date_id = Config.datetimeid();
                var booking = db.bookingnois.Find(id);
                DateTime? lastcancel=DateTime.Now.AddMinutes(5);
                var cancel_list = db.cancel_booking_log.Where(o => o.date_id >= date_id && o.date_time >= lastcancel);
                //Tìm tài xế gần nhất mà chưa bị hủy chuyến trong 5 phút gần đây, online trong vòng 1 tiếng gần đây và gần với khách nhất
                var startGuest = new { Latitude = booking.start_point_lat, Longitude = booking.start_point_lon };
                DateTime? latest = DateTime.Now.AddHours(-1);
                var driver = db.list_online.Where(o => o.car_type == 1 && o.status == 0 && o.date_time >= latest).Where(p => !cancel_list.Any(p2 =>p2.date_id>=date_id && p2.driver_phone ==p.phone)).OrderBy(x => 12742 * SqlFunctions.Asin(SqlFunctions.SquareRoot(SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.lat - startGuest.Latitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.lat - startGuest.Latitude)) / 2) +
                                    SqlFunctions.Cos((SqlFunctions.Pi() / 180) * startGuest.Latitude) * SqlFunctions.Cos((SqlFunctions.Pi() / 180) * (x.lat)) *
                                    SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.lon - startGuest.Longitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.lon - startGuest.Longitude)) / 2)))).FirstOrDefault();//Take(5).
                //Gửi tin báo cho tài xế này
                //tìm thông tin tài xế này
                //var dr = db.drivers.Where(o => o.phone == driver.phone).FirstOrDefault();
                if (driver != null)
                {
                    sendNotify(driver.os, driver.reg_id, id.ToString(), "Khách đi từ " + booking.start_point_name + " đến " + booking.list_end_point_name);
                }
                else
                {
                    var ctm = db.customers.Find((long)booking.user_id);
                    sendNotify(ctm.os, ctm.regId, "Hiện chưa tìm được tài xế nào cho chuyến đi của bạn!", "Các tài xế đều bận hoặc chưa có ai đăng ký đường ngắn lúc này!");
                }
            }catch{
                var booking = db.bookingnois.Find(id);
                var ctm = db.customers.Find((long)booking.user_id);
                sendNotify(ctm.os, ctm.regId, "Hiện chưa tìm được tài xế nào cho chuyến đi của bạn!", "Các tài xế đều bận hoặc chưa có ai đăng ký đường ngắn lúc này!");
            }
        }
        [HttpPost]
        //Tài xế có id là driver_id nhận chuyến đi id_booking này và gửi báo tin đến cho khách đặt qua app ứng dụng
        //Trả về là số phone, tên, user_id của khách đặt chuyến
        //Lỗi trả về rỗng
        public string receivedTrip(long id_booking,long driver_id){
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                var booking = db.bookingnois.Find(id_booking);
                var driver = db.drivers.Find(driver_id);
                var ctm = db.customers.Find((long)booking.user_id);
                sendNotify(ctm.os, ctm.regId, "Tài xế " + driver.name, driver.phone);
                db.Database.ExecuteSqlCommand("update bookingnoi set driver_id=" + driver_id + ", status_booking=1 where id=" + id_booking);
                field.Add("user_id", booking.user_id.ToString());
                field.Add("user_id", booking.user_id.ToString());           
                return Api("success", field, "Đã gửi số tài xế cho khách");
            }
            catch (Exception ex)
            {
                field.Add("user_id", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        [HttpPost]
        //Tài xế có id là driver_id không nhận chuyến đi id_booking này do server phân công,  và gửi báo server, server phải điều hành gửi đến cho tài xế khác ngay, bỏ qua tài xế này cho lần phân xe sau
        //Trả về là đồng ý tìm tài xế khác, id_booking chuyến xe, đồng thời gọi hàm tìm tài xế khác cho chuyến xe này
        public string noReceivedTrip(long id_booking, long driver_id)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                var booking = db.bookingnois.Find(id_booking);
                var driver = db.drivers.Find(driver_id);
                cancel_booking_log cbl=new cancel_booking_log();
                cbl.custom_phone = booking.custom_phone;
                cbl.date_id = Config.datetimeid();
                cbl.date_time = DateTime.Now;
                cbl.driver_id = driver_id;
                cbl.driver_phone = driver.phone;
                cbl.id_booking = id_booking;
                cbl.type_cancel = 0;//tài xế hủy, =1 là khách hủy
                cbl.user_id = booking.user_id;
                db.cancel_booking_log.Add(cbl);
                db.SaveChanges();
                findDriverSuitable(id_booking);
                field.Add("user_id", booking.user_id.ToString());
                return Api("success", field, "Tài xế đã bỏ không nhận chuyến xe này. Server đã tìm tài xế khác.");
            }
            catch (Exception ex)
            {
                field.Add("user_id", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
        //Sau khi đi xong nhớ báo cho server biết là chuyến xe kết thúc, tài xế ấn vào nút xác nhận, server sẽ đánh dấu trạng thái chuyến xe đã hoàn thành
        //Server trả vế thông báo xác nhận chuyến đi thành công và số km, số tiền sẽ được thanh toán như lúc đầu
        //Lỗi trả về id_booking rỗng và lỗi chi tiết
        [HttpPost]
        public string confirmTrip(long id_booking,long driver_id)
        {
            Dictionary<string, string> field = new Dictionary<string, string>();
            try
            {
                var booking = db.bookingnois.Find(id_booking);
                var driver = db.drivers.Find(driver_id);
                booking_noi_history bnh = new booking_noi_history();
                bnh.custom_name = booking.custom_name;
                bnh.custom_phone = booking.custom_phone;
                bnh.date_id = Config.datetimeid();
                bnh.date_time = DateTime.Now;
                bnh.driver_id = driver_id;
                bnh.driver_name = driver.name;
                bnh.driver_phone = driver.phone;
                bnh.end_point = booking.list_end_point_name;
                bnh.id_booking = id_booking;
                bnh.start_point = booking.start_point_name;
                bnh.user_id = booking.user_id;
                db.booking_noi_history.Add(bnh);
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("update bookingnoi set status_booking=2 where id=" + id_booking);
                field.Add("distance", booking.distance.ToString());
                field.Add("price", booking.price.ToString());
                field.Add("date_time", DateTime.Now.ToString());
                return Api("success", field, "Đặt xe thành công");
            }
            catch (Exception ex)
            {
                field.Add("id_booking", "");
                return Api("error", field, "Lỗi server!" + ex.ToString());
            }
        }
       
        //[HttpPost]
        //Trả về các chuyến xe do khách đặt trong bán kính 1km
        public string getListBookingNoiArround(double lat, double lon)
        {
            double distance = 12742 * SqlFunctions.Asin(SqlFunctions.SquareRoot(SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (21.022703 - startGuest.Latitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (21.022703 - startGuest.Latitude)) / 2) +
                                   SqlFunctions.Cos((SqlFunctions.Pi() / 180) * startGuest.Latitude) * SqlFunctions.Cos((SqlFunctions.Pi() / 180) * (x.start_point_lat)) *
                                   SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lon - startGuest.Longitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lon - startGuest.Longitude)) / 2)));
            //Dictionary<string, string> field = new Dictionary<string, string>();
            //try
            //{
            //    DateTime booktime=DateTime.Now.AddMinutes(-1);
            //    var startGuest = new { Latitude = lat, Longitude = lon };
            //    var driver = db.bookingnois.Where(o => o.book_time>=booktime && o.car_type == 1).Where(x => 12742 * SqlFunctions.Asin(SqlFunctions.SquareRoot(SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lat - startGuest.Latitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lat - startGuest.Latitude)) / 2) +
            //                       SqlFunctions.Cos((SqlFunctions.Pi() / 180) * startGuest.Latitude) * SqlFunctions.Cos((SqlFunctions.Pi() / 180) * (x.start_point_lat)) *
            //                       SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lon - startGuest.Longitude)) / 2) * SqlFunctions.Sin(((SqlFunctions.Pi() / 180) * (x.start_point_lon - startGuest.Longitude)) / 2)))<=1000).Take(10);
            //    field.Add("bookinglist", );                
            //    return Api("success", field, "Đặt xe thành công");
            //}
            //catch (Exception ex)
            //{
            //    field.Add("id_booking", "");
            //    return Api("error", field, "Lỗi server!" + ex.ToString());
            //}
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
