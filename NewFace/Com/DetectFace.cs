//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Text;
#if !(__IOS__ || NETFX_CORE)
using Emgu.CV.Cuda;
#endif

namespace NewFace.Com
{
   public static class DetectFace
   {
      public static void Detect(
        Mat image, String faceFileName, String eyeFileName, 
        List<Rectangle> faces, List<Rectangle> eyes, 
        bool tryUseCuda,
        out long detectionTime)
      {
          
         Stopwatch watch;
 
            //Read the HaarCascade objects
            using (CascadeClassifier face = new CascadeClassifier(faceFileName))
            using (CascadeClassifier eye = new CascadeClassifier(eyeFileName))
            {
                   
               watch = Stopwatch.StartNew();
               using (UMat ugray = new UMat())
               {
                     
                        CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                       
                        //normalizes brightness and increases contrast of the image
                        CvInvoke.EqualizeHist(ugray, ugray);

                  //Detect the faces  from the gray scale image and store the locations as rectangle
                  //The first dimensional is the channel
                  //The second dimension is the index of the rectangle in the specific channel
                  Rectangle[] facesDetected = face.DetectMultiScale(
                     ugray,
                     1.1,
                     10,
                     new Size(20, 20));
                     
                  faces.AddRange(facesDetected);

                  foreach (Rectangle f in facesDetected)
                  {
                     //Get the region of interest on the faces
                     using (UMat faceRegion = new UMat(ugray, f))
                     {
                        Rectangle[] eyesDetected = eye.DetectMultiScale(
                           faceRegion,
                           1.1,
                           10,
                           new Size(20, 20));
                        
                        foreach (Rectangle e in eyesDetected)
                        {
                           Rectangle eyeRect = e;
                           eyeRect.Offset(f.X, f.Y);
                           eyes.Add(eyeRect);
                        }
                     }
                  }
               }
               watch.Stop();
            }
         
         detectionTime = watch.ElapsedMilliseconds;
      }

        public static void Detect(
       IInputArray image, String faceFileName, String eyeFileName,
       List<Rectangle> faces, List<Rectangle> eyes,
       bool tryUseCuda,
       out long detectionTime)
        {
            AddLog("detec");
            Stopwatch watch;


                //Read the HaarCascade objects
                using (CascadeClassifier face = new CascadeClassifier(faceFileName))
                using (CascadeClassifier eye = new CascadeClassifier(eyeFileName))
                {
                AddLog(faceFileName);

                watch = Stopwatch.StartNew();
                    using (UMat ugray = new UMat())
                    {
                        CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                    AddLog("1");
                        //normalizes brightness and increases contrast of the image
                        CvInvoke.EqualizeHist(ugray, ugray);
                    AddLog("2");
                    //Detect the faces  from the gray scale image and store the locations as rectangle
                    //The first dimensional is the channel
                    //The second dimension is the index of the rectangle in the specific channel
                    Rectangle[] facesDetected = face.DetectMultiScale(
                           ugray,
                           1.1,
                           10,
                           new Size(20, 20));

                        faces.AddRange(facesDetected);

                        foreach (Rectangle f in facesDetected)
                        {
                            //Get the region of interest on the faces
                            using (UMat faceRegion = new UMat(ugray, f))
                            {
                                Rectangle[] eyesDetected = eye.DetectMultiScale(
                                   faceRegion,
                                   1.1,
                                   10,
                                   new Size(20, 20));

                                foreach (Rectangle e in eyesDetected)
                                {
                                    Rectangle eyeRect = e;
                                    eyeRect.Offset(f.X, f.Y);
                                    eyes.Add(eyeRect);
                                }
                            }
                        }
                    }
                    watch.Stop();
                }
            
            detectionTime = watch.ElapsedMilliseconds;
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
    }
}
