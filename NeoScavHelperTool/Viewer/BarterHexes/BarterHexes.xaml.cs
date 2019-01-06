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
using static NeoScavHelperTool.Viewer.HexTypes.HexTypes;
using static NeoScavHelperTool.Viewer.Maps.Maps;

namespace NeoScavHelperTool.Viewer.BarterHexes
{
    /// <summary>
    /// Interaction logic for BarterHexes.xaml
    /// </summary>
    public partial class BarterHexes : UserControl, IChangeGUIType
    {
        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        public BarterHexes()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void BarterHexesControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();

                _loadItemsWorker.RunWorkerAsync();
            }
        }

        private void CreateUpdateCanvas()
        {
            _isOnBigGUI = MainWindow.I.IsBigGUISelected;

            int nBarterHexColumn = Convert.ToInt32(_arrayDBValues[(int)EDBBarterHexesTableColumns.eNX]);
            int nBarterHexRow = Convert.ToInt32(_arrayDBValues[(int)EDBBarterHexesTableColumns.eNY]);
            SizeMap sizeMap = Maps.Maps.SizeGameMap;
            BitmapSource mark = null;
            Point? markPosition = null;
            // Just a sanity check to see if the barter spot exists on map
            if (nBarterHexColumn <= sizeMap.Columns && nBarterHexRow <= sizeMap.Rows)
            {
                //HexHilight image will mark the spot
                mark = Images.Images.GetImageToDraw("HexHilight", "0_images", _isOnBigGUI, false);

                markPosition = Maps.Maps.GetGameMapImagePixelCoordinate(nBarterHexColumn, nBarterHexRow, _isOnBigGUI);
            }

            //Fetch the map with the barterhex marked on it
            DrawingImage finalMapWithBarterhexMarked = Maps.Maps.GetGameMapImageWithDrawnImageAtPoint(_isOnBigGUI, Maps.Maps.EDayTime.eDay, true, mark, markPosition);

            //We need this to try to center the scroll view on the barter hex
            SizeTile sizeTile = _isOnBigGUI ? HexTypes.HexTypes.SizeBigTile : HexTypes.HexTypes.SizeSmallTile;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ContainerBarterHexesCanvas.Source = finalMapWithBarterhexMarked;
                ContainerBarterHexesCanvas.Width = finalMapWithBarterhexMarked.Width;
                ContainerBarterHexesCanvas.Height = finalMapWithBarterhexMarked.Height;
                //I will do this here instead of on RunWorkerCompleted because I need the grid to reserve it's space on GUI
                if (DataGridBarterHexes.ItemsSource == null)
                {
                    DataGridBarterHexes.ItemsSource = _dataGridItems;
                    DataGridBarterHexes.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size visualSize = DataGridBarterHexes.DesiredSize;
                    DataGridBarterHexes.Arrange(new Rect(new Point(0, 0), visualSize));
                    DataGridBarterHexes.UpdateLayout();
                }
                // let's try to center the scroll view on the barter hex
                if (markPosition.HasValue)
                {
                    double scrollWidth = ContainerBarterHexesScroll.ViewportWidth;
                    double scrollHeight = ContainerBarterHexesScroll.ViewportHeight;

                    double offsetX = markPosition.Value.X + sizeTile.Width * 0.5 - scrollWidth * 0.5;
                    double offsetY = markPosition.Value.Y + sizeTile.Height * 0.5 - scrollHeight * 0.5;

                    ContainerBarterHexesScroll.ScrollToVerticalOffset(offsetY);
                    ContainerBarterHexesScroll.ScrollToHorizontalOffset(offsetX);
                }
            }));
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
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eBarterHexes)[_dataGridItems.Count], columnValue));
            }
            //3rd - Display the information on canvas
            CreateUpdateCanvas();
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update the GUI
            BarterHexesTitle.Content = string.Format("{0}__({1},{2})", _arrayDBValues[(int)EDBBarterHexesTableColumns.eId], _arrayDBValues[(int)EDBBarterHexesTableColumns.eNX], _arrayDBValues[(int)EDBBarterHexesTableColumns.eNY]);            
            BarterHexesMainGrid.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        private void ChangeGUIType_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateUpdateCanvas();
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

    }
}
