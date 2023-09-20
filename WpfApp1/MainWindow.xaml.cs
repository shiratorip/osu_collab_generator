using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
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
            double selectionBoxWidth = selectionBoxLeft + selectionBox.Width + innerGrid.Margin.Right;
            double selectionBoxTop = Canvas.GetTop(selectionBox) - innerGrid.Margin.Top;
            double selectionBoxHeight = selectionBoxTop + selectionBox.Height + innerGrid.Margin.Bottom;

            double minusHeight = 0;
            double minusWidth = 0;
            
            if (selectionBoxLeft < 0)
            {
                minusWidth -= selectionBoxLeft;
                selectionBoxLeft = 0;
            }
            if (selectionBoxWidth > innerGrid.Width) {
                minusWidth += selectionBoxWidth - innerGrid.Width;
                selectionBoxWidth = innerGrid.Width;
            }
            if (selectionBoxTop < 0)
            {
                minusHeight -= selectionBoxTop;
                selectionBoxTop = 0;
            }
            if (selectionBoxHeight > innerGrid.Height)
            {
                minusHeight += selectionBoxHeight - innerGrid.Height;
                selectionBoxHeight = innerGrid.Height;
            }

            Canvas.SetLeft(selectionBox, selectionBoxLeft + innerGrid.Margin.Left);
            //Canvas.SetRight(selectionBox, selectionBoxRight - innerGrid.Margin.Right);
            selectionBox.Width = selectionBoxWidth - minusWidth;
            selectionBox.Height = selectionBoxHeight - minusHeight;
            Canvas.SetTop(selectionBox, selectionBoxTop + innerGrid.Margin.Top);
            //Canvas.SetBottom(selectionBox, selectionBoxBottom - innerGrid.Margin.Bottom);
            //selectionBox.Width -= minusWidth;
            //selectionBox.Height -= minusHeight;
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
        private double RawBottom;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(selectionBox.Visibility == Visibility.Visible)
            {
                double left = Canvas.GetLeft(selectionBox);
                double right = Canvas.GetRight(selectionBox);
                double bottom = Canvas.GetBottom(selectionBox);
                double top = Canvas.GetTop(selectionBox);
                
                

                list.Add(new SelectionCoordinates(left, right, bottom, top));
                Rectangle rec = new Rectangle()
                {
                    Width = selectionBox.Width,
                    Height = selectionBox.Height,
                    
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                };
                
                StorageCanvas.Children.Add(rec);
     

                Canvas.SetTop(rec, top);
                Canvas.SetLeft(rec, left);

            }
        }
        private bool CheckOverlap(double[] TestedSquare)
        {
            foreach (var rect in list)
            {
                TestedSquare[0] = (TestedSquare[0] < rect.right) ?rect.right : TestedSquare[0];

                TestedSquare[1] = (TestedSquare[1] > rect.left) ?rect.left : TestedSquare[1];

                TestedSquare[2] = (TestedSquare[2] < rect.top) ?rect.right : TestedSquare[2];

                TestedSquare[3] = (TestedSquare[3] < rect.bottom) ?rect.right : TestedSquare[3];
            }
            return true;
        }
    }
    
    struct SelectionCoordinates
    {
        public double left;
        public double right;
        public double bottom;
        public double top;

        public SelectionCoordinates(double left, double right, double bottom, double top)
        {
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
        }
    }
}
