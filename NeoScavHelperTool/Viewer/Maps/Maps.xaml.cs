using NeoScavHelperTool.Viewer.HexTypes;
using NeoScavHelperTool;
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
using NeoScavHelperTool.DBTableAttributes;
using static NeoScavHelperTool.Viewer.HexTypes.HexTypes;

namespace NeoScavHelperTool.Viewer.Maps
{
    /// <summary>
    /// Interaction logic for Maps.xaml
    /// </summary>
    public partial class Maps : UserControl, IChangeGUIType
    {
        public struct SizeMap
        {
            public int Rows { get; set; }
            public int Columns { get; set; }

            public SizeMap(int rows, int columns)
            {
                Rows = rows;
                Columns = columns;
            }
        }

        private static SizeMap _sizeGameMap = new SizeMap(0, 0);
        public static SizeMap SizeGameMap
        {
            get
            {
                // Check if we already initialized this and if not let's do it
                if (_sizeGameMap.Rows == 0)
                {
                    // Since this method is only used to fetch the game map size we can have this hard-coded
                    object[] arrayDBValues = App.DB.GetAllDataOfAnItemFromMemory("2", DBTableAttributtesFetcher.GetPrimaryKeyName(EDBTable.eMaps), "0_maps");

                    string [] strMapRows = arrayDBValues[(int)EDBMapsTableColumns.eStrDef].ToString().Split('\n');
                    string[] strMapColumns = strMapRows[0].Split(',');

                    _sizeGameMap.Rows = strMapRows.Length;
                    _sizeGameMap.Columns = strMapColumns.Length;
                }

                return _sizeGameMap;
            }
        }

        public enum EDayTime
        {
            eMorning,
            eDay,
            eDusk,
            eNight
        }

        static private EDayTime _eDayTime = EDayTime.eMorning; //static since I want to save the current value between items
        static private bool _bIsHighlighted = true; //static since I want to save the current value between items

        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        public Maps()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void MapsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();

