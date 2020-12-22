using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;

namespace webapi.Controllers
{
    public class WechatJSSDKController : ApiController
    {
        /// <summary>
        /// 微信JSSDK基础信息
        /// </summary>

        private static string appid = ConfigurationManager.AppSettings["appID"];

        private static string appsecret = ConfigurationManager.AppSettings["appsecret"];

        /// <summary>
        /// 获取微信配置基础信息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public WXShare GetWxShareInfo(URLShare link_url)
        {
            URLShare my_url = link_url;
            string url = my_url.url;
            
            var context = HttpContext.Current;
            //string url = context.Request["url"].ToString();
            //url = HttpContext.Current.Request.Url.ToString();
            
            DateTime now = DateTime.Now;
            var timestamp = GetTimeStamp(now);//取十位时间戳


            //var guid = Guid.NewGuid().ToString("N");//随机串
            var guid = GetNonceStr();

            var ticket = "";//签名密钥
            try
            {
                WXShare s = new WXShare();
                //取缓存中的Ticket，没有则重新生成Ticket值（也可以将Ticket值保存到文件中，此时从文件中读取Ticket）
                var Cache = CacheHelper.GetCache("ticket");
                if (Cache == null) CacheHelper.SetCache("ticket", GetTicket(), 7000);
                ticket = CacheHelper.GetCache("ticket").ToString();

                url = HttpUtility.UrlDecode(url);//url解码

                string sign = GetSignature(ticket, guid, timestamp, url);
                s.appid = appid;
                s.noncestr = guid;
                s.timestamp = timestamp;
                s.signature = sign;
                s.ticket = ticket;
                s.url = url;

                //logger.Warn($"url:{url},时间戳:{timestamp},随机数:{guid},ticket:{ticket},sign值:{sign}");//记录日志
                return s;
            }
            catch (Exception ex)
            {
                //logger.Warn(ex);
                throw ex;
            }
        }


        /// <summary>
        /// GetTicket 签名密钥
        /// </summary>
        /// <returns></returns>
        public static string GetTicket()
        {
            string token = "";
            var Cache = CacheHelper.GetCache("token");
            if (Cache == null) CacheHelper.SetCache("token", GetAccessToken(), 7000);//获取AccessToken            
            token = CacheHelper.GetCache("token").ToString();
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["access_token"] = token;
            dic["type"] = "jsapi";
            FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
            var response = Client.PostAsync("https://api.weixin.qq.com/cgi-bin/ticket/getticket", content).Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return "";
            var result = response.Content.ReadAsStringAsync().Result;
            JObject obj = JObject.Parse(result);
            string ticket = obj["ticket"]?.ToString() ?? "";
            return ticket;
        }

