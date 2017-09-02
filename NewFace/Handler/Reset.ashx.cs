using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFace.Handler
{
    /// <summary>
    /// Reset 的摘要说明
    /// </summary>
    public class Reset : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current.Application["detect"] = null;

            context.Response.Write("ok");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}