using MahApps.Metro.Controls;
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
using static NeoScavHelperTool.Viewer.Maps.Maps;

namespace NeoScavHelperTool.Viewer.HexTypes
{
    /// <summary>
    /// Interaction logic for HexTypes.xaml
    /// </summary>
    public partial class HexTypes : UserControl, IChangeGUIType
    {   
        public struct SizeTile
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public SizeTile(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }
        private static SizeTile _sizeBigTile = new SizeTile(0, 0);
        public static SizeTile SizeBigTile
        {
            get
            {
                // Check if we already initialized this and if not let's do it
                if(_sizeBigTile.Width == 0)
                {
                    BitmapSource tile = GetCorrectTileForDrawing(0, true, EDayTime.eMorning, true);
                    _sizeBigTile.Height = tile.PixelHeight;
                    _sizeBigTile.Width = tile.PixelWidth;
                }

                return _sizeBigTile;
            }
        }
        private static SizeTile _sizeSmallTile = new SizeTile(0, 0);
        public static SizeTile SizeSmallTile
        {
            get
            {
                // Check if we already initialized this and if not let's do it
                if (_sizeSmallTile.Width == 0)
                {
                    BitmapSource tile = GetCorrectTileForDrawing(0, false, EDayTime.eMorning, true);
                    _sizeSmallTile.Height = tile.PixelHeight;
                    _sizeSmallTile.Width = tile.PixelWidth;
                }

                return _sizeSmallTile;
            }
        }

        private static Dictionary<int, HexTypesImages> _dictHexTypesBigImages = new Dictionary<int, HexTypesImages>();
        public static Dictionary<int, HexTypesImages> DictHexTypesBigImages
        {
            get
            {
                return _dictHexTypesBigImages;
            }
            set
            {
                _dictHexTypesBigImages = value;
            }
        }

        private static Dictionary<int, HexTypesImages> _dictHexTypesSmallImages = new Dictionary<int, HexTypesImages>();
        public static Dictionary<int, HexTypesImages> DictHexTypesSmallImages
        {
            get
            {
                return _dictHexTypesSmallImages;
            }
            set
            {
                _dictHexTypesSmallImages = value;
            }
        }

        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private Grid _gridCanvasBigGUI = new Grid();
        private Grid _gridCanvasSmallGUI = new Grid();

        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        private Image _tileBigImage = new Image();
        private Image _tileSmallImage = new Image();
        static private EDayTime _eDayTime = EDayTime.eMorning; //static since I want to save the current value between items
        static private bool _bIsHighlighted = true; //static since I want to save the current value between items

        public HexTypes()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void HexTypesControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();

                //Configure the grid
                _gridCanvasBigGUI.HorizontalAlignment = HorizontalAlignment.Stretch;
                _gridCanvasBigGUI.VerticalAlignment = VerticalAlignment.Stretch;
                _gridCanvasSmallGUI.HorizontalAlignment = HorizontalAlignment.Stretch;
                _gridCanvasSmallGUI.VerticalAlignment = VerticalAlignment.Stretch;

