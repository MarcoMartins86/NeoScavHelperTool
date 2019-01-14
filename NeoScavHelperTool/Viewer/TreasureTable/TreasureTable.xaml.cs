using MahApps.Metro.Controls;
using NeoScavHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace NeoScavHelperTool.Viewer.TreasureTable
{
    /// <summary>
    /// Interaction logic for TreasureTable.xaml
    /// </summary>
    public partial class TreasureTable : UserControl, IChangeGUIType
    {
        public struct STreasureTableElement
        {
            public enum EIDIndex
            {
                eGroupID,
                eSubgroupID
            }
            public enum EDataIndex
            {
                eID,
                eProbability,
                eOccurrence,
                eTotal
            }

            public BitmapSource ImageBig;
            public BitmapSource ImageSmall;
            public string DisplayInfo;

            public STreasureTableElement(string [] id, string [] entry, string table)
            {
                string TableNamePrefix = table.Split('_')[0];
                string GroupID = id[(int)STreasureTableElement.EIDIndex.eGroupID];
                string SubgroupID = id[(int)STreasureTableElement.EIDIndex.eSubgroupID];
                string Probability = entry[(int)STreasureTableElement.EDataIndex.eProbability];
                //TODO: when not present what will the occurence value defaults to in-game???
                if (entry.Length > (int)STreasureTableElement.EDataIndex.eOccurrence)
                {
                    string Occurrence = entry[(int)STreasureTableElement.EDataIndex.eOccurrence];
                    DisplayInfo = string.Format("Item {0}:{1}.{2}x{3}x{4}", TableNamePrefix, GroupID, SubgroupID, Probability, Occurrence);
                }
                else
                    DisplayInfo = string.Format("Item {0}:{1}.{2}x{3}", TableNamePrefix, GroupID, SubgroupID, Probability);

                ImageBig = ItemTypes.ItemTypes.GetItemDisplayImage(GroupID, SubgroupID, table, true);
                ImageSmall = ItemTypes.ItemTypes.GetItemDisplayImage(GroupID, SubgroupID, table, false);                
            }
        }
        public struct STreasureTable
        {
            public string DisplayInfo;
            public List<List<object>> Elements;
            public string TableName;
            public string ID;

            public STreasureTable(string id, string table, string probability, string occurrence)
            {
                string strTreasureTableSufix = DBTableAttributtesFetcher.GetNameSufix(EDBTable.eTreasureTable);
                ItemTypes.ItemTypes.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref id, ref table, strTreasureTableSufix);
                string strTablePrefix = table.Split('_')[0];
                DisplayInfo = string.Format("Table {0}:{1}x{2}x{3}", strTablePrefix, id, probability, occurrence);
                Elements = new List<List<object>>();
                TableName = table;
                ID = id;               
            }
        }
        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;
        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private STreasureTable _treasures;

        public TreasureTable()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void TreasureTableControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();
                _loadItemsWorker.RunWorkerAsync();
            }
        }

        public static void Parse(STreasureTable treasures_table)
        {            
            string strColumnSearchName = DBTableAttributtesFetcher.GetPrimaryKeyName(EDBTable.eTreasureTable);
            string strColumnRetriveName = DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eTreasureTable)[(int)EDBTreasureTableColumns.eATreasures];            
            string strItemTypesTableSufix = DBTableAttributtesFetcher.GetNameSufix(EDBTable.eItemTypes);            

            //Split the and entries first
            string strATreasures = App.DB.GetColumnValueFromMemoryTable(strColumnSearchName, treasures_table.ID, treasures_table.TableName, strColumnRetriveName);
            string [] strTableAndEntriesSplitted = strATreasures.Split(',');
            foreach(string strTableOrEntries in strTableAndEntriesSplitted)
            {
                List<object> treasureItems = new List<object>();

                //Split the or entries second
                string[] strTableOrEntriesSplitted = strTableOrEntries.Split('|');
                foreach(string strEntry in strTableOrEntriesSplitted)
                {                    
                    string[] strEntrySplitted = strEntry.Split('x');
                    //Translate the entry to the respective table
                    string strEntryID = strEntrySplitted[(int)STreasureTableElement.EDataIndex.eID];
                    string strEntryTable = treasures_table.TableName;
                    ItemTypes.ItemTypes.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strEntryID, ref strEntryTable, strItemTypesTableSufix);
                    //Now we need to check if it is a table or an item
                    string[] strEntryIDSplitted = strEntryID.Split('.');
                    //is an item in the format GroupID.SubgroupID
                    if(strEntryIDSplitted.Length == 2)
                    {
                        STreasureTableElement item = new STreasureTableElement(strEntryIDSplitted, strEntrySplitted, strEntryTable);
                        treasureItems.Add(item);
                    }
                    else if(string.IsNullOrEmpty(strEntryIDSplitted[0]) == false)
                    {
                        STreasureTable innerTresureTable = new STreasureTable(strEntryIDSplitted[0], strEntryTable, strEntrySplitted[(int)STreasureTableElement.EDataIndex.eProbability], strEntrySplitted[(int)STreasureTableElement.EDataIndex.eOccurrence]);
                        // Recursive parse tables, hopefully modders didn't create loop references in the treasure tables because I am not protecting them for now
                        Parse(innerTresureTable);
                        treasureItems.Add(innerTresureTable);
                    }
                }
                if(treasureItems.Count > 0)
                    treasures_table.Elements.Add(treasureItems);
            }
        }
        
        private FrameworkElement GetItemImageFromStructure(STreasureTableElement item, bool big_gui)
        {       
            BitmapSource source = big_gui ? item.ImageBig : item.ImageSmall;

            if (source != null)
            {
                //Let's create a new stack panel to aggregate the item image and description
                StackPanel stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(2);

                Image itemImage = new Image();
                itemImage.Source = source;
                itemImage.Width = source.Width;
                itemImage.Height = source.Height;

                TextBlock itemInfo = new TextBlock();
                itemInfo.Text = item.DisplayInfo;
                itemInfo.HorizontalAlignment = HorizontalAlignment.Center;

                stackPanel.Children.Add(itemImage);
                stackPanel.Children.Add(itemInfo);
                stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                stackPanel.VerticalAlignment = VerticalAlignment.Center;

                return stackPanel;
            }
            else
                return null;
        }

        private FrameworkElement RenderObject(object obj, bool big_gui)
        {
            FrameworkElement elementReturn = null;
            if(obj.GetType() == typeof(STreasureTableElement))
                elementReturn = GetItemImageFromStructure((STreasureTableElement)obj, big_gui);
            else if(obj.GetType() == typeof(STreasureTable))
                elementReturn = RenderTreasureTable((STreasureTable)obj, big_gui);

            return elementReturn;
        }

        private FrameworkElement RenderTreasureTable(STreasureTable table, bool big_gui)
        {
            //Create a group box to group all treasures from this table
            GroupBox groupBox = new GroupBox();
            groupBox.Header = table.DisplayInfo;
            groupBox.HorizontalAlignment = HorizontalAlignment.Center;
            groupBox.Style = new Style();
            //Create a wrap pannel to wrap items/tables 
            WrapPanel groupWrapPanel = new WrapPanel();
            groupWrapPanel.HorizontalAlignment = HorizontalAlignment.Center;
            groupWrapPanel.VerticalAlignment = VerticalAlignment.Center;
            groupBox.Content = groupWrapPanel;

            foreach (List<object> listObj in table.Elements)
            {
                //if there is only one object we will add it directly to the wrap pannel
                //otherwise we will add it to a flip view
                if (listObj.Count == 1)
                {
                    FrameworkElement itemRendered = RenderObject(listObj[0], big_gui);
                    if (itemRendered != null)
                        groupWrapPanel.Children.Add(itemRendered);
                }
                else
                {
                    //we will make a list with all the images/tables
                    List<FrameworkElement> items = new List<FrameworkElement>();
                    foreach (object obj in listObj)
                    {
                        FrameworkElement itemRendered = RenderObject(obj, big_gui);
                        if (obj != null)
                        {
                            items.Add(itemRendered);
                        }
                    }
                    //If the other items didn't exist we will add this as an and item
                    if (items.Count == 1)
                        groupWrapPanel.Children.Add(items[0]);
                    else if (items.Count > 1)
                    {
                        //Create a flip view to aggregate all items images
                        FlipView flipView = new FlipView();
                        flipView.IsBannerEnabled = false;
                        flipView.MouseHoverBorderEnabled = false;
                        flipView.CircularNavigation = true;
                        flipView.ItemsSource = items;
                        flipView.MinHeight = 0;
                        flipView.MinWidth = 0;
                        flipView.HorizontalContentAlignment = HorizontalAlignment.Center;
                        flipView.VerticalContentAlignment = VerticalAlignment.Center;
                        flipView.HorizontalAlignment = HorizontalAlignment.Center;
                        flipView.VerticalAlignment = VerticalAlignment.Center;
                        flipView.IsNavigationEnabled = false;

                        NumericUpDown numericUpDown = new NumericUpDown();
                        numericUpDown.Style = FindResource("CustomNumericUpDownStyle") as Style;
                        numericUpDown.DataContext = flipView;
                        numericUpDown.IsReadOnly = false;
                        numericUpDown.InterceptManualEnter = false;
                        numericUpDown.Interval = 1;
                        numericUpDown.Minimum = 1;
                        numericUpDown.HorizontalAlignment = HorizontalAlignment.Stretch;
                        numericUpDown.HorizontalContentAlignment = HorizontalAlignment.Center;
                        numericUpDown.VerticalAlignment = VerticalAlignment.Center;
                        numericUpDown.VerticalContentAlignment = VerticalAlignment.Center;
                        numericUpDown.PreviewKeyUp += NumericUpDown_PreviewKeyUp;
                        numericUpDown.PreviewKeyDown += NumericUpDown_PreviewKeyDown;
                        Binding maximumBinding = new Binding();
                        maximumBinding.Path = new PropertyPath("Items.Count");
                        maximumBinding.Mode = BindingMode.OneWay;
                        BindingOperations.SetBinding(numericUpDown, NumericUpDown.MaximumProperty, maximumBinding);
                        Binding valueBinding = new Binding();
                        valueBinding.Path = new PropertyPath("SelectedIndex");
                        valueBinding.Mode = BindingMode.TwoWay;
                        valueBinding.Converter = new Int32IndexToNumberConverter();
                        BindingOperations.SetBinding(numericUpDown, NumericUpDown.ValueProperty, valueBinding);

                        StackPanel stackPanel = new StackPanel();
                        stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                        stackPanel.VerticalAlignment = VerticalAlignment.Center;
                        stackPanel.Children.Add(flipView);
                        stackPanel.Children.Add(numericUpDown);

                        groupWrapPanel.Children.Add(stackPanel);
                    }                    
                }
            }

            return groupBox;
        }

        private void CreateUpdateTreasurePanel()
        {
            bool isOnBigGUI = MainWindow.I.IsBigGUISelected;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                //clear all UI elements to draw again
                TreasurePanel.Children.Clear();                
                TreasurePanel.Children.Add(RenderTreasureTable(_treasures, isOnBigGUI));
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
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eTreasureTable)[_dataGridItems.Count], columnValue));
            }
            //2nd - Parse the treasure table data to a list
            _treasures = new STreasureTable(selectedItem.PrimaryKeyValue, selectedItem.TableName, "1.0", "1");
            Parse(_treasures);
            //3rd - Display the information on canvas
            CreateUpdateTreasurePanel();
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update GUI
            Title.Text = string.Format("{0}-{1}", _arrayDBValues[(int)EDBTreasureTableColumns.eId], _arrayDBValues[(int)EDBTreasureTableColumns.eStrName]);
            DataGrid.ItemsSource = _dataGridItems;
            MainViewer.Visibility = Visibility.Visible;
            //Stop the loading spinner
            MainWindow.I.StopWaitSpinner();
        }

        private void Control_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(
                    e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Let's set the value column to the style that wraps the text
            DataGridTextColumn dgTextC = (DataGridTextColumn)e.Column;
            if (dgTextC != null && string.Equals("Value", dgTextC.Header))
                dgTextC.ElementStyle = this.FindResource("WrapText") as Style;
        }

        private void ChangeGUIType_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateUpdateTreasurePanel();
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

        private void NumericUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                Key keyEvent = Key.Escape;
                switch (e.Key)
                {                    
                    case Key.Right:
                        e.Handled = true;
                        keyEvent = Key.Up;
                        break;
                    case Key.Left:
                        e.Handled = true;
                        keyEvent = Key.Down;
                        break;
                }

                if (e.Handled)
                {
                    KeyEventArgs keyEventArgs = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, keyEvent);
                    keyEventArgs.RoutedEvent = e.RoutedEvent;
                    keyEventArgs.Source = e.Source;
                    keyEventArgs.Handled = false;
                    
                    ((NumericUpDown)sender).RaiseEvent(keyEventArgs);
                }
            }            
        }

        private void NumericUpDown_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                Key keyEvent = Key.Escape;
                switch (e.Key)
                {
                    case Key.Right:
                        e.Handled = true;
                        keyEvent = Key.Up;
                        break;
                    case Key.Left:
                        e.Handled = true;
                        keyEvent = Key.Down;
                        break;
                }

                if (e.Handled)
                {
                    KeyEventArgs keyEventArgs = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, keyEvent);
                    keyEventArgs.RoutedEvent = e.RoutedEvent;
                    keyEventArgs.Source = e.Source;
                    keyEventArgs.Handled = false;

                    ((NumericUpDown)sender).RaiseEvent(keyEventArgs);
                }
            }
        }
    }
}
