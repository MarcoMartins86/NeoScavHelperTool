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

namespace NeoScavHelperTool.Viewer.ForbiddenHexes
{
    /// <summary>
    /// Interaction logic for ForbiddenHexes.xaml
    /// </summary>
    public partial class ForbiddenHexes : UserControl, IChangeGUIType
    {
        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;

        public ForbiddenHexes()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void ForbiddenHexesControl_Loaded(object sender, RoutedEventArgs e)
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

            int nForbiddenHexColumn = Convert.ToInt32(_arrayDBValues[(int)EDBForbiddenHexesTableColumns.eNX]);
            int nForbiddenHexRow = Convert.ToInt32(_arrayDBValues[(int)EDBForbiddenHexesTableColumns.eNY]);
            SizeMap sizeMap = Maps.Maps.SizeGameMap;
            BitmapSource mark = null;
            Point? markPosition = null;
            // Just a sanity check to see if the forbidden spot exists on map
            if (nForbiddenHexColumn <= sizeMap.Columns && nForbiddenHexRow <= sizeMap.Rows)
            {
                //HexHilightInvalid image will mark the spot
                mark = Images.Images.GetImageToDraw("HexHilightInvalid", "0_images", _isOnBigGUI, false);

                markPosition = Maps.Maps.GetGameMapImagePixelCoordinate(nForbiddenHexColumn, nForbiddenHexRow, _isOnBigGUI);
            }

            //Fetch the map with the forbiddenhex marked on it
            DrawingImage finalMapWithForbiddenhexMarked = Maps.Maps.GetGameMapImageWithDrawnImageAtPoint(_isOnBigGUI, Maps.Maps.EDayTime.eDay, true, mark, markPosition);

            //We need this to try to center the scroll view on the forbidden hex
            SizeTile sizeTile = _isOnBigGUI ? HexTypes.HexTypes.SizeBigTile : HexTypes.HexTypes.SizeSmallTile;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ContainerForbiddenHexesCanvas.Source = finalMapWithForbiddenhexMarked;
                ContainerForbiddenHexesCanvas.Width = finalMapWithForbiddenhexMarked.Width;
                ContainerForbiddenHexesCanvas.Height = finalMapWithForbiddenhexMarked.Height;
                //I will do this here instead of on RunWorkerCompleted because I need the grid to reserve it's space on GUI
                if (DataGridForbiddenHexes.ItemsSource == null)
                {
                    DataGridForbiddenHexes.ItemsSource = _dataGridItems;
                    DataGridForbiddenHexes.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size visualSize = DataGridForbiddenHexes.DesiredSize;
                    DataGridForbiddenHexes.Arrange(new Rect(new Point(0, 0), visualSize));
                    DataGridForbiddenHexes.UpdateLayout();
                }
                // let's try to center the scroll view on the forbidden hex
                if (markPosition.HasValue)
                {
                    double scrollWidth = ContainerForbiddenHexesScroll.ViewportWidth;
                    double scrollHeight = ContainerForbiddenHexesScroll.ViewportHeight;

                    double offsetX = markPosition.Value.X + sizeTile.Width * 0.5 - scrollWidth * 0.5;
                    double offsetY = markPosition.Value.Y + sizeTile.Height * 0.5 - scrollHeight * 0.5;

                    ContainerForbiddenHexesScroll.ScrollToVerticalOffset(offsetY);
                    ContainerForbiddenHexesScroll.ScrollToHorizontalOffset(offsetX);
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
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eForbiddenHexes)[_dataGridItems.Count], columnValue));
            }
            //3rd - Display the information on canvas
            CreateUpdateCanvas();
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update the GUI
            ForbiddenHexesTitle.Content = string.Format("{0}__({1},{2})__{3}", _arrayDBValues[(int)EDBForbiddenHexesTableColumns.eId], _arrayDBValues[(int)EDBForbiddenHexesTableColumns.eNX], _arrayDBValues[(int)EDBForbiddenHexesTableColumns.eNY], _arrayDBValues[(int)EDBForbiddenHexesTableColumns.eStrName]);
            ForbiddenHexesMainGrid.Visibility = Visibility.Visible;
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