                _loadItemsWorker.RunWorkerAsync();
            }
        }

        private Grid CreateButtonsGrid()
        {
            Grid buttonsGrid = new Grid();
            buttonsGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            buttonsGrid.VerticalAlignment = VerticalAlignment.Top;

            RadioButton buttonMorning = new RadioButton();
            buttonMorning.Content = "Morning";
            buttonMorning.GroupName = "DayTime";
            buttonMorning.Checked += DayTimeRadioButton_Checked;
            buttonMorning.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsGrid.Children.Add(buttonMorning);
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(buttonMorning, 0);

            RadioButton buttonDay = new RadioButton();
            buttonDay.Content = "Day";
            buttonDay.GroupName = "DayTime";
            buttonDay.Checked += DayTimeRadioButton_Checked;
            buttonDay.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsGrid.Children.Add(buttonDay);
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(buttonDay, 1);

            RadioButton buttonDusk = new RadioButton();
            buttonDusk.Content = "Dusk";
            buttonDusk.GroupName = "DayTime";
            buttonDusk.Checked += DayTimeRadioButton_Checked;
            buttonDusk.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsGrid.Children.Add(buttonDusk);
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(buttonDusk, 2);

            RadioButton buttonNight = new RadioButton();
            buttonNight.Content = "Night";
            buttonNight.GroupName = "DayTime";
            buttonNight.Checked += DayTimeRadioButton_Checked;
            buttonNight.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsGrid.Children.Add(buttonNight);
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(buttonNight, 3);

            ToggleSwitch buttonHighlighted = new ToggleSwitch();
            buttonHighlighted.OnLabel = "Highlighted";
            buttonHighlighted.OffLabel = "Highlighted";
            buttonHighlighted.Checked += HighlightedButton_Checked;
            buttonHighlighted.Unchecked += HighlightedButton_Unchecked;            
            Style style = this.FindResource("CustomMetroToggleSwitch") as Style;
            buttonHighlighted.Style = style;
            buttonHighlighted.HorizontalAlignment = HorizontalAlignment.Center;
            buttonsGrid.Children.Add(buttonHighlighted);
            buttonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(buttonHighlighted, 4);

            //Set the default values            
            switch(_eDayTime)
            {
                case EDayTime.eMorning:
                    buttonMorning.IsChecked = true;
                    break;
                case EDayTime.eDay:
                    buttonDay.IsChecked = true;
                    break;
                case EDayTime.eDusk:
                    buttonDusk.IsChecked = true;
                    break;
                case EDayTime.eNight:
                    buttonNight.IsChecked = true;
                    break;
            }
            if (_bIsHighlighted)
                buttonHighlighted.IsChecked = true;

            return buttonsGrid;
        }

        private void CreateHexTypesCanvas()
        {
            Grid gridCanvas = _isOnBigGUI ? _gridCanvasBigGUI : _gridCanvasSmallGUI;
            // Create the buttons
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Create the grid with buttons
                Grid gridButtons = CreateButtonsGrid();
                gridCanvas.Children.Add(gridButtons);
                RowDefinition buttonsRow = new RowDefinition();
                buttonsRow.Height = GridLength.Auto;
                gridCanvas.RowDefinitions.Add(buttonsRow);
            }));

            // Fetch the tile
            int nHexTypesID = Convert.ToInt32(_arrayDBValues[(int)EDBHexTypesTableColumns.eId]);
            BitmapSource tile = GetCorrectTileForDrawing(nHexTypesID - 1, _isOnBigGUI, _eDayTime, _bIsHighlighted);
            Image image = _isOnBigGUI ? _tileBigImage : _tileSmallImage;

            Dispatcher.BeginInvoke(new Action(() =>
            {                
                SetImageSourceWithTile(image, tile);
                gridCanvas.Children.Add(image);
                gridCanvas.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(image, 1);

                ContainerHexTypesCanvas.Content = gridCanvas;
            }));
        }

        private void CreateUpdateHexTypesCanvas()
        {
            bool bIsBigGUISelected = MainWindow.I.IsBigGUISelected;
            // 1st - Let's check if we need to create the canvas or just update the control
            bool bNeedToCreateCanvas = false;
            Dispatcher.Invoke(new Action(() =>
            {
                if (_isOnBigGUI != bIsBigGUISelected)
                {
                    if (bIsBigGUISelected)
                    {
                        if (_gridCanvasBigGUI.Children.Count > 0)
                            ContainerHexTypesCanvas.Content = _gridCanvasBigGUI; // Just update   
                        else
                            bNeedToCreateCanvas = true;
                    }
                    else
                    {
                        if (_gridCanvasSmallGUI.Children.Count > 0)
                            ContainerHexTypesCanvas.Content = _gridCanvasSmallGUI; // Just update
                        else
                            bNeedToCreateCanvas = true;
                    }
                }
                else if (_gridCanvasBigGUI.Children.Count == 0) // This will be used on creation if on big mode
                    bNeedToCreateCanvas = true;
            }));

            // Update the GUI mode
            _isOnBigGUI = bIsBigGUISelected;

            //2nd - If we need to create let's do it
            if (bNeedToCreateCanvas)
            {
                CreateHexTypesCanvas();
            }            
        }

        private void LoadItemsWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            //1st - Get from DB all data to fill the grid and save it on the list
            //Fetch this item data from DB
            ViewerTreeItemDescriptor selectedItem = Viewer.I.SelectedItem;
            _arrayDBValues = App.DB.GetAllDataOfAnItemFromMemory(selectedItem.PrimaryKeyValue, selectedItem.PrimaryKeyName, selectedItem.TableName);
            //Fill a list with the data so it can be shown on the DataGrid
            foreach (object columnValue in _arrayDBValues)
            {
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eHexTypes)[_dataGridItems.Count], columnValue));
            }
            //2nd - Display the information on canvas
            CreateUpdateHexTypesCanvas();       
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update the GUI
            HexTypesTitle.Content = _arrayDBValues[(int)EDBHexTypesTableColumns.eStrName];
            DataGridHexTypes.ItemsSource = _dataGridItems;
            HexTypesMainGrid.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        private void SetImageSourceWithTile(Image image, BitmapSource tile)
        {
            if (image != null)
            {
                image.Source = tile;
                image.Width = tile.Width;
                image.Height = tile.Height;
            }
        }

        public static BitmapSource GetCorrectTileForDrawing(int hextypes_index, bool big_gui, EDayTime day_time, bool is_highlighted)
        {
            Dictionary<int, HexTypesImages> dictImages = big_gui ? _dictHexTypesBigImages : _dictHexTypesSmallImages;
            // Check if we already have this hextype images created and if not create them        
            if (dictImages.ContainsKey(hextypes_index) == false)
            {
                HexTypesImages hexImage = new HexTypesImages(hextypes_index + 1, big_gui);
                // Recheck just in case (once crashed here saying that the same key was already there,
                // must be a concurrency problem, this might suffice and if not use a mutex
                if (dictImages.ContainsKey(hextypes_index) == false)
                    dictImages.Add(hextypes_index, hexImage);
            }

            BitmapSource sourceReturn = null;
            switch (day_time)
            {
                case EDayTime.eMorning:
                    if (is_highlighted)
                        sourceReturn = dictImages[hextypes_index].SummerMorningHighlighted;
                    else
                        sourceReturn = dictImages[hextypes_index].SummerMorning;
                    break;
                case EDayTime.eDay:
                    if (is_highlighted)
                        sourceReturn = dictImages[hextypes_index].SummerDayHighlighted;
                    else
                        sourceReturn = dictImages[hextypes_index].SummerDay;
                    break;
                case EDayTime.eDusk:
                    if (is_highlighted)
                        sourceReturn = dictImages[hextypes_index].SummerDuskHighlighted;
                    else
                        sourceReturn = dictImages[hextypes_index].SummerDusk;
                    break;
                case EDayTime.eNight:
                    if (is_highlighted)
                        sourceReturn = dictImages[hextypes_index].SummerNightHighlighted;
                    else
                        sourceReturn = dictImages[hextypes_index].SummerNight;
                    break;
            }

            return sourceReturn;
        }

        private void RefreshGUIAfterButtonPress()
        {
            if (_alreadyLoaded)
            {                
                int nHexTypesId = Convert.ToInt32(_arrayDBValues[(int)EDBHexTypesTableColumns.eId]);
                SetImageSourceWithTile(_isOnBigGUI ? _tileBigImage : _tileSmallImage, GetCorrectTileForDrawing(nHexTypesId - 1, _isOnBigGUI, _eDayTime, _bIsHighlighted));
                //Restore focus to tree view so user can navigate with keyboard
                Viewer.I.RestoreFocusSelectedItem();
            }
        }

        private void DayTimeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            //Update the control variable with correct day time
            switch (radioButton.Content.ToString())
            {
                case "Morning":
                    _eDayTime = EDayTime.eMorning;
                    break;
                case "Day":
                    _eDayTime = EDayTime.eDay;
                    break;
                case "Dusk":
                    _eDayTime = EDayTime.eDusk;
                    break;
                case "Night":
                    _eDayTime = EDayTime.eNight;
                    break;
            }
            //Refresh the GUI to show reflect the change
            RefreshGUIAfterButtonPress();
        }

        private void HighlightedButton_Checked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = true;
            //Refresh the GUI to show reflect the change
            RefreshGUIAfterButtonPress();
        }

        private void HighlightedButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = false;
            //Refresh the GUI to show reflect the change
            RefreshGUIAfterButtonPress();
        }

        private void ChangeGUIType_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateUpdateHexTypesCanvas();
        }

        private void ChangeGUIType_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        public void ChangeGUIType()
        {
            MainWindow.I.StartWaitSpinner();
            _changeGUITypeWorker.RunWorkerAsync();
        }
    }
}

