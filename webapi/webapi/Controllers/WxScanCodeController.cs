using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dal;
using System.Data;

namespace webapi.Controllers
{
    public class WxScanCodeController : ApiController
    {
        private readonly BLL.HN_Customer bll = new BLL.HN_Customer();

        [System.Web.Http.HttpPost]
        /// <summary>
        /// Weixin_ScanCode项目，add用户，公众号扫码使用
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        public int addWeixin_ScanCode_user(Weixin_ScanCode_user newperson)
        {

            return bll.addWeixin_ScanCode_user(newperson);
        }

        [System.Web.Http.HttpPost]
        /// <summary>
        /// Weixin_ScanCode项目，add扫码记录，公众号扫码使用
        /// </summary>
        /// <param name="record类"></param>
        /// <returns></returns>
        public int addWeixin_ScanCode_record(Weixin_ScanCode_record newrecord)
        {
            
            return bll.addWeixin_ScanCode_record(newrecord);
        }


        [System.Web.Http.HttpPost]
        /// <summary>
        /// Weixin_ScanCode项目 使用openid返回相关信息，公众号扫码使用
        /// </summary>
        /// <param name="classOpenid"></param>
        /// <returns></returns>
        public classScanUser getWeixin_ScanCode_user(classOpenid strOpenid)
        {

            DataTable dt = bll.getWeixin_ScanCode_user(strOpenid.openid);
            DataRow dr = dt.Rows[0];
            classScanUser myuser = new classScanUser();
            myuser.OpenId = dr["OpenId"].ToString();
            myuser.NickName = dr["NickName"].ToString();
            myuser.Avatar = dr["Avatar"].ToString();
            myuser.Gender = dr["Gender"].ToString();
            myuser.DataFlag = dr["DataFlag"].ToString();
            myuser.Name = dr["Name"].ToString();
            myuser.IsDel = dr["IsDel"].ToString();
            myuser.telephone = dr["telephone"].ToString();
            return myuser;
        }


        [System.Web.Http.HttpPost]
        /// <summary>
        /// 利用openid，查询record记录
        /// </summary>
        /// <returns></returns>
        public IList<class_Weixin_ScanCode_record> getWeixin_ScanCode_record(Weixin_ScanCode_openid openid)

        {
            string my_openid = openid.user_openid;
            DataTable mytable = bll.getWeixin_ScanCode_record_top20(my_openid);
            IList<class_Weixin_ScanCode_record> mylist = ModelConvertHelper.DataTableToList<class_Weixin_ScanCode_record>(mytable);
            return mylist;

        }


        public class classOpenid
        {
            public string openid { get; set; }

        }

        public class classScanUser
        {
            public string OpenId { get; set; }
            public string NickName { get; set; }
            public string Avatar { get; set; }
            public string Gender { get; set; }
            public string DataFlag { get; set; }
            public string Name { get; set; }
            public string IsDel { get; set; }
            public string telephone { get; set; }



        }
        public class class_Weixin_ScanCode_record
        {
            public string AccountBasic { get; set; }   
            public string point_id { get; set; }   
            public string user_openid { get; set; }   
            public string scan_time { get; set; }  


        }

        public class Weixin_ScanCode_openid
        {
            public string user_openid { get; set; }
        }
    }
}
