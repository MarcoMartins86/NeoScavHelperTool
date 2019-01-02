using MahApps.Metro.Controls;
using NeoScavHelperTool;
using NeoScavHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace NeoScavHelperTool.Viewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : UserControl
    {
        private enum ETreeMode
        {
            eByMods = 1,
            eByTypes
        }
        public static Viewer I = null;

        private List<ViewerTreeItemDescriptor> _treeItemsList = null;
        private ObservableCollection<Tier1TreeNode> ByTypeTreeData = new ObservableCollection<Tier1TreeNode>();
        private ObservableCollection<Tier1TreeNode> ByModTreeData = new ObservableCollection<Tier1TreeNode>();

        private ViewerTreeItemDescriptor _selectedItem;
        public ViewerTreeItemDescriptor SelectedItem => _selectedItem;

        private readonly BackgroundWorker _loadTreeItemsWorker = new BackgroundWorker();

        private ETreeMode _eTreeMode = ETreeMode.eByTypes;

        public Viewer() : base()
        {
            I = this;

            InitializeComponent();

            _loadTreeItemsWorker.DoWork += LoadTreeItemsWoker_DoWork;
            _loadTreeItemsWorker.RunWorkerCompleted += LoadTreeItemsWoker_RunWorkerCompleted;
        }

        void ViewerControl_ContentRendered(object sender, EventArgs e)
        {   
            if (_treeItemsList == null && MainWindow.I != null)
            {
                // Don't forget to unsubscribe from the event
                ((PresentationSource)sender).ContentRendered -= ViewerControl_ContentRendered;

                MainWindow.I.StartWaitSpinner();

                _loadTreeItemsWorker.RunWorkerAsync();
            }
        }

        private void ViewerControl_Loaded(object sender, RoutedEventArgs e)
        {
            //// Get PresentationSource
            //PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);
            //// Subscribe to PresentationSource's ContentRendered event
            //presentationSource.ContentRendered += ViewerControl_ContentRendered;
            if (_treeItemsList == null && MainWindow.I != null)
            {
                MainWindow.I.StartWaitSpinner();

                _loadTreeItemsWorker.RunWorkerAsync();
            }
        }
        
        static private int GetTier1NodeIndex(ObservableCollection<Tier1TreeNode> collection, string name)
        {
            int nReturnIndex;
            for (nReturnIndex = 0; nReturnIndex < collection.Count; nReturnIndex++)
            {
                if (collection[nReturnIndex].HashCode == name.GetHashCode())
                    break;
            }
            if (nReturnIndex == collection.Count)
                collection.Add(new Tier1TreeNode(name));

            return nReturnIndex;
        }

        static private int GetTier2NodeIndex(ObservableCollection<Tier2TreeNode> collection, string name)
        {
            int nReturnIndex;
            for (nReturnIndex = 0; nReturnIndex < collection.Count; nReturnIndex++)
            {
                if (collection[nReturnIndex].HashCode == name.GetHashCode())
                    break;
            }
            if (nReturnIndex == collection.Count)
                collection.Add(new Tier2TreeNode(name));

            return nReturnIndex;
        }

        private void LoadTreeItemsWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            _treeItemsList = new List<ViewerTreeItemDescriptor>();

            for(int nModIndex = 0; nModIndex< App.I.ModsPrefix.Count; nModIndex++)
            {
                for (int nTableIndex = 0; nTableIndex < (int)EDBTable.eTotal; nTableIndex++)
                {
                    EDBTable eDBTable = (EDBTable)nTableIndex;

                    string strTableNamePrefix = App.I.ModsPrefix[nModIndex];
                    string strTableNameSufix = DBTableAttributtesFetcher.GetNameSufix(eDBTable);                    
                                       
                    string strTableName = string.Format("{0}_{1}", strTableNamePrefix, strTableNameSufix);
                    List<string> strColumnsList = DBTableAttributtesFetcher.GetTreeViewDecriptiveColumnsNames(eDBTable);

                    //Get items from mem DB
                    List<object[]> listColumnsValuesTable = App.DB.GetAllTableDataOfSpecificColumnsFromMemory(strTableName, strColumnsList);
                    if (listColumnsValuesTable.Count > 0)
                    {
                        //Now check if the collection already have the type
                        int nByTypeTypeIndex = GetTier1NodeIndex(ByTypeTreeData, strTableNameSufix);
                        //Now check if the Tier1 collection already have the mod
                        int nByTypeModIndex = GetTier2NodeIndex(ByTypeTreeData[nByTypeTypeIndex].Tier2, strTableNamePrefix);
                        //Now check if the collection already have the mod
                        int nByModModIndex = GetTier1NodeIndex(ByModTreeData, strTableNamePrefix);
                        //Now check if the Tier1 collection already have the type
                        int nByModTypeIndex = GetTier2NodeIndex(ByModTreeData[nByModModIndex].Tier2, strTableNameSufix);
                        //Now for each item we will create a new ViewerTreeItemDescriptor object and add it to the trees
                        listColumnsValuesTable.ForEach(item =>
                        {
                            int nByTypeItemIndex = ByTypeTreeData[nByTypeTypeIndex].Tier2[nByTypeModIndex].Items.Count;
                            int nByModItemIndex = ByModTreeData[nByModModIndex].Tier2[nByModTypeIndex].Items.Count;

                            int[] nTreeIndexByType = new int[3] { nByTypeTypeIndex, nByTypeModIndex, nByTypeItemIndex };
                            int[] nTreeIndexByMod = new int[3] { nByModModIndex, nByModTypeIndex, nByModItemIndex };
                            ViewerTreeItemDescriptor newItem = new ViewerTreeItemDescriptor(item, eDBTable, nModIndex, nTreeIndexByType, nTreeIndexByMod, strTableName);
                            _treeItemsList.Add(newItem);
                            ByTypeTreeData[nByTypeTypeIndex].Tier2[nByTypeModIndex].Items.Add(newItem);
                            ByModTreeData[nByModModIndex].Tier2[nByModTypeIndex].Items.Add(newItem);
                        });
                    }
                }
            }
        }

        private void LoadTreeItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeViewViewerTypes.ItemsSource = ByTypeTreeData;
            TreeViewViewerMods.ItemsSource = ByModTreeData;

            MainWindow.I.StopWaitSpinner();
        }

        private void DrawSelectedItem()
        {
            switch (_selectedItem.Type)
            {
                case EDBTable.eBarterHexes:
                    ViewerDataContainer.Content = new BarterHexes.BarterHexes(); 
                    break;
                case EDBTable.eForbiddenHexes:
                    ViewerDataContainer.Content = new ForbiddenHexes.ForbiddenHexes();
                    break;
                case EDBTable.eHexTypes:
                    ViewerDataContainer.Content = new HexTypes.HexTypes();
                    break;
                case EDBTable.eImages:
                    ViewerDataContainer.Content = new Images.Images();
                    break;
                case EDBTable.eMaps:
                    ViewerDataContainer.Content = new Maps.Maps();
                    break;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //Check if it is a leaf first and not an intermediary node
            if ( typeof(ViewerTreeItemDescriptor) == e.NewValue.GetType())
            {
                _selectedItem = (ViewerTreeItemDescriptor)e.NewValue;

                DrawSelectedItem();
            }
        }

        public void RestoreFocusSelectedItem()
        {
            switch(_eTreeMode)
            {
                case ETreeMode.eByMods:
                    TreeViewViewerMods.Focus();
                    break;
                case ETreeMode.eByTypes:
                    TreeViewViewerTypes.Focus();
                    break;
            }
        }

        private void TabControlViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _eTreeMode = (ETreeMode)TabControlViewer.SelectedIndex;

            // Check if we need to redraw the viewer data renderer
            ViewerTreeItemDescriptor selectedItem = null;
            switch (_eTreeMode)
            {
                case ETreeMode.eByMods:
                    selectedItem = (ViewerTreeItemDescriptor)TreeViewViewerMods.SelectedItem;
                    break;
                case ETreeMode.eByTypes:
                    selectedItem = (ViewerTreeItemDescriptor)TreeViewViewerTypes.SelectedItem;
                    break;
            }
            if(selectedItem != null && _selectedItem != selectedItem)
            {
                _selectedItem = selectedItem;
                DrawSelectedItem();
            }            
        }
    }
}
