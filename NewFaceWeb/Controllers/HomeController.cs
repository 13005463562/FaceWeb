using NewFaceWeb.Com;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NewFaceWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            
            return View();
        }


        public ActionResult Add()
        {
            return View();
        }


        public ActionResult AddFaceInfor(string img,string name)
        {

            string imgStr = img.Split(',')[1];
           byte[] imageBytes= Convert.FromBase64String(imgStr);

           // byte[] imageBytes = GetImgByte(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");
            //参数字典
            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();

            verifyPostParameters.Add("name", name);
            //添加图片参数
            verifyPostParameters.Add("image_file", new HttpHelper4MultipartForm.FileParameter(imageBytes, "picture.jpg", "application/octet-stream"));


            HttpWebResponse verifyResponse = HttpHelper4MultipartForm.MultipartFormDataPost("http://119.23.237.231:8082/Home/AddFromOther", "", verifyPostParameters);
            StreamReader sr = new StreamReader(verifyResponse.GetResponseStream(), Encoding.UTF8);
            string strhtml = sr.ReadToEnd();

            return Content(strhtml);
        }

        public static byte[] GetImgByte(string path)
        {
            Bitmap bmp = new Bitmap(path); // 图片地址
            using (Stream stream1 = new MemoryStream())
            {
                bmp.Save(stream1, ImageFormat.Jpeg);
                byte[] arr = new byte[stream1.Length];
                stream1.Position = 0;
                stream1.Read(arr, 0, (int)stream1.Length);
                stream1.Close();
                return arr;
            }
        }

        public static MemoryStream ChangeStringToImage(string pic)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(pic);
                //读入MemoryStream对象
                MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
                memoryStream.Write(imageBytes, 0, imageBytes.Length);
                //转成图片
                // Image image = Image.FromStream(memoryStream);

                //  return image;
                return memoryStream;
            }
            catch (Exception e)
            {
                // Image image = null;
                //return image;
                return null;
            }
        }
	}
}