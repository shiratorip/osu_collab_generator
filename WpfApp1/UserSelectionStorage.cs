using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace osuCollabGenerator
{
    internal class UserSelectionStorage
    {
        private readonly MainWindow window;
        public UserSelectionStorage(MainWindow window)
        {
            this.window = window;
        }

        private readonly List<BoundingBox> boundingBoxes = new List<BoundingBox>();
        private readonly List<Rectangle> rectangles = new List<Rectangle>();
        private readonly List<UserStorage?> users = new List<UserStorage?>();

        public List<BoundingBox> BoundingBoxes { get { return boundingBoxes; } }
        public List<UserStorage?> Users { get { return users; } }

        private int? selectionIndex;

        public void Clear()
        {
            selectionIndex = null;
            users.Clear();
            rectangles.Clear();
            boundingBoxes.Clear();
            window.StorageCanvas.Children.Clear();
            window.ButtonsGrid.Children.Clear();
        }

        public int AddBoundingBox(BoundingBox boundingBox, TextBlock CurName)
        {
            Rectangle rec = new Rectangle()
            {
                Width = boundingBox.right - boundingBox.left,
                Height = boundingBox.bottom - boundingBox.top,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
            };

            window.StorageCanvas.Children.Add(rec);

            Canvas.SetTop(rec, boundingBox.top - window.innerGrid.Margin.Top);
            Canvas.SetLeft(rec, boundingBox.left - window.innerGrid.Margin.Left);

            boundingBoxes.Add(boundingBox);
            rectangles.Add(rec);
            users.Add(null);

            Button button = new Button()
            {
                Height = 20,
                Width = 20
        };
            

            int index = boundingBoxes.Count - 1;

            Grid.SetColumn(button, index % 5);
            Grid.SetRow(button, (index / 5));
            button.Click += delegate
            {
                int index = window.ButtonsGrid.Children.IndexOf(button);
                Select(index);

                if(users.Count>=index && users[index]!=null) {
                    CurName.Text = users[index].Username;
                }
                else
                {
                    CurName.Text ="...";

                }
            };
            window.ButtonsGrid.Children.Add(button);

            return index;
        }

        public void DeleteCurrentSelection()
        {
            if (selectionIndex.HasValue)
            {
                window.ButtonsGrid.Children.RemoveAt(selectionIndex.Value);
                foreach (Button item in window.ButtonsGrid.Children)
                {
                    int itemCounter = Grid.GetRow(item) * 5 + Grid.GetColumn(item);
                    if (itemCounter > selectionIndex.Value)
                    {
                        itemCounter--;
                        int row = itemCounter / 5;
                        int column = itemCounter % 5;
                        Grid.SetRow(item, row);
                        Grid.SetColumn(item, column);
                    }
                }
                window.StorageCanvas.Children.RemoveAt(selectionIndex.Value);
                boundingBoxes.RemoveAt(selectionIndex.Value);
                rectangles.RemoveAt(selectionIndex.Value);
                users.RemoveAt(selectionIndex.Value);
                    
                selectionIndex = null;
            }
        }

        public void SetUserForCurrentSelection(UserStorage user)
        {
            if (selectionIndex.HasValue)
            {
                users[selectionIndex.Value] = user;
            }
        }

        public void Select(int index)
        {
            if(selectionIndex.HasValue)
            {
                Rectangle prevRect = (Rectangle)window.StorageCanvas.Children[selectionIndex.Value];
                Button prevButton = (Button)window.ButtonsGrid.Children[selectionIndex.Value];
                prevButton.Background = Brushes.Blue;
                prevRect.Stroke = Brushes.Blue;
            }
            Rectangle rect = (Rectangle)window.StorageCanvas.Children[index];
            Button button = (Button)window.ButtonsGrid.Children[index];
            button.Background = Brushes.Red;
            rect.Stroke = Brushes.Red;
            selectionIndex = index;
        }
    }
}
