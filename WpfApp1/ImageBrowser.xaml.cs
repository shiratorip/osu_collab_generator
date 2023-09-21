using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OCG
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
        }

        private void PreviewImage(object sender, RoutedEventArgs e)
        {
            proceedButton.Visibility = Visibility.Hidden;
            previewImage.Source = null;
            try
            { 
            currentImage = new BitmapImage(new Uri(linkText.Text));
            {
                previewImage.Source = currentImage;

                //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
            }
            }catch(Exception exception)
            {
                return;
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += delegate
            {           
                proceedButton.Visibility = Visibility.Visible;

                timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            
            
        }

        private void Proceed(object sender, RoutedEventArgs e)
        {
            if(currentImage != null)
            {
                mainWindow.SetImage(currentImage);
                this.Close();
            }
        }
    }
}
