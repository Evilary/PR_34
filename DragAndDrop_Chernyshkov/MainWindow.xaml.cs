using System.Windows;
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
            Canvas.SetLeft(image, position.X - deltaPoint.X);
            Canvas.SetTop(image, position.Y - deltaPoint.Y);
        }

        private void image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDrag = false;
            dispatcherTimer.Stop();
            image.ReleaseMouseCapture();
            ShowSizeText();
        }

        private void image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDrag = true;

            Point mousePosition = e.GetPosition(canvasArea);
            deltaPoint.X = mousePosition.X - Canvas.GetLeft(image);
            deltaPoint.Y = mousePosition.Y - Canvas.GetTop(image);

            image.CaptureMouse();
            dispatcherTimer.Start();
        }

        private void ShowSizeText()
        {
            tbRealSize.Text = ((int)startWidth).ToString() + " x " + ((int)startHeight).ToString();
            tbScaleSize.Text = ((int)image.Width).ToString() + " x " + ((int)image.Height).ToString();
            tbCropSize.Text = ((int)cropFrame.Width).ToString() + " x " + ((int)cropFrame.Height).ToString();
        }

        private void ClearImage(object sender, RoutedEventArgs e)
        {
            image.Width = startWidth;
            image.Height = startHeight;
            Canvas.SetLeft(image, startLeft);
            Canvas.SetTop(image, startTop);
            ShowSizeText();
        }

        private void CropImage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Следующий шаг: реализовать обрезку.");
        }
    }
}
