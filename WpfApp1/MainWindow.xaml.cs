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
using System.Threading;

namespace osuCollabGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary
    /// collab maker project
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            userSelectionStorage = new UserSelectionStorage(this);
            searchTimer.Tick += async delegate
            {
                searchTimer.Stop();
                try
                {
                    searchMutex.WaitOne();
                    Trace.WriteLine("Search initiated");
                    usersStack.Children.Clear();
                    UserCompact[]? users = await MainWindowHelpers.SearchUsers($"https://osu-collab-generator-api.shuttleapp.rs/username/{searchBox.Text}");
                    if (users != null)
                    {
                        List<UserStorage> usersList = new List<UserStorage>(users.Length);
                        foreach (var user in users)
                        {
                            usersList.Add(new UserStorage(user.Username, user.Id, user.AvatarUrl));
                        }
                        PopulateUsersList(usersList);
                    }
                    else
                    {
                        usersStack.Children.Clear();
                    }
                    usersStack.UpdateLayout();
                    usersScroll.Visibility = Visibility.Visible;
                }
                finally
                {
                    searchMutex.ReleaseMutex();
                }
            };
            InitializeComponent();
        }
        
        private bool mouseDown = false;
        private Point mouseDownPos;
        private BoundingBox? currentSelectionCoordinates;
        public BitmapImage? image;

        private UserSelectionStorage userSelectionStorage;
        
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
                currentSelectionCoordinates = null;
                StorageCanvas.Children.Clear();
                selectionBox.Visibility = Visibility.Collapsed;
                userSelectionStorage.Clear();

                BitmapImage bitmap = new BitmapImage(new Uri(openImageDialog.FileName));
                Imported_image.Source = bitmap;

                //Trace.WriteLine($"{bitmap.Height} {bitmap.Width} {Imported_image.ActualHeight} {Imported_image.ActualWidth}");
                innerGrid.Width = bitmap.Width;
                innerGrid.Height = bitmap.Height;
                imageSelected = true;
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
        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
           /* if (!searchBox.IsMouseOver && !usersScroll.IsMouseDirectlyOver)
            {
                Keyboard.ClearFocus();
                usersScroll.Visibility = Visibility.Hidden;
            }*/
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
            foreach (var rect in userSelectionStorage.BoundingBoxes)
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

                selectionBox.Visibility = Visibility.Collapsed;

                //currentSelectionCoordinates = null;
            }
        }

        private int AddButton(BoundingBox boundingBox)
        {
            return userSelectionStorage.AddBoundingBox(boundingBox);
        }

        private int AddButonAndSelect(BoundingBox boundingBox)
        {   
            int index = AddButton(boundingBox);
            userSelectionStorage.Select(index);
            return index;
        }

        private void DeleteSelection(object sender, RoutedEventArgs e)
        {
            userSelectionStorage.DeleteCurrentSelection();
        }

        private void ApplyUser(object sender, RoutedEventArgs e)
        {
        }

        private void ExportCollab(object sender, RoutedEventArgs e)
        {
            if(image != null && userSelectionStorage.BoundingBoxes.Count != 0)
            {
                string exportString = "[imagemap]\n";
                exportString += $"{image.UriSource}\n";
                for (int i = 0; i < userSelectionStorage.BoundingBoxes.Count; i++)
                {
                    UserStorage? user = userSelectionStorage.Users[i];
                    if (user != null)
                    {
                        BoundingBox coords = userSelectionStorage.BoundingBoxes[i];
                        double left = (coords.left - innerGrid.Margin.Left) / innerGrid.Width * 100;
                        double top = (coords.top - innerGrid.Margin.Top) / innerGrid.Height * 100;
                        double width = (coords.right - coords.left) / innerGrid.Width * 100;
                        double height = (coords.bottom - coords.top) / innerGrid.Height * 100;

                        exportString += $"{left} {top} {width} {height} https://osu.ppy.sh/users/{user.Id} {user.Username}\n";
                    }
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
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
            else
            {
                Brush b = ExportButton.Background;
                ExportButton.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE7, 0x7D, 0x83));
                ExportButton.Content = "Collab Export failed!";
                
                DispatcherTimer timer = new DispatcherTimer();

                timer.Tick += delegate
                {
                    ExportButton.Content = "Export";
                    ExportButton.Background = b;
                    timer.Stop();
                };
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
        }

        private readonly DispatcherTimer searchTimer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        private readonly Mutex searchMutex = new Mutex();
        private void SearchUsers(object sender, RoutedEventArgs e)
        {
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void PopulateUsersList(List<UserStorage> usersList)
        {
            foreach (UserStorage user in usersList)
            {
                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = usersStack.ActualWidth
                };

                BitmapImage bitmap = new BitmapImage(new Uri(user.AvatarUrl));
                Image img = new Image()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Stretch = Stretch.Fill,
                    Source = bitmap,
                    Width = usersStack.ActualWidth * 0.3
                };
                stackPanel.Children.Add(img);

                Label label = new Label() {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = user.Username
                };
                stackPanel.Children.Add(label);

                Button button = new Button()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = usersStack.ActualWidth,
                    Content = stackPanel
                };
                button.Click += delegate
                {
                    userSelectionStorage.SetUserForCurrentSelection(user);
                    usersScroll.Visibility = Visibility.Hidden;
                };
                usersStack.Children.Add(button);
            }
        }
    }
}
