using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NewFaceWeb.Com
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 图片转成字符串
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <returns>图片流数据 字符串格式</returns>
        public static string GetImage(string imgUrl)
        {
            if (!File.Exists(imgUrl))
            {
                return "";
            }

            var imgPath = imgUrl;
            //从图片中读取byte
            var imgByte = File.ReadAllBytes(imgPath);
            //从图片中读取流
            var imgStream = new MemoryStream(File.ReadAllBytes(imgPath));

            return ChangeImageToString(imgStream);
        }

        /// <summary>
        /// 文件流转成string
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string ChangeImageToString(MemoryStream ms)
        {
            try
            {

                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string pic = Convert.ToBase64String(arr);

                return pic;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 字符串转文件流
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
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