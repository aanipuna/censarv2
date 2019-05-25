//--------------------------------------------------------------------------------------
// Copyright 2016 Intel Corporation
// All Rights Reserved
//
// Permission is granted to use, copy, distribute and prepare derivative works of this
// software for any purpose and without fee, provided, that the above copyright notice
// and this statement appear in all copies.  Intel makes no representations about the
// suitability of this software for any purpose.  THIS SOFTWARE IS PROVIDED "AS IS."
// INTEL SPECIFICALLY DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, AND ALL LIABILITY,
// INCLUDING CONSEQUENTIAL AND OTHER INDIRECT DAMAGES, FOR THE USE OF THIS SOFTWARE,
// INCLUDING LIABILITY FOR INFRINGEMENT OF ANY PROPRIETARY RIGHTS, AND INCLUDING THE
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  Intel does not
// assume any responsibility for any errors which may appear in this software nor any
// responsibility to update it.
//--------------------------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using ConsoleApplication3;

namespace PersonTracking
{
    public partial class MainWindow : Window
    {
        private PXCMSenseManager sm;
        private PXCMFaceData faceData;
        private PXCMPersonTrackingData personData;
        private PXCMPersonTrackingModule personModule;
        private Thread update;
        private const int ImageWidth = 640;
        private const int ImageHeight = 480;
        private ushort cameraXPos = 0;
        private ushort cameraYPos = 0;
        static Behaviour my = new Behaviour();
       
        Thread child = new Thread(my.randomMotion);

        bool doit = true;
        int imgID = 0;
        Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            ConfigureRealSense();

            // Start the Update (data acquisition) thread
            update = new Thread(new ThreadStart(Update));
            update.Start();
        }

