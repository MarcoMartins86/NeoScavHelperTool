using NeoScavModHelperTool;
using NeoScavModHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeoScavHelperTool.Viewer.Images
{
    /// <summary>
    /// Interaction logic for Images.xaml
    /// </summary>
    public partial class Images : UserControl, IChangeGUIType
    {
        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private Grid _gridCanvas = new Grid();
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        public Images()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;
        }

        private void ImagesControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                _alreadyLoaded = true;

                MainWindow.I.StartWaitSpinner();

                //Configure the grid
                _gridCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
                _gridCanvas.VerticalAlignment = VerticalAlignment.Stretch;
                
                _loadItemsWorker.RunWorkerAsync();
            }
        }

        private Panel CreateImagePanel(string label_text, string image_path)
        {
            //Create a new stack panel to hold a label and a canvas
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            //Create the label and add it to the panel
            Label label = new Label();
            label.Content = label_text;
            label.IsEnabled = false;
            label.FontSize = 16;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(label);
            //Create the image and convert it to use the App Dpi and add it to the panel
            Image image = new Image();
            image.Source = App.ConvertImageDpi(new BitmapImage(new Uri(image_path)), App.I.DpiX, App.I.DpiY);
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Top;
            panel.Children.Add(image);

            return panel;
        }

        private void LoadItemsWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            //1st - Get from DB all data to fill the grid and save it on the list
            //Fetch this item data from DB
            ViewerTreeItemDescriptor selectedItem = Viewer.I.SelectedItem;
            _arrayDBValues = App.DB.GetAllDataOfAnItemFromMemory(selectedItem.PrimaryKeyValue, selectedItem.PrimaryKeyName, selectedItem.TableName);
            //Fill a list with the data so it can be shown on the DataGrid
            foreach(object columnValue in _arrayDBValues)
            {                
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eImages)[_dataGridItems.Count], columnValue));
            }
            //2nd - Display the information on canvas 
            string strSmallImagePath = _arrayDBValues[(int)EDBImagesTableColumns.eSmall].ToString();
            string strBigImagePath = _arrayDBValues[(int)EDBImagesTableColumns.eBig].ToString();
            //Let's add the small image representation if it exists
            if (string.IsNullOrEmpty(strSmallImagePath) == false && System.IO.File.Exists(strSmallImagePath) == true)
            {
                //Create the small image panel in the GUI thread and add it to the grid on the correct place
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Panel panelSmallImage = CreateImagePanel("Small image representation:", strSmallImagePath);                    
                    _gridCanvas.Children.Add(panelSmallImage);
                    int nColumnIndex = _gridCanvas.ColumnDefinitions.Count;
                    _gridCanvas.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(panelSmallImage, nColumnIndex);
                }));                
            }
            if (string.IsNullOrEmpty(strBigImagePath) == false && System.IO.File.Exists(strBigImagePath) == true)
            {
                //Create the big image panel in the GUI thread and add it to the grid on the correct place
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Panel panelBigImage = CreateImagePanel("Big image representation:", strBigImagePath);
                    _gridCanvas.Children.Add(panelBigImage);
                    int nColumnIndex = _gridCanvas.ColumnDefinitions.Count;
                    _gridCanvas.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(panelBigImage, nColumnIndex);
                }));
            }
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Update the GUI
            ImagesTitle.Content = _arrayDBValues[(int)EDBImagesTableColumns.eName];
            ContainerImagesCanvas.Content = _gridCanvas;
            DataGridImages.ItemsSource = _dataGridItems;            
            ImagesMainGrid.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        public void ChangeGUIType(bool big_gui)
        {
            //on images we don't need to do anything since we are drawing both of the images
        }
    }
}

