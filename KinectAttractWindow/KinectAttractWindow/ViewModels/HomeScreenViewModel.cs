namespace KinectAttractWindow.ViewModels
{
    using KinectAttractWindow.Navigation;
    using KinectAttractWindow.Models;
    using Microsoft.Kinect;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;

    [ExportNavigable(NavigableContextName = DefaultNavigableContexts.HomeScreen)]
    public class HomeScreenViewModel : ViewModelBase
    {
        public HomeModel RGBImage = new HomeModel();
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

                int stride = colorFrame.Width * 4;

                this.RGBImage.DisplayImage.Source =
                    BitmapSource.Create(colorFrame.Width,
                    colorFrame.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    pixels,
                    stride);
            }
        }
    }
}