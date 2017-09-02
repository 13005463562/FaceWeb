using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;


using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Web.WebSockets;

using System.IO;

using System.Drawing.Imaging;

namespace NewFace.Handler
{
    /// <summary>
    /// AddFaceInfor 的摘要说明
    /// </summary>
    public class AddFaceInfor : IHttpHandler
    {

        private static string phy;
        private int _maxBufferSize = 256 * 1024;

        public void ProcessRequest(HttpContext context)
        {

            if (context.IsWebSocketRequest)
            {
                phy = context.Request.PhysicalApplicationPath;

                context.AcceptWebSocketRequest(ProcessWSChat);
            }
        }

        private async Task ProcessWSChat(AspNetWebSocketContext context)
        {
            try
            {
                WebSocket socket = context.WebSocket;


                byte[] receiveBuffer = new byte[_maxBufferSize];
                ArraySegment<byte> buffer = new ArraySegment<byte>(receiveBuffer);

                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(
                            result.CloseStatus.GetValueOrDefault(),
                            result.CloseStatusDescription,
                            CancellationToken.None);
                        break;
                    }

                    int offset = result.Count;

                    while (result.EndOfMessage == false)
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, offset, _maxBufferSize - offset), CancellationToken.None);
                        offset += result.Count;
                    }

                    if (result.MessageType == WebSocketMessageType.Binary && offset != 0)
                    {

                        ArraySegment<byte> newbuff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(FaceDetection(receiveBuffer, offset)));
                        await socket.SendAsync(newbuff, WebSocketMessageType.Text, true, CancellationToken.None);

                    }
                }
            }
            catch (Exception e)
            {
                var err = e.Message;
                Com.Other.AddLog(err);
            }
        }

        private static string FaceDetection(byte[] data, int plength)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("[");



            Image img = Com.Other.GetImageByBytes(data);// Properties.Resources.Form3_PIC_00;  //只能是system.drawing.image能读入，Mat和emgu的image读不了
            Bitmap bmpImage = new Bitmap(img); //这是关键，国外网站看到的
            Emgu.CV.Image<Bgr, Byte> currentFrame = new Emgu.CV.Image<Bgr, Byte>(bmpImage);  //只能这么转

            Mat invert = new Mat();
            CvInvoke.BitwiseAnd(currentFrame, currentFrame, invert);  //这是官网上的方法，变通用。没看到提供其它方法直接转换的。



            if (invert != null)
            {
                List<Rectangle> faces = Run(invert);
                foreach (var face in faces)
                {
                    sb.AppendFormat("{{\"X\":{0},\"Y\":{1},\"W\":{2},\"H\":{3}}},", face.X, face.Y, face.Width, face.Height);

                }

                if (sb[sb.Length - 1] == ',')
                {
                    sb.Remove(sb.Length - 1, 1);
                }

                //File.Delete(b);


            }

            sb.Append("]");

            //AddLog((System.Environment.TickCount - aa).ToString()); //单位毫秒 
            return sb.ToString();
        }

        private static string FaceDetectionDetail(byte[] data, int plength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");


            Image img = Com.Other.GetImageByBytes(data); //只能是system.drawing.image能读入，Mat和emgu的image读不了
            Bitmap bmpImage = new Bitmap(img); //这是关键，国外网站看到的
            Emgu.CV.Image<Bgr, Byte> currentFrame = new Emgu.CV.Image<Bgr, Byte>(bmpImage);  //只能这么转

            Mat invert = new Mat();
            CvInvoke.BitwiseAnd(currentFrame, currentFrame, invert);  //这是官网上的方法，变通用。没看到提供其它方法直接转换的。

            if (invert != null)
            {
                Com.KingFaceDetect.faceDetectedObj faces = Run1(invert);
                for (int i = 0; i < faces.facesRectangle.Count; i++)
                {
                    sb.AppendFormat("{{\"X\":{0},\"Y\":{1},\"W\":{2},\"H\":{3},\"N\":\"{4}\"}},", faces.facesRectangle[i].X, faces.facesRectangle[i].Y, faces.facesRectangle[i].Width, faces.facesRectangle[i].Height, faces.names[i]);
                }

                if (sb[sb.Length - 1] == ',')
                {
                    sb.Remove(sb.Length - 1, 1);
                }

            }

            sb.Append("]");

            GC.Collect();
            //AddLog((System.Environment.TickCount - aa).ToString()); //单位毫秒 
            return sb.ToString();
        }


        /// <summary>
        /// 只识别脸 不查找姓名
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        static List<Rectangle> Run(Mat image)
        {
            long detectionTime;
            List<Rectangle> faces = new List<Rectangle>();
            List<Rectangle> eyes = new List<Rectangle>();

            bool tryUseCuda = false;


            Com.DetectFace.Detect(
        image, phy + "haarcascade_frontalface_default.xml", phy + "haarcascade_eye.xml",
        faces, eyes,
        tryUseCuda,
        out detectionTime);

            // image.Save(@"E:\Work\Publish\face\bin\2.jpg");

            return faces;
        }

        /// <summary>
        /// 脸和姓名都查找
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        static Com.KingFaceDetect.faceDetectedObj Run1(Mat image)
        {
            List<Rectangle> faces = new List<Rectangle>();
            List<Rectangle> eyes = new List<Rectangle>();

            if (HttpContext.Current.Application["detect"] == null)
            {
                HttpContext.Current.Application["detect"] = new Com.KingFaceDetect();  //存入全局 否则好像会报内存错误
            }
            Com.KingFaceDetect detect = (Com.KingFaceDetect)HttpContext.Current.Application["detect"];
            Com.KingFaceDetect.faceDetectedObj resut = detect.faceRecognize(image);

            return resut;
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