                _loadItemsWorker.RunWorkerAsync();
            }
        }

        private static DrawingImage GetMapImage(bool big_gui, object[] array_db_values, EDayTime day_time, bool is_highlighted)
        {
            return GetMapImageWithDrawnImageAtPoint(big_gui, array_db_values, day_time, is_highlighted, null, null);
        }

        public static DrawingImage GetGameMapImageWithDrawnImageAtPoint(bool big_gui, EDayTime day_time, bool is_highlighted, BitmapSource image_to_draw, Point? where_to_draw)
        {
            // Since this method is only used to fetch the game map we can have this hard-coded
            object [] arrayDBValues = App.DB.GetAllDataOfAnItemFromMemory("2", DBTableAttributtesFetcher.GetPrimaryKeyName(EDBTable.eMaps), "0_maps");

            return GetMapImageWithDrawnImageAtPoint(big_gui, arrayDBValues, day_time, is_highlighted, image_to_draw, where_to_draw);
        }

        private static DrawingImage GetMapImageWithDrawnImageAtPoint(bool big_gui, object[] array_db_values, EDayTime day_time, bool is_highlighted, BitmapSource image_to_draw, Point? where_to_draw)
        {
            string[] strMapRows = array_db_values[(int)EDBMapsTableColumns.eStrDef].ToString().Split('\n');

            SizeTile sizeTile = big_gui ? HexTypes.HexTypes.SizeBigTile : HexTypes.HexTypes.SizeSmallTile;

            // Calculation by trial and error of the formula to draw the tiles on map
            double nStartYMultiplier = sizeTile.Height * 0.25;
            double nStartXOddOffset = sizeTile.Width * 0.8;
            double nStartXMultiplier = 2 * nStartXOddOffset;

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                for (int nRows = 0; nRows < strMapRows.Length; nRows++)
                {
                    // Calculation by trial and error of the formula to draw the tiles on map
                    double nStartY = nRows * nStartYMultiplier;

                    string[] strMapColumns = strMapRows[nRows].Split(',');
                    bool bIsRowOdd = nRows % 2 != 0;
                    for (int nColumn = 0; nColumn < strMapColumns.Length; nColumn++)
                    {
                        int nHextype = Convert.ToInt32(strMapColumns[nColumn]);
                        BitmapSource tile = HexTypes.HexTypes.GetCorrectTileForDrawing(nHextype, big_gui, day_time, is_highlighted);

                        // Calculation by trial and error of the formula to draw the tiles on map
                        double nStartX = (nColumn * nStartXMultiplier);
                        if (bIsRowOdd)
                            nStartX += nStartXOddOffset;                        

                        drawingContext.DrawImage(tile, new Rect(nStartX, nStartY, tile.PixelWidth, tile.PixelHeight));
                    }
                }
                // As a final step let's see if there's an image to draw
                if(image_to_draw != null)
                {
                    drawingContext.DrawImage(image_to_draw, new Rect(where_to_draw.Value.X, where_to_draw.Value.Y, image_to_draw.PixelWidth, image_to_draw.PixelHeight));
                }


                // Below code might be useful to save things to a file, and since it took me a long time to find it, I will maintain this here for now in case I will need it later

                //drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));
                ////Brush brush = new Brush()
                //SolidColorBrush semiTransBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                //drawingContext.DrawRoundedRectangle(semiTransBrush, null, new Rect(10, 34, 80, 22), 5, 5);
                //FormattedText text = new FormattedText("Random", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 20.0, Brushes.Red);
                //drawingContext.DrawText(text, new Point(14, 30));
            }
            //RenderTargetBitmap mergedImage = new RenderTargetBitmap(nTotalWidth, nTotalHeight, 96, 96, PixelFormats.Pbgra32);
            //mergedImage.Render(visual);
            //BitmapSource final = App.ConvertImageDpi(mergedImage, App.I.DpiX, App.I.DpiY);

            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(mergedImage));

            //using (FileStream file = File.OpenWrite("c:\\teste\\cenas.png"))
            //{
            //    encoder.Save(file);
            //}

            DrawingImage finalMap = new DrawingImage(visual.Drawing);
            finalMap.Freeze();

            return finalMap;
        }

        private void CreateUpdateMapsCanvas()
        {
            _isOnBigGUI = MainWindow.I.IsBigGUISelected;

            DrawingImage finalMap = GetMapImage(_isOnBigGUI, _arrayDBValues, _eDayTime, _bIsHighlighted);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ContainerMapsCanvas.Source = finalMap;
                ContainerMapsCanvas.Width = finalMap.Width;
                ContainerMapsCanvas.Height = finalMap.Height;
            }));
        }

        private void LoadItemsWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            //1st - Get from DB all data
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
            ChangeGUIType();
        }

        private void HighlightedButton_Checked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = true;
            //Refresh the GUI to show reflect the change
            ChangeGUIType();
        }

        private void HighlightedButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _bIsHighlighted = false;
            //Refresh the GUI to show reflect the change
            ChangeGUIType();
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update the GUI
            MapsTitle.Content = _arrayDBValues[(int)EDBMapsTableColumns.eStrName];
            MapsMainGrid.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        private void ChangeGUIType_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateUpdateMapsCanvas();
        }

        private void ChangeGUIType_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        public void ChangeGUIType()
        {
            if (_alreadyLoaded)
            {
                MainWindow.I.StartWaitSpinner();
                _changeGUITypeWorker.RunWorkerAsync();
            }
        }

        public static Point GetGameMapImagePixelCoordinate(int row, int column, bool big_gui)
        {
            SizeTile sizeTile = big_gui ? HexTypes.HexTypes.SizeBigTile : HexTypes.HexTypes.SizeSmallTile;

            // Calculation by trial and error of the formula to draw the tiles on map
            double nStartYMultiplier = sizeTile.Height * 0.25;
            double nStartXOddOffset = sizeTile.Width * 0.8;
            double nStartXMultiplier = 2 * nStartXOddOffset;
            double nStartY = column * nStartYMultiplier;
            double nStartX = (row * nStartXMultiplier);
            if (column % 2 != 0)
                nStartX += nStartXOddOffset;

            return new Point(nStartX, nStartY);
        }
    }
}