        private void ConfigureRealSense()
        {
            
            my.init();
            child.Start();
            try
            {
                // Create the SenseManager instance  
                sm = PXCMSenseManager.CreateInstance();

                // Enable the color stream
                sm.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, ImageWidth, ImageHeight, 30);

                // Enable person tracking
                sm.EnablePersonTracking();
                personModule = sm.QueryPersonTracking();
                PXCMPersonTrackingConfiguration personConfig = personModule.QueryConfiguration();
                personConfig.SetTrackedAngles(PXCMPersonTrackingConfiguration.TrackingAngles.TRACKING_ANGLES_ALL);

                // Enable the face module
                sm.EnableFace();
                PXCMFaceModule faceModule = sm.QueryFace();
                PXCMFaceConfiguration faceConfig = faceModule.CreateActiveConfiguration();
                faceConfig.SetTrackingMode(PXCMFaceConfiguration.TrackingModeType.FACE_MODE_COLOR_PLUS_DEPTH);
                faceConfig.strategy = PXCMFaceConfiguration.TrackingStrategyType.STRATEGY_APPEARANCE_TIME;
                faceConfig.detection.maxTrackedFaces = 1;

                // Apply changes and initialize the SenseManager
                faceConfig.ApplyChanges();
                sm.Init();
                faceData = faceModule.CreateOutput();
                
                // Mirror the image
                sm.QueryCaptureManager().QueryDevice().SetMirrorMode(PXCMCapture.Device.MirrorMode.MIRROR_MODE_HORIZONTAL);

                // Release resources
                personConfig.Dispose();
                faceConfig.Dispose();
                faceModule.Dispose();
            }
            catch (Exception)
            {
                // For the sake of brevity we're not doing extensive exception handling in this code sample,
                // simply give a hint that the camera is not connected, and then throw the exception
                MessageBox.Show("Unable to configure the RealSense camera. Please make sure a R200 camera is connected.", "System Error");
                throw;
            }
        }

        private void Update()
        {
            // Start AcquireFrame-ReleaseFrame loop
            while (sm.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                // Acquire color image data
                PXCMCapture.Sample sample = sm.QuerySample();
                Bitmap colorBitmap;
                PXCMImage.ImageData colorData;
                sample.color.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out colorData);
                colorBitmap = colorData.ToBitmap(0, sample.color.info.width, sample.color.info.height);

                // Create an instance of MyTrackedPerson
                MyTrackedPerson myTrackedPerson = new MyTrackedPerson();

                // Acquire person tracking data
                personData = personModule.QueryOutput();
                myTrackedPerson.PersonsDetected = personData.QueryNumberOfPeople();

                if (myTrackedPerson.PersonsDetected == 1)
                {
                    // person track data
                    PXCMPersonTrackingData.Person trackedPerson = personData.QueryPersonData(PXCMPersonTrackingData.AccessOrderType.ACCESS_ORDER_BY_ID, 0);
                    PXCMPersonTrackingData.PersonTracking trackedPersonData = trackedPerson.QueryTracking();
                    PXCMPersonTrackingData.BoundingBox2D personBox = trackedPersonData.Query2DBoundingBox();
                    myTrackedPerson.X = personBox.rect.x;
                    myTrackedPerson.Y = personBox.rect.y;
                    myTrackedPerson.H = personBox.rect.h;
                    myTrackedPerson.W = personBox.rect.w;

                    // Acquire face tracking data
                    faceData.Update();
                    myTrackedPerson.FacesDetected = faceData.QueryNumberOfDetectedFaces();

                    if (myTrackedPerson.FacesDetected == 1)
                    {
                        PXCMFaceData.Face face = faceData.QueryFaceByIndex(0);
                        PXCMFaceData.DetectionData faceDetectionData = face.QueryDetection();
                        PXCMRectI32 faceRectangle;
                        faceDetectionData.QueryBoundingRect(out faceRectangle);
                        myTrackedPerson.FaceH = faceRectangle.h;
                        myTrackedPerson.FaceW = faceRectangle.w;
                        myTrackedPerson.FaceX = faceRectangle.x;
                        myTrackedPerson.FaceY = faceRectangle.y;
                        float faceDepth;
                        faceDetectionData.QueryFaceAverageDepth(out faceDepth);
                        myTrackedPerson.FaceDepth = faceDepth;

                        //save image
                        if (doit == true)
                        {
                            colorBitmap.Save("myBitmap" + imgID + ".bmp");
                            doit = false;
                            stopwatch.Start();
                        }
                        else if (stopwatch.Elapsed.Seconds > 10)
                        {
                            imgID++;
                            doit = true;
                            stopwatch.Reset();
                        }
                    }

                    //IRobotCreate.SetDrive(40, 40);
                    //my.servoNeck.setTargetPosition(1500);
                    my.servos.move(Behaviour.neck, 100);//change because new servo class

                    if ((0 < (myTrackedPerson.X)) && ((myTrackedPerson.X) <= 120)) {
                        //my.servoShoulder.setTargetPosition(1300);
                        my.servos.move(Behaviour.neck, 90);
                        IRobotCreate.SetDrive(20,-20);
                    }
                    else if ((120 < (myTrackedPerson.X)) && ((myTrackedPerson.X) < 310)) {
                        // follow up based on face depth
                        //my.servoShoulder.setTargetPosition(1500);
                        my.servos.move(Behaviour.head, 100);//change because new servo class
                        float depth = myTrackedPerson.FaceDepth - (int)myTrackedPerson.FaceDepth % 100;
                        if (myTrackedPerson.FacesDetected == 1 && depth < 1750 && depth > 1400) {
                            IRobotCreate.SetDrive(0, 0);
                        }
                        else if (myTrackedPerson.FacesDetected == 1 && depth < 1400) {
                            IRobotCreate.SetDrive(-100, -100);
                        }
                        else if (myTrackedPerson.FacesDetected == 1 && depth > 1750) {
                            IRobotCreate.SetDrive(100, 100);
                        }
                        
                    }
                    else if (310 <= (myTrackedPerson.X)) {
                        //my.servoShoulder.setTargetPosition(1700);
                        my.servos.move(Behaviour.head, 120);//change because new servo class
                        IRobotCreate.SetDrive(-20, 20);
                    }
                }
                //my.servoNeck.SetSpeed(40);
                // my.servoShoulder.SetSpeed(40);
                my.servos.setSpeed(Behaviour.neck, 100);//change because new servo class
                my.servos.setSpeed(Behaviour.head, 100);//change because new servo class

                // Update UI
                Render(colorBitmap, myTrackedPerson);

                // Release resources
                colorBitmap.Dispose();
                sample.color.ReleaseAccess(colorData);
                sm.ReleaseFrame();
            }
        }

        private void Render(Bitmap bitmap, MyTrackedPerson myTrackedPerson)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                // Update the bitmap image
                BitmapImage bitmapImage = ConvertBitmap(bitmap);

                if (bitmapImage != null)
                {
                    imgStream.Source = bitmapImage;
                }

                // Update the data labels
                lblFacesDetected.Content = string.Format("Faces Detected: {0}", myTrackedPerson.FacesDetected);
                lblFaceH.Content = string.Format("Face Rect H: {0}", myTrackedPerson.FaceH);
                lblFaceW.Content = string.Format("Face Rect W: {0}", myTrackedPerson.FaceW);
                lblFaceX.Content = string.Format("Face Coord X: {0}", myTrackedPerson.FaceX);
                lblFaceY.Content = string.Format("Face Coord Y: {0}", myTrackedPerson.FaceY);
                lblFaceDepth.Content = string.Format("Face Depth: {0}", myTrackedPerson.FaceDepth);
                lblNumberPersons.Content = string.Format("Persons Detected: {0}", myTrackedPerson.PersonsDetected);
                lblPersonH.Content = string.Format("Person Rect H: {0}", myTrackedPerson.H);
                lblPersonW.Content = string.Format("Person Rect W: {0}", myTrackedPerson.W);
                lblPersonX.Content = string.Format("Person Coord X: {0}", myTrackedPerson.X);
                lblPersonY.Content = string.Format("Person Coord Y: {0}", myTrackedPerson.Y);

                // Show or hide the markers
                if (chkShowMarkers.IsChecked == true)
                {
                    if (myTrackedPerson.FacesDetected == 1)
                    {
                        rectFaceMarker.Height = myTrackedPerson.FaceH;
                        rectFaceMarker.Width = myTrackedPerson.FaceW;
                        Canvas.SetLeft(rectFaceMarker, myTrackedPerson.FaceX);
                        Canvas.SetTop(rectFaceMarker, myTrackedPerson.FaceY);
                        rectFaceMarker.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rectFaceMarker.Visibility = Visibility.Hidden;
                    }

                    if (myTrackedPerson.PersonsDetected == 1)
                    {
                        rectPersonMarker.Height = myTrackedPerson.H;
                        rectPersonMarker.Width = myTrackedPerson.W;
                        Canvas.SetLeft(rectPersonMarker, myTrackedPerson.X);
                        Canvas.SetTop(rectPersonMarker, myTrackedPerson.Y);
                        rectPersonMarker.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rectPersonMarker.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    rectFaceMarker.Visibility = Visibility.Hidden;
                    rectPersonMarker.Visibility = Visibility.Hidden;
                }
            }));
        }

        private BitmapImage ConvertBitmap(Bitmap bitmap)
            
        {
            if (bitmap != null) {
                Console.WriteLine("Getting the image");
            }
            bitmap.Save("myBitmap.bmp");
            BitmapImage bitmapImage = null;

            if (bitmap != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Behaviour.run = false;
            update.Abort();
            personData.Dispose();
            personModule.Dispose();
            faceData.Dispose();
            sm.Dispose();

            IRobotCreate.SetDrive(0, 0); //To Stop iRobot

        }
    }
}
