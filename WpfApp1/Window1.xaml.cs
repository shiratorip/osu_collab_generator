﻿using System;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MainWindow mainWindow;
        BitmapImage? currentImage;

        public Window1(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void Preview_Image(object sender, RoutedEventArgs e)
        {
            currentImage = new BitmapImage(new Uri(linkText.Text));
            {
                previewImage.Source = currentImage;

                //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                proceedButton.Visibility = Visibility.Visible;
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
    }
}
