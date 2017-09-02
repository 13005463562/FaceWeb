using System;
using System.Web;
using System.IO;

namespace NewFace.Com
{
    public class FileUploadHelper
    {
        private string _filename = "";
        private string _filepath = "";
        private string _savepath = "";
        private decimal _size = 0m;//KB
        private string _extension = "";
        private string _error = "";
        public FileUploadHelper() { }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savepath"></param>
        /// <returns></returns>
        public bool UploadFile(HttpPostedFile file, string savepath)
        {

            if (file.ContentLength == 0)
            {
                this._error = "文件大小为0！";
                return false;
            }
            else
            {
                try
                {
                    this._size = Math.Round(Convert.ToDecimal(file.ContentLength / 1024), 2);
                    this._extension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1, (file.FileName.Length - file.FileName.LastIndexOf(".") - 1));
                    string osavePath = HttpContext.Current.Server.MapPath("~//" + savepath);
                    if (!Directory.Exists(osavePath))
                    {
                        Directory.CreateDirectory(osavePath);//在根目录下建立文件夹
                    }
                    this._filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "." + this._extension;
                    file.SaveAs(osavePath + "//" + this._filename);
                    this._filepath = savepath + "/" + this._filename;
                    return true;
                }
                catch (Exception e)
                {
                    this._error = e.Message;
                    return false;
                }
            }

        }


        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public string FilePath
        {
            get { return _filepath; }
            set { _filepath = value; }
        }
        public string SavePath
        {
            get { return _savepath; }
            set { _savepath = value; }
        }
        public decimal Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "YS"://不改变图片大小直接压缩
                    towidth = originalImage.Width;
                    toheight = originalImage.Height;
                    break;
                case "HW"://指定高宽缩放（能够变形） 
                    break;
                case "W"://指定宽，高按比例 
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽扩充（不变形） 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;

                case "C"://按原图比例缩放 
                    if (((double)originalImage.Width < (double)towidth) && ((double)originalImage.Height < (double)toheight))   //图片小于外框尺寸，不做处理
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Height;
                        x = (towidth - ow) / 2;
                        y = (toheight - oh) / 2;
                    }
                    else
                    {
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        else
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度出现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以通明背风光填充
            g.Clear(System.Drawing.Color.White);

            //在指定地位并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
            new System.Drawing.Rectangle(x, y, ow, oh),
            System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}
