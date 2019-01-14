using NeoScavHelperTool;
using NeoScavHelperTool.DBTableAttributes;
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

        private static BitmapSource _GUICellBigImage = null;
        public static BitmapSource GUICellBigImage
        {
            get
            {
                if(_GUICellBigImage == null)
                {
                    _GUICellBigImage = new BitmapImage(new Uri(GetImagePathFromMemory("GUICell", "0_images", true)));
                    _GUICellBigImage.Freeze();
                }

                return _GUICellBigImage;
            }
        }

        private static BitmapSource _GUICellSmallImage = null;
        public static BitmapSource GUICellSmallImage
        {
            get
            {
                if (_GUICellSmallImage == null)
                {
                    _GUICellSmallImage = new BitmapImage(new Uri(GetImagePathFromMemory("GUICell", "0_images", false)));
                    _GUICellSmallImage.Freeze();
                }

                return _GUICellSmallImage;
            }
        }


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
            BitmapSource imageSource = ConvertImageDpi(new BitmapImage(new Uri(image_path)), App.I.DpiX, App.I.DpiY);
            Image image = new Image();
            image.Source = imageSource;
            image.Width = imageSource.Width;
            image.Height = imageSource.Height;
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

        public void ChangeGUIType()
        {
            //on images we don't need to do anything since we are drawing both of the images
        }


        public static string GetImagePathFromMemory(string str_name, string str_table_name, bool big_gui)
        {
            return App.DB.GetImagePathFromMemory(str_name, str_table_name, big_gui);
        }

        public static BitmapSource GetImageToDraw(string str_name, string str_table_name, bool big_gui, bool is_mirrored)
        {
            string strImagePath = string.Empty;
            bool bNeedToUpscale = false;
            string strImageName = System.IO.Path.GetFileNameWithoutExtension(str_name);
            try
            {
                strImagePath = App.DB.GetImagePathFromMemory(strImageName, str_table_name, big_gui);
                if (string.IsNullOrEmpty(strImagePath) && big_gui)
                {
                    // if big image doesn't exist let's use the small one and upscale it
                    strImagePath = App.DB.GetImagePathFromMemory(strImageName, str_table_name, false);
                    bNeedToUpscale = true;
                }
            }
            catch //sometimes it gives an exception other times it returns string empty, :/ I should check this more carefully
            {
                // if big image doesn't exist let's use the small one and upscale it
                strImagePath = App.DB.GetImagePathFromMemory(strImageName, str_table_name, false);
                bNeedToUpscale = true;
            }            

            BitmapImage image = new BitmapImage(new Uri(strImagePath));
            BitmapSource sourceReturn = ConvertImageDpi(image, App.I.DpiX, App.I.DpiY);

            if(bNeedToUpscale && !is_mirrored)
                sourceReturn = new TransformedBitmap(sourceReturn, new ScaleTransform(-2, 2, 0, 0));
            else if(bNeedToUpscale)
                sourceReturn = new TransformedBitmap(sourceReturn, new ScaleTransform(2, 2));
            else if(is_mirrored)
                sourceReturn = new TransformedBitmap(sourceReturn, new ScaleTransform(-1, 1, 0, 0));

            sourceReturn.Freeze();
            return sourceReturn;
        }

        public static BitmapSource CopyImageRectWithDpi(BitmapSource image, Int32Rect rect, double dpiX, double dpiY)
        {
            int width = rect.Width;
            int height = rect.Height;
            var stride = (width * image.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[stride * height];
            image.CopyPixels(rect, pixelData, stride, 0);
            return BitmapSource.Create(width, height, dpiX, dpiY, image.Format, image.Palette, pixelData, stride);
        }

        public static BitmapSource ConvertImageDpi(BitmapSource image, double dpiX, double dpiY)
        {
            //hack to convert to same dpi //maybe if this is to slow consider using transforms  
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            var stride = (width * image.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[stride * height];
            image.CopyPixels(pixelData, stride, 0);
            return BitmapSource.Create(width, height, dpiX, dpiY, image.Format, image.Palette, pixelData, stride);
        }
    }
}

