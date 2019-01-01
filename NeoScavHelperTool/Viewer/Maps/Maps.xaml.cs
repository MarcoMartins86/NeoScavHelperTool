using NeoScavHelperTool.Viewer.Hextypes;
using NeoScavModHelperTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeoScavHelperTool.Viewer.Maps
{
    /// <summary>
    /// Interaction logic for Maps.xaml
    /// </summary>
    public partial class Maps : UserControl, IChangeGUIType
    {
        private enum EDayTime
        {
            eMorning,
            eDay,
            eDusk,
            eNight
        }

        static private EDayTime _eDayTime = EDayTime.eMorning; //static since I want to save the current value between items
        static private bool _bIsHighlighted = true; //static since I want to save the current value between items

        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _updateTileWorker = new BackgroundWorker();

        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        private Dictionary<EDayTime, BitmapSource> _dictMapsBitmaps = new Dictionary<EDayTime, BitmapSource>();

        public Maps()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _updateTileWorker.DoWork += UpdateTile_DoWork;
            _updateTileWorker.RunWorkerCompleted += UpdateTile_RunWorkerCompleted;
        }

        private void MapsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();

                _loadItemsWorker.RunWorkerAsync();
            }
        }

        private BitmapSource GetCorrectTileForDrawing(int hextypes, Dictionary<int, HextypesImages> dictImages)
        {
            BitmapSource sourceReturn = null;
            switch (_eDayTime)
            {
                case EDayTime.eMorning:
                    if (_bIsHighlighted)
                        sourceReturn = dictImages[hextypes].SummerMorningHighlighted;
                    else
                        sourceReturn = dictImages[hextypes].SummerMorning;
                    break;
                case EDayTime.eDay:
                    if (_bIsHighlighted)
                        sourceReturn = dictImages[hextypes].SummerDayHighlighted;
                    else
                        sourceReturn = dictImages[hextypes].SummerDay;
                    break;
                case EDayTime.eDusk:
                    if (_bIsHighlighted)
                        sourceReturn = dictImages[hextypes].SummerDuskHighlighted;
                    else
                        sourceReturn = dictImages[hextypes].SummerDusk;
                    break;
                case EDayTime.eNight:
                    if (_bIsHighlighted)
                        sourceReturn = dictImages[hextypes].SummerNightHighlighted;
                    else
                        sourceReturn = dictImages[hextypes].SummerNight;
                    break;
            }

            return sourceReturn;
        }

        private void CreateUpdateMapsCanvas()
        {
            _isOnBigGUI = MainWindow.I.IsBigGUISelected;

            Dictionary<int, HextypesImages> dictImages = _isOnBigGUI ? Hextypes.Hextypes.DictHextypesBigImages : Hextypes.Hextypes.DictHextypesSmallImages;

            string[] strMapRows = _arrayDBValues[(int)EDBMapsTableColumns.eStrDef].ToString().Split('\n');

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {                
                for (int nRows = 0; nRows < strMapRows.Length; nRows++)
                {
                    string[] strMapColumns = strMapRows[nRows].Split(',');
                    bool bIsRowEven = nRows % 2 == 0;
                    for (int nColumn = 0; nColumn < strMapColumns.Length; nColumn++)
                    {
                        // Check if we already have this hextype images created and if not create them
                        int nHextype = Convert.ToInt32(strMapColumns[nColumn]);
                        if (dictImages.ContainsKey(nHextype) == false)
                            dictImages.Add(nHextype, new HextypesImages(nHextype + 1, _isOnBigGUI));

                        BitmapSource tile = GetCorrectTileForDrawing(nHextype, dictImages);

                        // Calculation by trial and error of the formula to draw the tiles on map
                        double nStartX = (nColumn * tile.PixelWidth * 1.6);
                        if (bIsRowEven == false)
                            nStartX += tile.PixelWidth * 0.8;                        
                        double nStartY = nRows * tile.PixelHeight * 0.25;

                        drawingContext.DrawImage(tile, new Rect(nStartX, nStartY, tile.PixelWidth, tile.PixelHeight));
                    }
                }
                //drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));
                ////Brush brush = new Brush()
                //SolidColorBrush semiTransBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                //drawingContext.DrawRoundedRectangle(semiTransBrush, null, new Rect(10, 34, 80, 22), 5, 5);
                //FormattedText text = new FormattedText("Random", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 20.0, Brushes.Red);
                //drawingContext.DrawText(text, new Point(14, 30));
            }
            //RenderTargetBitmap mergedImage = new RenderTargetBitmap(nTotalWidth, nTotalHeight, 96, 96, PixelFormats.Pbgra32);
            //mergedImage.Render(visual);

            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(mergedImage));

            //using (FileStream file = File.OpenWrite("c:\\teste\\cenas.png"))
            //{
            //    encoder.Save(file);
            //}

            //BitmapSource final = App.ConvertImageDpi(mergedImage, App.I.DpiX, App.I.DpiY);
            DrawingImage finalMap = new DrawingImage(visual.Drawing);
            finalMap.Freeze();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ContainerMapsCanvas.Source = finalMap;
            }));
        }

        private void LoadItemsWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            //1st - Get from DB all data to fill the grid and save it on the list
            //Fetch this item data from DB
            ViewerTreeItemDescriptor selectedItem = Viewer.I.SelectedItem;
            _arrayDBValues = App.DB.GetAllDataOfAnItemFromMemory(selectedItem.PrimaryKeyValue, selectedItem.PrimaryKeyName, selectedItem.TableName);
            //2nd - Update the buttons states
            Dispatcher.Invoke(new Action(() =>
            {
                switch (_eDayTime)
                {
                    case EDayTime.eMorning:
                        RadioButtonMorning.IsChecked = true;
                        break;
                    case EDayTime.eDay:
                        RadioButtonDay.IsChecked = true;
                        break;
                    case EDayTime.eDusk:
                        RadioButtonDusk.IsChecked = true;
                        break;
                    case EDayTime.eNight:
                        RadioButtonNight.IsChecked = true;
                        break;
                }
                if (_bIsHighlighted)
                    ButtonHighlighted.IsChecked = true;
            }));
            //3rd - Display the information on canvas
            CreateUpdateMapsCanvas();

            _alreadyLoaded = true;
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
            ChangeGUIType(_isOnBigGUI);
        }

        private void HighlightedButton_Checked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = true;
            //Refresh the GUI to show reflect the change
            ChangeGUIType(_isOnBigGUI);
        }

        private void HighlightedButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = false;
            //Refresh the GUI to show reflect the change
            ChangeGUIType(_isOnBigGUI);
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update the GUI
            MapsTitle.Content = _arrayDBValues[(int)EDBMapsTableColumns.eStrName];
            //DataGridHextypes.ItemsSource = _dataGridItems;
            MapsMainGrid.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        private void UpdateTile_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateUpdateMapsCanvas();
        }

        private void UpdateTile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        public void ChangeGUIType(bool big_gui)
        {
            if (_alreadyLoaded)
            {
                MainWindow.I.StartWaitSpinner();
                _updateTileWorker.RunWorkerAsync();
            }
        }
    }
}