        /// <summary>
        /// GetAccessToken
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["grant_type"] = "client_credential";
            dic["appid"] = appid;//自己的appid
            dic["secret"] = appsecret;//自己的appsecret
            FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
            var response = Client.PostAsync("https://api.weixin.qq.com/cgi-bin/token", content).Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return "";
            var result = response.Content.ReadAsStringAsync().Result;
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(result);
            string token = obj["access_token"]?.ToString() ?? "";
            return token;
        }

        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="ticket">ticket</param>
        /// <param name="noncestr">随机字符串</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSignature(string ticket, string noncestr, long timestamp, string url)
        {

            var string1Builder = new System.Text.StringBuilder();
            //拼接字符串
            string1Builder.Append("jsapi_ticket=").Append(ticket).Append("&")
                          .Append("noncestr=").Append(noncestr).Append("&")
                          .Append("timestamp=").Append(timestamp).Append("&")
                          .Append("url=").Append(url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url);
            string str = string1Builder.ToString();
            return SHA1(str);//加密
        }

        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }
        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位小写字符串</returns>
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "").ToLower();//转小写
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }


        //请求基类
        private static HttpClient _client = null;
        public static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    var handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip,
                        AllowAutoRedirect = false,
                        UseCookies = false,
                    };
                    _client = new HttpClient(handler);
                    _client.Timeout = TimeSpan.FromSeconds(5);
                    _client.DefaultRequestHeaders.Add("Accept", "application/json");
                }
                return _client;
            }
        }

        /// <summary>
        /// 随机字符串数组集合
        /// </summary>
        private static readonly string[] NonceStrings = new string[]
                                    {
                                    "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                    "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
                                    };

        /// <summary>
        /// 生成签名的随机串
        /// </summary>
        /// <returns></returns>
        public static string GetNonceStr()
        {
            Random random = new Random();
            var sb = new StringBuilder();
            var length = NonceStrings.Length;

            //生成15位数的随机字符串，当然也可以通过控制对应字符串大小生成，但是至多不超过32位
            for (int i = 0; i < 15; i++)
            {
                sb.Append(NonceStrings[random.Next(length - 1)]);//通过random获得的随机索引到，NonceStrings数组中获取对应数组值
            }
            return sb.ToString();
        }

        /// <summary>
        /// 十位时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }


        /// <summary>
        /// 返回实体
        /// </summary>
        public class URLShare
        {
            public string url { get; set; }
            
        }
        /// <summary>
        /// 返回实体
        /// </summary>
        public class WXShare
        {
            public string appid { get; set; }
            /// <summary>
            /// 随机码
            /// </summary>
            public string noncestr { get; set; }
            /// <summary>
            /// 时间戳
            /// </summary>
            public int timestamp { get; set; }
            /// <summary>
            /// 签名值
            /// </summary>
            public string signature { get; set; }
            /// <summary>
            /// Ticket
            /// </summary>
            public string ticket { get; set; }
            /// <summary>
            /// Ticket
            /// </summary>
            public string url { get; set; }
        }

        public class CacheHelper
        {
            /// <summary>  
            /// 获取数据缓存  
            /// </summary>  
            /// <param name="cacheKey">键</param>  
            public static object GetCache(string cacheKey)
            {
                var objCache = HttpRuntime.Cache.Get(cacheKey);
                return objCache;
            }
            /// <summary>  
            /// 设置数据缓存  
            /// </summary>  
            public static void SetCache(string cacheKey, object objObject)
            {
                var objCache = HttpRuntime.Cache;
                objCache.Insert(cacheKey, objObject);
            }
            /// <summary>  
            /// 设置数据缓存  
            /// </summary>  
            public static void SetCache(string cacheKey, object objObject, int timeout = 7200)
            {
                try
                {
                    if (objObject == null) return;
                    var objCache = HttpRuntime.Cache;
                    //相对过期  
                    //objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue,  new TimeSpan(0, 0, timeout), CacheItemPriority.NotRemovable, null);  
                    //绝对过期时间  
                    objCache.Insert(cacheKey, objObject, null, DateTime.UtcNow.AddSeconds(timeout), TimeSpan.Zero, CacheItemPriority.High, null);
                }
                catch (Exception)
                {
                    //throw;  
                }
            }
            /// <summary>  
            /// 移除指定数据缓存  
            /// </summary>  
            public static void RemoveAllCache(string cacheKey)
            {
                var cache = HttpRuntime.Cache;
                cache.Remove(cacheKey);
            }
            /// <summary>  
            /// 移除全部缓存  
            /// </summary>  
            public static void RemoveAllCache()
            {
                var cache = HttpRuntime.Cache;
                var cacheEnum = cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    cache.Remove(cacheEnum.Key.ToString());
                }
            }



            /// <summary>
            /// 初始化微信接口返回参数
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>



        }

        [System.Web.Http.HttpPost]
        public MesOpenid GetOpenid(JsonCode jsoncode)
        {
            JsonCode myJsonCode = jsoncode;
            string json_code = myJsonCode.json_code;
            MesOpenid mymes = new MesOpenid();
            try
            {

                string _posdata = "appid=wx55a26ee672104c59&secret=3ff884262a1b3822183800c822abd8c1&code=" + json_code + "&grant_type=authorization_code";
                string _url = "https://api.weixin.qq.com/sns/oauth2/access_token";//获取openid
                string _data = request_url(_url, _posdata);
                if (_data.Contains("\"openid\""))
                {

                    string _ip = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString().Trim();
                    dynamic _modal = Newtonsoft.Json.Linq.JToken.Parse(_data) as dynamic;
                    string _openid = _modal.openid;
                    string _access_token = _modal.access_token;

                    mymes.openid = _openid;
                    mymes.access_token = _access_token;
                    mymes.MsgCode = "ok";
                    return mymes;




                }
                else
                {
                    mymes.openid = "找不到OpenidID或已失效";
                    mymes.access_token = "找不到access_token或已失效";
                    mymes.MsgCode = "400";
                    return mymes;
                }

            }

            catch (Exception ex)
            {
                //this.Json(new { succeed = false, data = ex.Message }, JsonRequestBehavior.AllowGet);
                mymes.openid = "严重错误";
                mymes.access_token = ex.ToString();
                mymes.MsgCode = "400";
                return mymes;
            }

        }

        private string Json(object p, JsonRequestBehavior allowGet)
        {
            throw new NotImplementedException();
        }

        #region 请求api
        /// <summary>
                /// 请求api
                /// </summary>
                /// <param name="_url"></param>
                /// <param name="post_data"></param>
                /// <returns></returns>
        private string request_url(string _url, string post_data)
        {
            string result = ""; string url = _url;// "https://api.weixin.qq.com/sns/oauth2/access_token";

            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            //request.Headers=""
            request.Method = "POST";

            byte[] buffer = encoding.GetBytes(post_data.Trim());
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        #endregion

        #region 解密电话号码
        [System.Web.Http.HttpGet]  //网页中显示这个方法
        public string getPhoneNumber(string encryptedData, string IV, string Session_key)
        {
            try
            {


                byte[] encryData = Convert.FromBase64String(encryptedData);  // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(Session_key); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(IV);// Encoding.UTF8.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryData, 0, encryData.Length);
                string result = Encoding.Default.GetString(plainText);
                //动态解析result 成对象
                dynamic model = Newtonsoft.Json.Linq.JToken.Parse(result) as dynamic;
                return model.phoneNumber;

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return ex.ToString();

            }

        }
        #endregion

        public class MesOpenid 
        {
            public string openid { get; set; }
            public string access_token { get; set; }
            public string MsgCode { get; set; }
        }
        public class JsonCode
        {
            public string json_code { get; set; }

        }

    }
}

