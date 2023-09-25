using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace osuCollabGenerator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ImageBrowser : Window
    {
        MainWindow mainWindow;
        BitmapImage? currentImage;

        public ImageBrowser(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent, new RoutedEventHandler(OnMouseDoubleClick));
        }

        private readonly Mutex imageDownloadingMutex = new Mutex();
        private void PreviewImage(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri uri = new Uri(linkText.Text); //this can fail quickly so we put it first before mutex
                imageDownloadingMutex.WaitOne();
                proceedButton.Visibility = Visibility.Hidden;
                previewImage.Source = null;
                currentImage = new BitmapImage();
                currentImage.BeginInit();
                currentImage.DownloadCompleted += delegate
                {
                    proceedButton.Visibility = Visibility.Visible;
                    imageDownloadingMutex.ReleaseMutex();
                };
                currentImage.DownloadFailed += delegate
                {
                    imageDownloadingMutex.ReleaseMutex();
                };
                currentImage.UriSource = uri;
                currentImage.EndInit();
                previewImage.Source = currentImage;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                return;
            }
        }

        private void Proceed(object sender, RoutedEventArgs e)
        {
            if(currentImage != null)
            {
                mainWindow.SetImage(currentImage);
                this.Close();
            }
        }


        private void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
