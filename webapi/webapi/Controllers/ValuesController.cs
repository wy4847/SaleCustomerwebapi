using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace webapi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        [System.Web.Http.HttpGet]
        public string pay(string jscode, string encryptedData, string IV)
        {
            
            try
            {
                encryptedData = "gr6oNLITRxeTuKEqa8Ru1Up1B9CUpx5t3+nexGoQ+f3YbKds7BZvMWVcKvJbzIi0v9Zp815Vx51PFlXEkCMtUXVRGHnW5oQ3hCc02kpcuGaDivX38CI1fN1TDpiKPlpPHzqCVrg8k2/nsP3lEHeKoy2WHe64hMDeugJpV4uZxK/MLaecz3PkFta/34qeafFKGeDHNzvhpf1Kawsoq54QFA==";
                IV = "T0jHddP3or0sHhNQs5UnAg==";
                string _posdata = "appid=wxd7526ad7c10909fa&secret=5895721c889c64f8dc7d46eae6298dbe&js_code=" + jscode + "&grant_type=authorization_code";
                string _url = "https://api.weixin.qq.com/sns/jscode2session";//获取openid
                string _data = request_url(_url, _posdata);
                if (_data.Contains("\"openid\""))
                {

                    string _ip = HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString().Trim();
                    dynamic _modal = Newtonsoft.Json.Linq.JToken.Parse(_data) as dynamic;
                    string _openid = _modal.openid;
                    string _session_key = _modal.session_key;
                    if (!String.IsNullOrEmpty(encryptedData) && !string.IsNullOrEmpty(IV))
                    {
                        //解析手机号码
                        //string _telPhone = getPhoneNumber(encryptedData, IV, _session_key);
                        //return _telPhone;
                        return _openid + "&" + _session_key;
                    }
                    else
                    {
                        return "无新号码";
                    }

                }
                else
                { return "出现错误"; }

            }

            catch (Exception ex)
            {
                return this.Json(new { succeed = false, data = ex.Message }, JsonRequestBehavior.AllowGet);
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
            string result = ""; string url = _url;// "https://api.weixin.qq.com/sns/jscode2session";

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

        /// AES解密：从小程序中 getPhoneNumber 返回值中，解析手机号码
        /// </summary>
        /// <param name="encryptedData">包括敏感数据在内的完整用户信息的加密数据，详细见加密数据解密算法</param>
        /// <param name="IV">加密算法的初始向量</param>
        /// <param name="Session_key"></param>
        /// <returns>手机号码</returns>
        private string getPhoneNumber(string encryptedData, string IV, string Session_key)
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
                return ex.ToString ();

            }
        }


    }
}
