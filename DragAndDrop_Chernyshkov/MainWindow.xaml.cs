using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DragAndDrop_Chernyshkov
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool isDrag = false;
        private Point deltaPoint;
        private double startLeft = 300;
        private double startTop = 60;
        private double startWidth = 318;
        private double startHeight = 350;
        private double startFrameLeft = 323;
        private double startFrameTop = 80;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);

            ShowSizeText();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (!isDrag) return;

            Point position = Mouse.GetPosition(canvasArea);
            Canvas.SetLeft(cropFrame, position.X - deltaPoint.X);
            Canvas.SetTop(cropFrame, position.Y - deltaPoint.Y);
        }

        private void image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDrag = false;
            dispatcherTimer.Stop();
            if (sender is UIElement element)
            {
                element.ReleaseMouseCapture();
            }
            ShowSizeText();
        }

        private void image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDrag = true;

            Point mousePosition = e.GetPosition(canvasArea);
            deltaPoint.X = mousePosition.X - Canvas.GetLeft(cropFrame);
            deltaPoint.Y = mousePosition.Y - Canvas.GetTop(cropFrame);

            if (sender is UIElement element)
            {
                element.CaptureMouse();
            }
            dispatcherTimer.Start();
        }

        private void ShowSizeText()
        {
            BitmapSource? bitmapSource = image.Source as BitmapSource;
            if (bitmapSource != null)
            {
                tbRealSize.Text = bitmapSource.PixelWidth.ToString() + " x " + bitmapSource.PixelHeight.ToString();
            }
            else
            {
                tbRealSize.Text = ((int)startWidth).ToString() + " x " + ((int)startHeight).ToString();
            }

            tbScaleSize.Text = ((int)image.Width).ToString() + " x " + ((int)image.Height).ToString();
            tbCropSize.Text = ((int)cropFrame.Width).ToString() + " x " + ((int)cropFrame.Height).ToString();
        }

        private void ClearImage(object sender, RoutedEventArgs e)
        {
            image.Width = startWidth;
            image.Height = startHeight;
            Canvas.SetLeft(image, startLeft);
            Canvas.SetTop(image, startTop);
            Canvas.SetLeft(cropFrame, startFrameLeft);
            Canvas.SetTop(cropFrame, startFrameTop);
            ShowSizeText();
        }

        private void CropImage(object sender, RoutedEventArgs e)
        {
            BitmapSource? bitmapSource = image.Source as BitmapSource;
            if (bitmapSource == null)
            {
                MessageBox.Show("Изображение не найдено.");
                return;
            }

            double frameLeft = Canvas.GetLeft(cropFrame);
            double frameTop = Canvas.GetTop(cropFrame);
            double imageLeft = Canvas.GetLeft(image);
            double imageTop = Canvas.GetTop(image);

            double sourceX = (frameLeft - imageLeft) * (bitmapSource.PixelWidth / image.Width);
            double sourceY = (frameTop - imageTop) * (bitmapSource.PixelHeight / image.Height);
            double sourceW = cropFrame.Width * (bitmapSource.PixelWidth / image.Width);
            double sourceH = cropFrame.Height * (bitmapSource.PixelHeight / image.Height);

            int x = (int)sourceX;
            int y = (int)sourceY;
            int w = (int)sourceW;
            int h = (int)sourceH;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x + w > bitmapSource.PixelWidth) w = bitmapSource.PixelWidth - x;
            if (y + h > bitmapSource.PixelHeight) h = bitmapSource.PixelHeight - y;

            if (w <= 0 || h <= 0)
            {
                MessageBox.Show("Рамка вышла за пределы изображения.");
                return;
            }

            CroppedBitmap cropped = new CroppedBitmap(bitmapSource, new Int32Rect(x, y, w, h));
            image.Source = cropped;
            image.Width = cropFrame.Width;
            image.Height = cropFrame.Height;
            Canvas.SetLeft(image, frameLeft);
            Canvas.SetTop(image, frameTop);

            tbCropSize.Text = w.ToString() + " x " + h.ToString();
            ShowSizeText();
        }
    }
}
