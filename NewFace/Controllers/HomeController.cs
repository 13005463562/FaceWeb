using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFace.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddSave(string Name,string Images)
        {
            var result = new { result = true, message = "保存成功！" };


            if (SaveImg(Name,Images) ==1)
            {
                result = new { result = false, message = "err！" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFromOther(string name)
        {
            var img= Request.Files[0];

            string path = Server.MapPath("~/")+"ImgFromOther/";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string imgName = path +Guid.NewGuid()+ ".jpg";
            img.SaveAs(imgName);

            if (SaveImg(name, imgName,true) == 1)
            {
                return Content("没有识别出来");
            }
            
            return Content("ok");
        }

        /// <summary>
        /// 保存样本图 从本地图片
        /// </summary>
        /// <param name="name"></param>
        private int SaveImg(string name, string fileName)
        {

            Com.KingFaceDetect detect = new Com.KingFaceDetect();

            string path = Server.MapPath("~/");
            Mat mat = new Mat(path+fileName.Replace("Y_","S_"), LoadImageType.Color);

         
            Com.KingFaceDetect.faceDetectedObj currentfdo = detect.GetFaceRectangle(mat);
         

            if (currentfdo.facesRectangle == null || currentfdo.facesRectangle.Count==0)
            {
                return 1;
            }

           

            Image<Gray, byte> result = currentfdo.originalImg.ToImage<Gray, byte>().Copy(currentfdo.facesRectangle[0]).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
            result._EqualizeHist();//灰度直方图均衡化

            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "\\trainedFaces\\" + name + "_" + System.Guid.NewGuid().ToString() + ".jpg";
            result.Bitmap.Save(filePath);
            return 2;
        }

        /// <summary>
        /// 保存样本图 从本地图片
        /// </summary>
        /// <param name="name"></param>
        private int SaveImg(string name, string fileName,bool fromOther)
        {

            Com.KingFaceDetect detect = new Com.KingFaceDetect();


            Mat mat = new Mat(fileName, LoadImageType.Color);


            Com.KingFaceDetect.faceDetectedObj currentfdo = detect.GetFaceRectangle(mat);


            if (currentfdo.facesRectangle == null || currentfdo.facesRectangle.Count == 0)
            {
                return 1;
            }



            Image<Gray, byte> result = currentfdo.originalImg.ToImage<Gray, byte>().Copy(currentfdo.facesRectangle[0]).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
            result._EqualizeHist();//灰度直方图均衡化

            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "\\trainedFaces\\" + name + "_" + System.Guid.NewGuid().ToString() + ".jpg";
            result.Bitmap.Save(filePath);
            return 2;
        }
	}
}