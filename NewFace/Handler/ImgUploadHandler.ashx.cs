using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NewFace.Handler
{
    /// <summary>
    /// ImgUploadHandler 的摘要说明
    /// </summary>
    public class ImgUploadHandler : IHttpHandler
    {
        HttpContext context;
        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            context.Response.ContentType = "text/plain";
            string result = "[]";
            string method = context.Request["method"];
            switch (method)
            {
                //上传图片
                case "FilesSend":
                    result = FilesSend();
                    break;
                //删除图片
                case "FilesDelete":
                    result = FilesDelete();
                    break;
            }
            context.Response.Write(result);
            context.Response.End();
        }
        string FilesSend()
        {
            string result = "{\"Result\":\"error\",\"Message\":\"图片上传失败！\"}";
            string DirPath = "images";
            if (!string.IsNullOrEmpty(context.Request["DirPath"]))
            {
                DirPath = context.Request["DirPath"];
            }
            HttpFileCollection files = context.Request.Files;
            if (files.Count > 0)
            {
                ///检查文件扩展名字   
                HttpPostedFile postedFile = files[0];
                string fileName, fileExtension;
                fileName = System.IO.Path.GetFileName(postedFile.FileName);
                if (fileName != "")
                {
                    fileExtension = System.IO.Path.GetExtension(fileName);
                    if (fileExtension.ToLower() != ".jpg" && fileExtension.ToLower() != ".jpeg" && fileExtension.ToLower() != ".gif" && fileExtension.ToLower() != ".png")
                    {
                        //this.AlbumFileLabel.Text = fileName + "图片格式不被支持！";
                        result = "{\"Result\":\"error\",\"Message\":\"" + fileExtension.ToLower() + "格式不被支持！\"}";
                    }
                    else
                    {
                        string filePath = "/UploadFile/" + DirPath + "/" + DateTime.Now.ToString("yyyyMMdd");
                        string filname = Com.UtilsHelper.DateTimeChuo(fileName);
                        string path = HttpContext.Current.Server.MapPath("~" + filePath + "/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string saveFilePath = path + filname;
                        postedFile.SaveAs(saveFilePath);
                        ////生成小图
                        string SaveFilePath_S = path + "S_" + filname;
                        Com.FileUploadHelper.MakeThumbnail(saveFilePath, SaveFilePath_S, 320, 320, "C");                        
                        //生成大图
                        string SaveFilePath_L = path + "L_" + filname;
                       // Com.FileUploadHelper.MakeThumbnail(saveFilePath, SaveFilePath_L, 510, 510, "C");

                        //生成中图
                        string SaveFilePath_Y = path + "Y_" + filname;
                        Com.FileUploadHelper.MakeThumbnail(saveFilePath, SaveFilePath_Y, 0, 0, "YS");

                        result = "{\"Result\":\"success\",\"Message\":\"" + filePath + "/" + "Y_" + filname + "\"}";
                    }
                }
                else
                {
                    result = "{\"Result\":\"error\",\"Message\":\"请选择图片！\"}";
                }
            }
            return result;
        }
        string FilesDelete()
        {
            string result = "{\"Result\":\"success\",\"Message\":\"图片删除成功！\"}";
            string photoPath = context.Request["imgpath"];

            if (!string.IsNullOrEmpty(photoPath))
            {
                DealWithFiles dwf = new DealWithFiles();
                string img = photoPath;
                int pos = img.LastIndexOf("/");
                //string s_img = img.Insert(pos + 1, "S_");
                //string m_img = img.Insert(pos + 1, "M_");
                //string l_img = img.Insert(pos + 1, "L_");
                //string y_img = img.Insert(pos + 1, "Y_");

                string l_img = img.Replace("Y_", "L_");
                string y_img = img;
                img = img.Replace("Y_", "");

                dwf.DeleteFile(HttpContext.Current.Server.MapPath("~" + img));
                //dwf.DeleteFile(HttpContext.Current.Server.MapPath("~" + s_img));
                //dwf.DeleteFile(HttpContext.Current.Server.MapPath("~" + m_img));
                dwf.DeleteFile(HttpContext.Current.Server.MapPath("~" + l_img));
                dwf.DeleteFile(HttpContext.Current.Server.MapPath("~" + y_img));
            }
            return result;
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