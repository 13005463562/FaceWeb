using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace NewFace.Com
{
    public class Other
    {
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(System.DateTime time)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }


        /// <summary>
        /// 记录日志文本Log文件夹
        /// </summary>
        /// <param name="msg"></param>
        public static void AddLog(string msg)
        {
            string LogFile = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Log");
            string DayFile = Path.Combine(LogFile, DateTime.Now.ToString("yyyy-MM-dd"));
            string ErrorHourPath = Path.Combine(DayFile, DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".txt");
            if (!Directory.Exists(LogFile))
                Directory.CreateDirectory(LogFile);
            if (!Directory.Exists(DayFile))
                Directory.CreateDirectory(DayFile);
            using (FileStream fs = File.Open(ErrorHourPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    fs.Seek(0, SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + msg);
                    //sw.WriteLine(msg);
                    sw.Write(Environment.NewLine);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }


        private static byte[] BitmapToByte(Bitmap b)
        {
            MemoryStream ms = new MemoryStream();
            //b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }
        private static Bitmap ByteToBitmap(byte[] datas, int pLength)
        {
            MemoryStream ms1 = new MemoryStream(datas, 0, pLength);
            Bitmap bm = (Bitmap)Bitmap.FromStream(ms1);

            ms1.Close();
            return bm;
        }
        /// <summary>
        /// 读取byte[]并转化为图片
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Image</returns>
        public static Image GetImageByBytes(byte[] bytes)
        {
            Image photo = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Write(bytes, 0, bytes.Length);
                photo = Image.FromStream(ms, true);
            }

            return photo;
        }
    }
}