using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private readonly OpenFileDialog openImageDialog = new OpenFileDialog
        {
            Filter = "PNG image (*.png)|*.png|BMP image (*.bmp)|*.bmp|All files (*.*)|*.*"
            
        };

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (openImageDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openImageDialog.FileName));
                Imported_image.Source = bitmap;
                Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
            }
        }

        private bool mouseDown = false;
        private Point mouseDownPos;

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
                selectionBox.Width -= widthOverflow;
            }
            if (heightOverflow > 0)
            {
                selectionBox.Height -= heightOverflow;
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

        private List<SelectionCoordinates> list = new List<SelectionCoordinates>();
        private SelectionCoordinates? currentSelectionCoordinates;
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


                Canvas.SetTop(rec, Box.top - innerGrid.Margin.Top);
                Canvas.SetLeft(rec, Box.left - innerGrid.Margin.Left);
            }
        }
        private void Apply_User(object sender, RoutedEventArgs e)
        {

        }
    }
    
    struct SelectionCoordinates
    {
        public double left;
        public double right;
        public double bottom;
        public double top;

        public SelectionCoordinates(double left, double right, double top, double bottom)
        {
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
        }
       
        public bool Intersects(SelectionCoordinates other)
        {
            foreach (BoundingPoint point in other.GetBoundingPoints())
            {
                if(this.Contains(point))
                {
                    return true;
                }
            }
            foreach (BoundingPoint point in this.GetBoundingPoints())
            {
                if (other.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        private BoundingPoint[] GetBoundingPoints()
        {
            return new BoundingPoint[]{
                new BoundingPoint(this.left, this.top),
                new BoundingPoint(this.right, this.top),
                new BoundingPoint(this.right, this.bottom),
                new BoundingPoint(this.left, this.bottom)
            };
        }

        public bool Contains(BoundingPoint point)
        {
            return point.X >= this.left && point.X <= this.right && point.Y >= this.top && point.Y <= this.bottom;
        }
    }

    record BoundingPoint(double X, double Y);
}
