using System;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace WebcamSample
{
    public partial class MainWindow : Window
    {
        private CascadeClassifier eyeCascade;
        private VideoCapture capture;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartWebcam_Click(object sender, RoutedEventArgs e)
        {
            eyeCascade = new CascadeClassifier("C:\\Users\\rsk_a\\Downloads\\haarcascade_eye.xml");
            capture = new VideoCapture(0); 

            if (capture != null)
            {
                
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(33);
                timer.Tick += (s, t) =>
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    if (!frame.IsEmpty)
                    {
                        Rectangle[] eyes = eyeCascade.DetectMultiScale(frame, 1.3, 5);

                        
                        foreach (Rectangle eye in eyes)
                        {
                            CvInvoke.Rectangle(frame, eye, new Bgr(0, 255, 0).MCvScalar, 2);
                        }
                        webcamImage.Source = MatToImageSource(frame);
                    }
                };
                timer.Start();
            }
        }

        private ImageSource MatToImageSource(Mat image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.ToImage<Bgr, byte>().ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (capture != null)
            {
                capture.Dispose();
            }
            Close();
        }
    }
}
