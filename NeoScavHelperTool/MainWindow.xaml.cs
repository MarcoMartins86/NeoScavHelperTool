using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

namespace NeoScavModHelperTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static List<TreeViewItem> _listModsTreeItems = new List<TreeViewItem>();
        public static List<TreeViewItem> ListModsTreeItems => _listModsTreeItems;
        private static List<TreeViewItem> _listTypeTreeItems = new List<TreeViewItem>();
        public static List<TreeViewItem> ListTypeTreeItems => _listTypeTreeItems;
        private TreeViewItemLeaf _selectedItem;
        private List<UIElement> _listAttackmodesCanvasDynamicAmmoObjects = new List<UIElement>();


        public MainWindow() : base ()
        {
            InitializeComponent();

            ListModsTreeItems.ForEach(item => this.TreeViewViewerMods.Items.Add(item));
            ListTypeTreeItems.ForEach(item => this.TreeViewViewerTypes.Items.Add(item));
            _selectedItem = null;
        }   
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //only hack I could think of so that the window is not behind other explorer windows on launch
            this.Topmost = true;
            this.Topmost = false;
            this.TabControlDataType.SelectedIndex = -1; // we don't want to show anything on start
        }
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    flipview.BannerText = "Cupcakes!";
                    break;
                case 1:
                    flipview.BannerText = "Xbox!";
                    break;
                case 2:
                    flipview.BannerText = "Chess!";
                    break;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //we only do things when we are on a leaf object else we clear window
            if(e.NewValue.GetType() == typeof(TreeViewItemLeaf))
            {
                _selectedItem = (TreeViewItemLeaf)e.NewValue;
                switch(_selectedItem.Type)
                {
                    case TreeViewItemLeaf.DataType.eAttackmodes:
                        TabItemAttackmodes_Selected();
                        break;
                }
                this.TabControlDataType.SelectedIndex = (int)_selectedItem.Type;
            }
            else
            {
                this.TabControlDataType.SelectedIndex = -1;
                _selectedItem = null;
            }
        }        

        private void TabItemAttackmodes_Selected()
        {            
            if(_selectedItem != null)
            {
                //let's fill this tab with information
                if(this.ImageAttackmodesAModeFrame.Source == null) //we only need to do this once, design the frame on canvas
                    this.ImageAttackmodesAModeFrame.Source = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory("AModeFrame_1360x768", "0_images", false)));
                
                //Fetch this item data from DB
                List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);
                
                //Set the item name on the canvas
                this.TextBlockAttackmodesName.Text = "= " + listValues[1].ToString();

                //Fill a list with the data so it can be shown on the DataGrid
                List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
                for (int index = 0; index < listValues.Count; index++)
                    list.Add(new ViewerDataGridItem(DBOperations.ListAttackmodesColumnNames[index], listValues[index]));
                this.DataGridAttackmods.ItemsSource = list;

                //Now let's fetch the item image and display it
                string strImageName = listValues[12].ToString();
                //string[] strImageNameSplitted = strImageName.Split(':');
                string strImageTable = _selectedItem.TableName;
                App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strImageName, ref strImageTable, "images");
                strImageName = Path.GetFileNameWithoutExtension(strImageName);
                BitmapImage image = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory(strImageName, strImageTable, true)));
                //we need to mantain consistency in Dpi so we need to convert the image to the frame dpi
                this.ImageAttackmodesItem.Source = App.ConvertImageDpi(image,
                    ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiX,
                    ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiY);

                //Finally if item have a chargedprofile value it means that it will have ammo associated so let's draw it
                //Since ammo images are dynamic because it can use more than one we need to delete the older ones from canvas
                _listAttackmodesCanvasDynamicAmmoObjects.ForEach(objectRemove => this.CanvasAttackmodes.Children.Remove(objectRemove));
                _listAttackmodesCanvasDynamicAmmoObjects.Clear();

                string strChargeProfileIDList = listValues[6].ToString();
                if (string.IsNullOrEmpty(strChargeProfileIDList) == false)
                {
                    //We need to fetch the item id from chargeprofiles table
                    string strChargeProfileTable = _selectedItem.TableName;
                    //It seems that there could be more than one chargeprofile id so
                    foreach (string item in strChargeProfileIDList.Split(','))
                    {
                        string strChargeProfileID = item;
                        App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strChargeProfileID, ref strChargeProfileTable, "chargeprofiles");
                        string strItemID = App.DB.GetColumnValueFromMemoryTable("nID", strChargeProfileID, strChargeProfileTable, "strItemID");
                        //Now we need to find the item using the nGroupID and nSubgroupID from itemtypes table
                        string strItemTable = strChargeProfileTable;
                        App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strItemID, ref strItemTable, "itemtypes");
                        string strAmmoImageList = App.DB.GetColumnValueFromMemoryTableMultipleAndConditions(new List<string> { "nGroupID", "nSubgroupID" }, strItemID.Split('.').ToList(),
                            strItemTable, "vImageList");
                        //Finally fetch the ammo image
                        string strAmmoImageName = strAmmoImageList.Split(',')[0];
                        string strAmmoImageTable = strItemTable;
                        App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strAmmoImageName, ref strAmmoImageTable, "images");
                        strAmmoImageName = Path.GetFileNameWithoutExtension(strAmmoImageName);
                        BitmapImage ammoImage = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory(strAmmoImageName, strAmmoImageTable, true)));

                        //since there could be already some ammo drawed we need to fetch an aproximate size of the right margin
                        double nRigthMarginUsed = 0;
                        for(int i = 0; i < _listAttackmodesCanvasDynamicAmmoObjects.Count; i+=2)
                            nRigthMarginUsed += 30 + ((Image)_listAttackmodesCanvasDynamicAmmoObjects[i + 1]).Source.Width;

                        TextBlock textAmmo = new TextBlock();
                        textAmmo.Text = "x0";
                        textAmmo.FontSize = 20;
                        Canvas.SetRight(textAmmo, 5 + nRigthMarginUsed);
                        Canvas.SetBottom(textAmmo, 5);
                        Canvas.SetZIndex(textAmmo, 3);
                        this.CanvasAttackmodes.Children.Add(textAmmo);
                        _listAttackmodesCanvasDynamicAmmoObjects.Add(textAmmo);

                        Image ammoCanvasImage = new Image();
                        ammoCanvasImage.Source = App.ConvertImageDpi(ammoImage,
                        ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiX,
                        ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiY);
                        Canvas.SetRight(ammoCanvasImage, 30 + nRigthMarginUsed);
                        Canvas.SetBottom(ammoCanvasImage, 3);
                        Canvas.SetZIndex(ammoCanvasImage, 2);
                        this.CanvasAttackmodes.Children.Add(ammoCanvasImage);
                        _listAttackmodesCanvasDynamicAmmoObjects.Add(ammoCanvasImage);
                    }
                }
            }
        }
    }
}
