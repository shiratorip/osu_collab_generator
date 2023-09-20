using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary
    /// collab maker project
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool mouseDown = false;
        private Point mouseDownPos;
        private List<SelectionCoordinates> list = new List<SelectionCoordinates>();
        private SelectionCoordinates? currentSelectionCoordinates;

        private Uri imageUrl = new Uri("https://a.ppy.sh/8934294?1671397512.png");

        private readonly OpenFileDialog openImageDialog = new OpenFileDialog
        {
            Filter = "PNG image (*.png)|*.png|BMP image (*.bmp)|*.bmp|All files (*.*)|*.*"
            
        };

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (openImageDialog.ShowDialog() == true)
            {
                mouseDown = false;
                list.Clear();
                currentSelectionCoordinates = null;
                StorageCanvas.Children.Clear();
                selectionBox.Visibility = Visibility.Collapsed;
                BitmapImage bitmap = new BitmapImage(new Uri(openImageDialog.FileName));
                Imported_image.Source = bitmap;

                //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                innerGrid.Width = bitmap.Width;
                innerGrid.Height = bitmap.Height;
                bitmap.DownloadCompleted += delegate
                {
                    Imported_image.Source = bitmap;

                    //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                    innerGrid.Width = bitmap.Width;
                    innerGrid.Height = bitmap.Height;
                };
            }
            
        }

        

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            mouseDown = true;
            mouseDownPos = e.GetPosition(theGrid);
            theGrid.CaptureMouse();

            Canvas.SetLeft(selectionBox, mouseDownPos.X);
            Canvas.SetTop(selectionBox, mouseDownPos.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;

            selectionBox.Visibility = Visibility.Visible;
        }
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
            mouseDown = false;
            theGrid.ReleaseMouseCapture();

            double selectionBoxLeft = Canvas.GetLeft(selectionBox) - innerGrid.Margin.Left;
            double selectionBoxTop = Canvas.GetTop(selectionBox) - innerGrid.Margin.Top;
            
            if (selectionBoxLeft < 0)
            {
                selectionBox.Width += selectionBoxLeft;
                Canvas.SetLeft(selectionBox, innerGrid.Margin.Left);
            }
            else
            {
                Canvas.SetLeft(selectionBox, selectionBoxLeft + innerGrid.Margin.Left);
            }

            if (selectionBoxTop < 0)
            {
                selectionBox.Height += selectionBoxTop;
                Canvas.SetTop(selectionBox, innerGrid.Margin.Top);
            }
            else
            {
                Canvas.SetTop(selectionBox, selectionBoxTop + innerGrid.Margin.Top);
            }

            double widthOverflow = (Canvas.GetLeft(selectionBox) - innerGrid.Margin.Left + selectionBox.Width) - innerGrid.ActualWidth;
            double heightOverflow = (Canvas.GetTop(selectionBox) - innerGrid.Margin.Top + selectionBox.Height) - innerGrid.ActualHeight;
            if (widthOverflow > 0)
            {
                selectionBox.Width -= Math.Min(widthOverflow, selectionBox.Width);
            }
            if (heightOverflow > 0)
            {
                selectionBox.Height -= Math.Min(heightOverflow, selectionBox.Height);
            }

            double left = Canvas.GetLeft(selectionBox);
            double right = left + selectionBox.Width;
            double top = Canvas.GetTop(selectionBox);
            double bottom = top + selectionBox.Height;
            SelectionCoordinates box = new SelectionCoordinates(left, right, top, bottom);
            foreach (var rect in list)
            {
                if (rect.Intersects(box))
                {
                    selectionBox.Visibility = Visibility.Collapsed;
                    currentSelectionCoordinates = null;
                    return;
                }
            }
            currentSelectionCoordinates = box;
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e) {
            if(mouseDown)
            {
                Point mousePos = e.GetPosition(theGrid);

                if (mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionBox, mouseDownPos.X);
                    selectionBox.Width = mousePos.X - mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePos.X);
                    selectionBox.Width = mouseDownPos.X - mousePos.X;
                }

                if (mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionBox, mouseDownPos.Y);
                    selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePos.Y);
                    selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                }
            }
        }

        private int columnCounter = 0;
        private int rowCounter = 0;
        private Rectangle selectedRect;
        private void Button_Click(object sender, RoutedEventArgs e)
        {   
            
            if(currentSelectionCoordinates.HasValue)
            {
                SelectionCoordinates Box = currentSelectionCoordinates.Value;
                list.Add(Box);
                Rectangle rec = new Rectangle()
                {
                    Width = selectionBox.Width,
                    Height = selectionBox.Height,

                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                };
                

                StorageCanvas.Children.Add(rec);

                Button but = new Button();
                but.Height = 20;
                but.Width = 20;
                
                Grid.SetColumn(but, columnCounter);
                Grid.SetRow(but, rowCounter);
                columnCounter++;
                if(columnCounter == 4)
                {
                    columnCounter = 0;
                    rowCounter++;
                }
                but.Click += delegate
                {
                    foreach (Rectangle item in StorageCanvas.Children)
                    {
                        item.Stroke = Brushes.Blue;
                    }
                    rec.Stroke = Brushes.Red;
                    
                };
                
                ButtonsGrid.Children.Add(but);

                Canvas.SetTop(rec, Box.top - innerGrid.Margin.Top);
                Canvas.SetLeft(rec, Box.left - innerGrid.Margin.Left);
            }
        }

       

        private void Apply_User(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("Test");

        }

        private void Export_Collab(object sender, RoutedEventArgs e)
        {
            string exportString = "[imagemap]\n";
            exportString += $"{imageUrl}\n";
            foreach (SelectionCoordinates coords in list)
            {
                double left = (coords.left - innerGrid.Margin.Left) / innerGrid.Width * 100;
                double top = (coords.top - innerGrid.Margin.Top) / innerGrid.Height * 100;
                double width = (coords.right - coords.left) / innerGrid.Width * 100;
                double height = (coords.bottom - coords.top) / innerGrid.Height * 100;
                exportString += $"{left} {top} {width} {height} https://osu.ppy.sh/users/8934294 FoLZer\n";
            }
            exportString += "[/imagemap]";
            Clipboard.SetText(exportString);
            ExportButton.Content = "Collab Exported!";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += delegate
            {
                ExportButton.Content = "Export";
                timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Start();

        }
    }
}
