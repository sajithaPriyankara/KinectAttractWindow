namespace KinectAttractWindow.ViewModels
{
    using KinectAttractWindow.Navigation;
    using KinectAttractWindow.Models;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows;

    [ExportNavigable(NavigableContextName = DefaultNavigableContexts.HomeScreen)]
    public class HomeScreenViewModel : ViewModelBase
    {
        //public HomeModel RGBImage = new HomeModel();
        
        public const string RGBImagePropertyName = "RGBImage";
        private HomeModel _rgbImage = new HomeModel();
        public HomeModel RGBImage
        {
            get { return _rgbImage; }

            set
            {
                if (_rgbImage == value) { return; }

                var oldValue = _rgbImage;
                _rgbImage = value;
            }
        }
        
        
        WriteableBitmap wBitmap;
        private KinectSensor Sensor;

        public HomeScreenViewModel()
            : base()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.Sensor = KinectSensor.KinectSensors[0];

                if (this.Sensor.Status == KinectStatus.Connected)
                {
                    this.Sensor.ColorStream.Enable();
                    this.Sensor.DepthStream.Enable();
                    this.Sensor.SkeletonStream.Enable();
                    this.Sensor.AllFramesReady += Sensor_AllFramesReady;
                    this.Sensor.Start();
                }
            }
        }

        void Sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                wBitmap = new WriteableBitmap(colorFrame.Width,
                                                  colorFrame.Height,
                    // Standard DPI
                                                  96, 96,
                    // Current format for the ColorImageFormat
                                                  PixelFormats.Bgr32,
                    // BitmapPalette
                                                  null);

                wBitmap.WritePixels(
                    // Represents the size of our image
                new Int32Rect(0, 0, colorFrame.Width, colorFrame.Height),
                    // Our image data
                pixels,
                    // How much bytes are there in a single row?
                colorFrame.Width * colorFrame.BytesPerPixel,
                    // Offset for the buffer, where does he need to start
                0);


                this.RGBImage.DisplayImage.Source = wBitmap;
                //int stride = colorFrame.Width * 4;

                //this.RGBImage.DisplayImage.Source =
                //    BitmapSource.Create(colorFrame.Width,
                //    colorFrame.Height,
                //    96,
                //    96,
                //    PixelFormats.Bgr32,
                //    null,
                //    pixels,
                //    stride);
            }
        }
    }
}
