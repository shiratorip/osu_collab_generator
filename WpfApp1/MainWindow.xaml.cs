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
using System.Linq;

namespace OCG
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
        private List<BoundingBox> BoundingBoxes = new List<BoundingBox>();
        private BoundingBox? currentSelectionCoordinates;
        private Dictionary<Rectangle, BoundingBox> rectanglesCoordsBinds = new();
        private Dictionary<Button, Rectangle> butsSelectionsBinds = new();
        private Button? selectedBut;
        private int selectedButCounter;

        public BitmapImage? image;

        bool imageSelected = false;

        private readonly OpenFileDialog openImageDialog = new OpenFileDialog
        {
            Filter = "PNG image (*.png)|*.png|BMP image (*.bmp)|*.bmp|All files (*.*)|*.*"
            
        };

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            if (openImageDialog.ShowDialog() == true)
            {
                mouseDown = false;
                BoundingBoxes.Clear();
                currentSelectionCoordinates = null;
                StorageCanvas.Children.Clear();
                selectionBox.Visibility = Visibility.Collapsed;
                rectanglesCoordsBinds.Clear();
                butsSelectionsBinds.Clear();
                selectedBut = null;
                selectedButCounter = -1;

                BitmapImage bitmap = new BitmapImage(new Uri(openImageDialog.FileName));
                {
                    Imported_image.Source = bitmap;

                    //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                    innerGrid.Width = bitmap.Width;
                    innerGrid.Height = bitmap.Height;
                    imageSelected = true;
                }

                bitmap.DownloadCompleted += delegate
                {
                    Imported_image.Source = bitmap;

                    //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                    innerGrid.Width = bitmap.Width;
                    innerGrid.Height = bitmap.Height;
                    imageSelected = true;
                };
            }
            
        }

        public void SetImage(BitmapImage bitmap)
        {
            Imported_image.Source = bitmap;
            innerGrid.Width = bitmap.Width;
            innerGrid.Height = bitmap.Height;
            imageSelected = true;
            this.image = bitmap;
        }

        private void OpenImageBrowser(object sender, RoutedEventArgs e)
        {
            ImageBrowser win1 = new ImageBrowser(this);
            win1.Show();
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e) {
            if (!imageSelected)
            {
                return;
            }
            mouseDown = true;
            mouseDownPos = e.GetPosition(theGrid);
            theGrid.CaptureMouse();

            Canvas.SetLeft(selectionBox, mouseDownPos.X);
            Canvas.SetTop(selectionBox, mouseDownPos.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;

            selectionBox.Visibility = Visibility.Visible;
        }
        private void GridMouseUp(object sender, MouseButtonEventArgs e) {
            mouseDown = false;
            theGrid.ReleaseMouseCapture();

            if (!imageSelected)
            {
                return;
            }

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
            BoundingBox box = new BoundingBox(left, right, top, bottom);
            foreach (var rect in BoundingBoxes)
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
        private void GridMouseMove(object sender, MouseEventArgs e) {
            if (!imageSelected)
            {
                return;
            }
            if (mouseDown)
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

        private void ButtonClick(object sender, RoutedEventArgs e)
        {   
            
            if(currentSelectionCoordinates != null && selectionBox.Visibility == Visibility.Visible)
            {
                AddButonAndSelect(currentSelectionCoordinates.Value);

                BoundingBox Box = currentSelectionCoordinates.Value;
                BoundingBoxes.Add(Box);

                selectionBox.Visibility = Visibility.Collapsed;

                //currentSelectionCoordinates = null;
            }
        }
        private Rectangle AddSelection(BoundingBox boundingBox)
        {
            if (selectionBox.Visibility == Visibility.Visible)
            {
                Rectangle rec = new Rectangle()
                {
                    Width = selectionBox.Width,
                    Height = selectionBox.Height,

                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                };
                StorageCanvas.Children.Add(rec);
                return rec;
            }
            else return null;

        }

            private Button AddButton(BoundingBox boundingBox)
            {
            Rectangle rec = AddSelection(boundingBox);
            if (rec != null)
            {
                Button button = new Button()
                {
                    Height = 20,
                    Width = 20
                };

                int currentCounter = ButtonsGrid.Children.Count;

                Grid.SetColumn(button, currentCounter % 5);
                Grid.SetRow(button, (currentCounter / 5));
                button.Click += delegate
                {
                    SelectButton(rec, button, currentCounter);
                };
                rectanglesCoordsBinds.Add(rec, boundingBox);
                butsSelectionsBinds.Add(button, rec);
                ButtonsGrid.Children.Add(button);

                Canvas.SetTop(rec, boundingBox.top - innerGrid.Margin.Top);
                Canvas.SetLeft(rec, boundingBox.left - innerGrid.Margin.Left);

                return button;
            }
            else return null;
        }

        private Button AddButonAndSelect(BoundingBox boundingBox)
        {   
            var button = AddButton(boundingBox);
            if (button != null) {          
                SelectButton(butsSelectionsBinds[button], button, ButtonsGrid.Children.Count - 1);
        }
            return button;
        }

        private void SelectButton(Rectangle rec, Button button, int buttonIndex)
        {
            foreach (Rectangle item in StorageCanvas.Children)
            {
                item.Stroke = Brushes.Blue;
            }
            rec.Stroke = Brushes.Red;
            selectedBut = button;
            selectedButCounter = buttonIndex;
        }

        private void DeleteSelection(object sender, RoutedEventArgs e)

        {
            if(selectedBut != null)
            {
                ButtonsGrid.Children.Remove(selectedBut);
                foreach (Button item in ButtonsGrid.Children)
                {
                    int itemCounter = Grid.GetRow(item) * 5 + Grid.GetColumn(item);
                    if (itemCounter > selectedButCounter)
                    {
                        itemCounter--;
                        int row = itemCounter / 5;
                        int column = itemCounter % 5;
                        Grid.SetRow(item, row);
                        Grid.SetColumn(item, column);
                    }
                }
                var selectedRect = butsSelectionsBinds[selectedBut];
                StorageCanvas.Children.Remove(selectedRect);
                BoundingBoxes.Remove(rectanglesCoordsBinds[selectedRect]);
                rectanglesCoordsBinds.Remove(selectedRect);
                butsSelectionsBinds.Remove(selectedBut);

                selectedButCounter = ButtonsGrid.Children.Count;
                if (selectedButCounter > 0) { 
                //complexity
                    selectedBut = butsSelectionsBinds.FirstOrDefault(x => x.Value.Equals(rectanglesCoordsBinds.FirstOrDefault(x => x.Value.Equals(BoundingBoxes.Last())).Key)).Key;
                    
                    SelectButton(butsSelectionsBinds[selectedBut], selectedBut, selectedButCounter);
                }
                else {  selectedBut = null;}
               
            }
        }

        private void ApplyUser(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("Test");

        }

        private void ExportCollab(object sender, RoutedEventArgs e)
        {
            string exportString = "[imagemap]\n";
            exportString += $"{image.UriSource}\n";
            foreach (BoundingBox coords in BoundingBoxes)
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
