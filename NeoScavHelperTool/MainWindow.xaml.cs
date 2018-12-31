using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NeoScavHelperTool;
using NeoScavHelperTool.Viewer;
using NeoScavModHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeoScavModHelperTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow I = null;
        private WaitSpinner _waitSpinnerDialog = null;
        private bool _isBigGUISelected;
        public bool IsBigGUISelected => _isBigGUISelected;
        //private TreeViewItemLeaf _selectedItem;
        private List<UIElement> _listAttackmodesCanvasDynamicAmmoObjects = new List<UIElement>();
        private BitmapImage _imageGridUsedIngame;
        private BitmapImage _imageLogMessageFrame;
        private BitmapImage _imageBackgroundFrame;

        public MainWindow() : base()
        {
            I = this;
            InitializeComponent();

            //ListModsTreeItems.ForEach(item => this.TreeViewViewerMods.Items.Add(item));
            //ListTypeTreeItems.ForEach(item => this.TreeViewViewerTypes.Items.Add(item));
            //_selectedItem = null;
            ////Let's load the in game grid image (ssems there's only the small one 10x10 per square)
            //_imageGridUsedIngame = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory("GUIGrid", "0_images", false)));
            //_imageLogMessageFrame = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory("GUIMessageFrameBig_1360x768", "0_images", false)));
            //_imageBackgroundFrame = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory("GUIBG", "0_images", false)));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //only hack I could think of so that the window is not behind other explorer windows on launch
            this.Topmost = true;
            this.Topmost = false;

            // Now that the radio buttons are already initialized let's check big mode by default
            this.BigGUI.IsChecked = true;
        }

        #region Spinner dialog actions
        public void StartWaitSpinner()
        {
            if (_waitSpinnerDialog == null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _waitSpinnerDialog = new WaitSpinner();
                    _waitSpinnerDialog.Owner = this;
                    _waitSpinnerDialog.ShowDialog();
                }));
            }
        }

        public void StopWaitSpinner()
        {
            if (_waitSpinnerDialog != null)
            {
                _waitSpinnerDialog.Close();
                _waitSpinnerDialog = null;
            }
        }
        #endregion


        #region GUI Type radio buttons Mouse events handlers
        private void GUIRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // We will always use the small images on our radio buttons
            if (sender == this.SmallGUI)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_off", "0_images", false);
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_on", "0_images", false);
                this._isBigGUISelected = false;
            }
            else if (sender == this.BigGUI)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_on", "0_images", false);
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_off", "0_images", false);
                this._isBigGUISelected = true;
            }

            Viewer.I.RestoreFocusSelectedItem();//TODO do this for the correct selected  tab item (viewer, editor)
        }

        private void GUIRadioButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // We will always use the small images on our radio buttons
            if (sender == this.SmallGUI && this._isBigGUISelected == true)
            {
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_on", "0_images", false);
            }
            else if (sender == this.BigGUI && this._isBigGUISelected == false)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_on", "0_images", false);
            }
        }

        private void GUIRadioButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // We will always use the small images on our radio buttons
            if (sender == this.SmallGUI && this._isBigGUISelected == true)
            {
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_off", "0_images", false);
            }
            else if (sender == this.BigGUI && this._isBigGUISelected == false)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_off", "0_images", false);
            }
        }

        private void GUIRadioButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // We will always use the small images on our radio buttons
            if (sender == this.SmallGUI)
            {
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_dn", "0_images", false);
            }
            else if (sender == this.BigGUI)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_dn", "0_images", false);
            }
        }

        private void GUIRadioButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == this.SmallGUI)
            {
                this.SmallGUI.Content = App.DB.GetImagePathFromMemory("btn_800_on", "0_images", false);
            }
            else if (sender == this.BigGUI)
            {
                this.BigGUI.Content = App.DB.GetImagePathFromMemory("btn_1360_on", "0_images", false);
            }            
        }
        #endregion

        //private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var flipview = ((FlipView)sender);
        //    switch (flipview.SelectedIndex)
        //    {
        //        case 0:
        //            flipview.BannerText = "Cupcakes!";
        //            break;
        //        case 1:
        //            flipview.BannerText = "Xbox!";
        //            break;
        //        case 2:
        //            flipview.BannerText = "Chess!";
        //            break;
        //    }
        //}

        //private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    //we only do things when we are on a leaf object else we clear window
        //    if (e.NewValue.GetType() == typeof(TreeViewItemLeaf))
        //    {                
        //        _selectedItem = (TreeViewItemLeaf)e.NewValue;
        //        this.TabControlDataType.SelectedIndex = (int)_selectedItem.Type;
        //        switch (_selectedItem.Type)
        //        {
        //            case TreeViewItemLeaf.DataType.eAttackmodes:
        //                TabItemAttackmodes_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eBattlemoves:
        //                TabItemBattlemoves_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eChargeprofiles:
        //                TabItemChargeprofiles_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eConditions:
        //                TabItemConditions_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eContainertypes:
        //                TabItemContainertypes_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eCreatures:
        //                TabItemCreatures_Selected();
        //                break;
        //            case TreeViewItemLeaf.DataType.eCreaturesources:
        //                TabItemCreaturesources_Selected();
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        this.TabControlDataType.SelectedIndex = -1;
        //        _selectedItem = null;
        //    }
        //}
        //#region auxiliary methods


        //private void InsertConditionMessageFormatedIntoTextBlock(TextBlock text_block, string message, int color)
        //{
        //    text_block.Text = message.Replace("<us>", "Player");
        //    switch(color)
        //    {
        //        case 0:
        //            text_block.Foreground = new SolidColorBrush(Colors.White);
        //            break;
        //        case 1:
        //            text_block.Foreground = new SolidColorBrush(Colors.Red);
        //            break;
        //        case 2:
        //            text_block.Foreground = new SolidColorBrush(Colors.Lime);
        //            break;
        //        case 3:
        //            text_block.Foreground = new SolidColorBrush(Colors.Gold);
        //            break;
        //    }
        //}

        //private Dictionary<int, List<ComboBoxEquipmentItem>> GetItemNamesAndSpritesFromTreasure(string treasure_id, string treasure_table)
        //{
        //    Dictionary<int, List<ComboBoxEquipmentItem>> dicReturn = new Dictionary<int, List<ComboBoxEquipmentItem>>();
        //    //first lets fetch the treasue items information
        //    string strTreasureID = treasure_id;
        //    string strTreasureTable = treasure_table;
        //    App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strTreasureID, ref strTreasureTable, "treasuretable");
        //    string strListTreasures = App.DB.GetColumnValueFromMemoryTable("id", strTreasureID, strTreasureTable, "aTreasures");
        //    //Now split the items
        //    string[] strListTreasuresSplitted = strListTreasures.Split(new char[] { ',', '|' });
        //    //Add only unique items to this list
        //    List<string> listUniqueIDs = new List<string>();
        //    foreach (string strItem in strListTreasuresSplitted)
        //    {
        //        string strID = strItem.Split('x')[0];
        //        if (listUniqueIDs.Contains(strID) == false)
        //            listUniqueIDs.Add(strID);
        //    }

        //    foreach (string strItem in listUniqueIDs)
        //    {
        //        string strID = strItem;
        //        string strTable = strTreasureTable;

        //        string[] strIDSplitted = strID.Split('.');
        //        if (strIDSplitted.Length == 1)//this means that this id is really an item from treasuretable
        //        {
        //            //we call this method recursively
        //            Dictionary<int, List<ComboBoxEquipmentItem>> itemsNameSprites = GetItemNamesAndSpritesFromTreasure(strIDSplitted[0], strTable);
        //            //now we need to merge the results
        //            foreach (KeyValuePair<int, List<ComboBoxEquipmentItem>> itemPerBodyPart in itemsNameSprites)
        //            {
        //                //now we need to check if we already have this key or not
        //                if (dicReturn.ContainsKey(itemPerBodyPart.Key) == false)
        //                    dicReturn.Add(itemPerBodyPart.Key, new List<ComboBoxEquipmentItem>()); //let's add it

        //                //now let's merge only the unique list values
        //                itemPerBodyPart.Value.ForEach(item =>
        //                {
        //                    if (dicReturn[itemPerBodyPart.Key].Contains(item) == false)
        //                        dicReturn[itemPerBodyPart.Key].Add(item);
        //                });
        //            }
        //        }
        //        else
        //        {
        //            //fetch the item name and sprite from the itemtype table
        //            App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strID, ref strTable, "itemtypes");
        //            List<object> listNameSprites = App.DB.GetColumnsValuesFromMemoryTableMultipleAndConditions(new List<string> { "nGroupID", "nSubgroupID" },
        //                strID.Split('.').ToList(), strTable, new List<string> { "strDesc", "vSpriteList", "vEquipSlots", "strDescAlt" });

        //            if ((listNameSprites != null) && //this is for the case where there are some bad references on items 
        //                string.IsNullOrEmpty(listNameSprites[1].ToString()) == false) //only continue if there are some results on the sprite list
        //            {
        //                //If there is an alternate description for the item we will use that
        //                string strItemName = string.IsNullOrEmpty(listNameSprites[3].ToString()) ? listNameSprites[0].ToString() : listNameSprites[3].ToString();

        //                //Now we need to check the equip slots since sometimes there are more sprites than equip slots
        //                List<int> listEquipslotsUsedBodyParts = new List<int>();
        //                string[] equipSlotsSplitted = listNameSprites[2].ToString().Split(',');
        //                foreach (string equipSlot in equipSlotsSplitted)
        //                    listEquipslotsUsedBodyParts.Add(Convert.ToInt32(equipSlot.Split('=')[0]));

        //                //Since the item can be equiped in multiple slots lets split the sprites and add to on item to each combobox list
        //                string[] spritesSplitted = listNameSprites[1].ToString().Split(',');
        //                foreach (string sprite in spritesSplitted)
        //                {
        //                    string[] spriteSplitted = sprite.Split('=');
        //                    //get the body part id
        //                    int nBodyPartID = Convert.ToInt32(spriteSplitted[0]);
        //                    //continue only if this body part is in the equipable slots
        //                    if (listEquipslotsUsedBodyParts.Contains(nBodyPartID))
        //                    {
        //                        //Create the new item
        //                        ComboBoxEquipmentItem newItem = new ComboBoxEquipmentItem(strItemName, spriteSplitted[1], strTable, nBodyPartID);
        //                        //check if it is already on teh dictionary
        //                        if (dicReturn.ContainsKey(nBodyPartID) == false)
        //                            dicReturn.Add(nBodyPartID, new List<ComboBoxEquipmentItem>());
        //                        //add only unique items
        //                        if (dicReturn[nBodyPartID].Contains(newItem) == false)
        //                            dicReturn[nBodyPartID].Add(newItem);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //TODO: items that are not from body part or doesn't have any sprites
        //                //or list is null
        //            }
        //        }
        //    }

        //    return dicReturn;
        //}
        //#endregion

        //private void TabItemAttackmodes_Selected()
        //{
        //    if (_selectedItem != null)
        //    {
        //        //let's fill this tab with information
        //        if (this.ImageAttackmodesAModeFrame.Source == null) //we only need to do this once, design the frame on canvas
        //            this.ImageAttackmodesAModeFrame.Source = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory("AModeFrame_1360x768", "0_images", false)));

        //        //Fetch this item data from DB
        //        List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //        //Set the item name on the canvas
        //        this.TextBlockAttackmodesName.Text = "= " + listValues[1].ToString();

        //        //Fill a list with the data so it can be shown on the DataGrid
        //        List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //        listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eAttackModes)[list.Count], value)));
        //        this.DataGridAttackmods.ItemsSource = list;

        //        //Now let's fetch the item image and display it
        //        string strImageName = listValues[12].ToString();
        //        string strImageTable = _selectedItem.TableName;
        //        App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strImageName, ref strImageTable, "images");
        //        strImageName = Path.GetFileNameWithoutExtension(strImageName);
        //        BitmapImage image = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory(strImageName, strImageTable, true)));
        //        //we need to mantain consistency in Dpi so we need to convert the image to the frame dpi
        //        this.ImageAttackmodesItem.Source = App.ConvertImageDpi(image,
        //            ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiX,
        //            ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiY);

        //        //Finally if item have a chargedprofile value it means that it will have ammo associated so let's draw it
        //        //Since ammo images are dynamic because it can use more than one we need to delete the older ones from canvas
        //        _listAttackmodesCanvasDynamicAmmoObjects.ForEach(objectRemove => this.CanvasAttackmodes.Children.Remove(objectRemove));
        //        _listAttackmodesCanvasDynamicAmmoObjects.Clear();

        //        string strChargeProfileIDList = listValues[6].ToString();
        //        if (string.IsNullOrEmpty(strChargeProfileIDList) == false)
        //        {
        //            //We need to fetch the item id from chargeprofiles table
        //            string strChargeProfileTable = _selectedItem.TableName;
        //            //It seems that there could be more than one chargeprofile id so
        //            foreach (string item in strChargeProfileIDList.Split(','))
        //            {
        //                string strChargeProfileID = item;
        //                App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strChargeProfileID, ref strChargeProfileTable, "chargeprofiles");
        //                string strItemID = App.DB.GetColumnValueFromMemoryTable("nID", strChargeProfileID, strChargeProfileTable, "strItemID");
        //                //since there could be already some ammo drawed we need to fetch an aproximate size of the right margin
        //                double nRigthMarginUsed = 0;
        //                for (int i = 0; i < _listAttackmodesCanvasDynamicAmmoObjects.Count; i += 2)
        //                    nRigthMarginUsed += 30 + ((Image)_listAttackmodesCanvasDynamicAmmoObjects[i + 1]).Source.Width;

        //                //this is just to be similar to game
        //                TextBlock textAmmo = new TextBlock();
        //                textAmmo.Text = "x0";
        //                textAmmo.FontSize = 20;
        //                Canvas.SetRight(textAmmo, 5 + nRigthMarginUsed);
        //                Canvas.SetBottom(textAmmo, 5);
        //                Canvas.SetZIndex(textAmmo, 3);
        //                this.CanvasAttackmodes.Children.Add(textAmmo);
        //                _listAttackmodesCanvasDynamicAmmoObjects.Add(textAmmo);
        //                //now fetch the image and convert it to the frame dpi and draw it
        //                BitmapImage ammoImage = GetItemDisplayImage(strItemID, strChargeProfileTable, true);
        //                Image ammoCanvasImage = new Image();
        //                ammoCanvasImage.Source = App.ConvertImageDpi(ammoImage,
        //                ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiX,
        //                ((BitmapImage)this.ImageAttackmodesAModeFrame.Source).DpiY);
        //                Canvas.SetRight(ammoCanvasImage, 30 + nRigthMarginUsed);
        //                Canvas.SetBottom(ammoCanvasImage, 3);
        //                Canvas.SetZIndex(ammoCanvasImage, 2);
        //                this.CanvasAttackmodes.Children.Add(ammoCanvasImage);
        //                _listAttackmodesCanvasDynamicAmmoObjects.Add(ammoCanvasImage);
        //            }
        //        }
        //    }
        //}

        //private void TabItemBattlemoves_Selected()
        //{
        //    if (_selectedItem != null)
        //    {
        //        if (this.ImageBattlemovesGridFrame.Source == null) //we only need to do this once, design the frame on canvas
        //        {
        //            //we need to copy only part of the in game grid
        //            //icons are 3x3 which means 60x60 on big mode or 30x30 on small mode
        //            //we throw extra 1 cell
        //            BitmapSource smallGrid = App.CopyImageRectWithDpi(_imageGridUsedIngame, new Int32Rect(0, 0, 40, 40), _imageGridUsedIngame.DpiX, _imageGridUsedIngame.DpiY);
        //            TransformedBitmap bigGrid = new TransformedBitmap(smallGrid, new ScaleTransform(2, 2));
        //            this.ImageBattlemovesGridFrame.Source = bigGrid;
        //        }

        //        //Fetch this item data from DB
        //        List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //        //Fill a list with the data so it can be shown on the DataGrid
        //        List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //        listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eBattleMoves)[list.Count], value)));
        //        this.DataGridBattlemoves.ItemsSource = list;

        //        //Now let's fetch the item image
        //        BitmapImage image = GetItemDisplayImage(listValues[1].ToString(), _selectedItem.TableName, true);

        //        this.ImageBattlemovesItem.ToolTip = listValues[6];
        //        //now convert the image to the frame Dpi and draw it
        //        this.ImageBattlemovesItem.Source = App.ConvertImageDpi(image,
        //            ((TransformedBitmap)this.ImageBattlemovesGridFrame.Source).DpiX,
        //            ((TransformedBitmap)this.ImageBattlemovesGridFrame.Source).DpiY);
        //    }
        //}        

        //private void TabItemChargeprofiles_Selected()
        //{
        //    if (_selectedItem != null)
        //    {
        //        //Fetch this item data from DB
        //        List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //        //Fill a list with the data so it can be shown on the DataGrid
        //        List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //        listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eChargeProfiles)[list.Count], value)));
        //        this.DataGridChargeprofiles.ItemsSource = list;
        //        //Fetch the item image
        //        BitmapImage ammoImage = GetItemDisplayImage(listValues[2].ToString(), _selectedItem.TableName, true);               
        //        //Draw the frame on canvas with the needed number of cells +1       
        //        BitmapSource smallGrid = App.CopyImageRectWithDpi(_imageGridUsedIngame, new Int32Rect(0, 0, Convert.ToInt32(ammoImage.PixelWidth*0.5)+10,
        //            Convert.ToInt32(ammoImage.PixelHeight*0.5)+10), _imageGridUsedIngame.DpiX, _imageGridUsedIngame.DpiY);
        //        TransformedBitmap bigGrid = new TransformedBitmap(smallGrid, new ScaleTransform(2, 2));
        //        this.ImageChargeprofilesGridFrame.Source = bigGrid;
        //        //now convert the image to the frame Dpi and draw it
        //        this.ImageChargeprofilesItem.Source = App.ConvertImageDpi(ammoImage,
        //               ((TransformedBitmap)this.ImageChargeprofilesGridFrame.Source).DpiX,
        //               ((TransformedBitmap)this.ImageChargeprofilesGridFrame.Source).DpiY); 
        //    }
        //}        

        //private void TabItemConditions_Selected()
        //{
        //    //Fetch this item data from DB
        //    List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //    //Fill a list with the data so it can be shown on the DataGrid
        //    List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //    listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eConditions)[list.Count], value)));
        //    this.DataGridConditions.ItemsSource = list;
        //    //Only bDisplay controls if messages will appear or not
        //    bool bDisplay = Convert.ToBoolean(listValues[12]);

        //    double nUsedHeight = 0;
        //    double nUsedWidth = 0;
        //    if (!bDisplay) // hide the canvas since we don't want to show the messages
        //        this.CanvasConditions.Visibility = Visibility.Hidden;
        //    else
        //    {
        //        this.CanvasConditions.Visibility = Visibility.Visible;
        //        //food and water doesn't show up status message
        //        bool bDisplayStatusMessage = bDisplay && !string.IsNullOrEmpty(listValues[1].ToString()) &&
        //            !listValues[3].ToString().Contains("fFoodDebt") && !listValues[3].ToString().Contains("fWaterDebt");
        //        bool bDisplayLogMessage = bDisplay && !string.IsNullOrEmpty(listValues[2].ToString());

        //        if (bDisplayStatusMessage)
        //        {
        //            //show the status message
        //            this.LabelConditionsStatusMessage.Visibility = Visibility.Visible;
        //            this.ImageConditionsStatusMessage.Visibility = Visibility.Visible;
        //            this.TextBlockConditionsStatusMessage.Visibility = Visibility.Visible;

        //            //Set the status message label position
        //            Canvas.SetTop(this.LabelConditionsStatusMessage, nUsedHeight);
        //            if (this.LabelConditionsStatusMessage.IsMeasureValid == false) //if measure is not valid let's force it
        //                this.LabelConditionsStatusMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            nUsedHeight += Convert.ToInt32(this.LabelConditionsStatusMessage.DesiredSize.Height);
        //            //Set the status message image background position
        //            Canvas.SetTop(this.ImageConditionsStatusMessage, nUsedHeight);
        //            //Insert the status message text into the canvas
        //            InsertConditionMessageFormatedIntoTextBlock(this.TextBlockConditionsStatusMessage,
        //                listValues[1].ToString(), Convert.ToInt32(listValues[15]));
        //            //Set the status message text position
        //            Canvas.SetTop(this.TextBlockConditionsStatusMessage, nUsedHeight);
        //            //Set the status message text max width so it wraps around (need to convert 300 pixels from image to device independent units)
        //            this.TextBlockConditionsStatusMessage.MaxWidth = (300 * (_imageBackgroundFrame.DpiX/96.0));
        //            //we need to do this always since it is dynamic
        //            this.TextBlockConditionsStatusMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            //Load the status message background image
        //            this.ImageConditionsStatusMessage.Source = App.CopyImageRectWithDpi(_imageBackgroundFrame,
        //            new Int32Rect(_imageBackgroundFrame.PixelWidth - 300, 10, 300,
        //            5 + Convert.ToInt32(this.TextBlockConditionsStatusMessage.DesiredSize.Height * 96.0 / _imageBackgroundFrame.DpiY)),
        //            _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiY);
        //            //we need to do this always since it is dynamic
        //            this.ImageConditionsStatusMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            nUsedWidth = Convert.ToInt32(this.ImageConditionsStatusMessage.DesiredSize.Width);
        //            nUsedHeight += Convert.ToInt32(this.ImageConditionsStatusMessage.DesiredSize.Height);
        //        }
        //        else
        //        {                    
        //            //hide the status message
        //            this.LabelConditionsStatusMessage.Visibility = Visibility.Hidden;
        //            this.ImageConditionsStatusMessage.Visibility = Visibility.Hidden;
        //            this.TextBlockConditionsStatusMessage.Visibility = Visibility.Hidden;
        //        } 

        //        if(bDisplayLogMessage)
        //        {
        //            //show the log message
        //            this.LabelConditionsLogMessage.Visibility = Visibility.Visible;
        //            this.ImageConditionsLogMessage.Visibility = Visibility.Visible;
        //            this.TextBlockConditionsLogMessage.Visibility = Visibility.Visible;

        //            //Set the log message label position
        //            Canvas.SetTop(this.LabelConditionsLogMessage, nUsedHeight);
        //            if (this.LabelConditionsLogMessage.IsMeasureValid == false) //if measure is not valid let's force it
        //                this.LabelConditionsLogMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            nUsedHeight += Convert.ToInt32(this.LabelConditionsLogMessage.DesiredSize.Height);
        //            //Set the log message image background position
        //            Canvas.SetTop(this.ImageConditionsLogMessage, nUsedHeight);
        //            //Insert the log message text into the canvas
        //            InsertConditionMessageFormatedIntoTextBlock(this.TextBlockConditionsLogMessage,
        //                listValues[2].ToString(), Convert.ToInt32(listValues[15]));
        //            //Set the log message text position
        //            Canvas.SetTop(this.TextBlockConditionsLogMessage, nUsedHeight);
        //            //Set the log message text max width so it wraps around (need to convert 300 pixels from image to device independent units)
        //            this.TextBlockConditionsLogMessage.MaxWidth = _imageLogMessageFrame.Width;
        //            //we need to do this always since it is dynamic
        //            this.TextBlockConditionsLogMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            //Load the log message background image
        //            this.ImageConditionsLogMessage.Source = App.CopyImageRectWithDpi(_imageLogMessageFrame,
        //            new Int32Rect(0, 0, _imageLogMessageFrame.PixelWidth, 
        //            5 + Convert.ToInt32(this.TextBlockConditionsLogMessage.DesiredSize.Height * 96.0/_imageLogMessageFrame.DpiY)),
        //            _imageLogMessageFrame.DpiX, _imageLogMessageFrame.DpiY);
        //            //we need to do this always since it is dynamic
        //            this.ImageConditionsLogMessage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //            nUsedWidth = Convert.ToInt32(this.ImageConditionsLogMessage.DesiredSize.Width); //this will always be bigger
        //            nUsedHeight += Convert.ToInt32(this.ImageConditionsLogMessage.DesiredSize.Height);
        //        }
        //        else
        //        {
        //            //hide the log message
        //            this.LabelConditionsLogMessage.Visibility = Visibility.Hidden;
        //            this.ImageConditionsLogMessage.Visibility = Visibility.Hidden;
        //            this.TextBlockConditionsLogMessage.Visibility = Visibility.Hidden;
        //        }
        //    }
        //    //Set the canvas height to the displayed height or 0 if not displayed
        //    this.CanvasConditions.Height = nUsedHeight;
        //    this.CanvasConditions.Width = nUsedWidth;
        //}

        //private void TabItemContainertypes_Selected()
        //{
        //    //Fetch this item data from DB
        //    List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //    //Fill a list with the data so it can be shown on the DataGrid
        //    List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //    listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eContainerTypes)[list.Count], value)));
        //    this.DataGridContainertypes.ItemsSource = list;
        //}



        //private void TabItemCreatures_Selected()
        //{
        //    /* if(this.ImageCreaturesBackground.Source == null)
        //     {
        //         this.ImageCreaturesBackground.Source = new SolidColorBrush(Colors.White);
        //         this.ImageCreaturesBackground.Source = App.CopyImageRectWithDpi(_imageBackgroundFrame,
        //             new Int32Rect(_imageBackgroundFrame.PixelWidth - 300, 10, 210, 164),
        //             _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiY);
        //         //this.DataGridCreaturesEquipment.ItemsSource = _listCreaturesEquipment;
        //     }
        //     */

        //    //we need to clean the combo box every time we choose an item
        //    this.ComboBoxCreaturesHead.SelectedIndex = -1;
        //    this.ComboBoxCreaturesNeck.SelectedIndex = -1;
        //    this.ComboBoxCreaturesShoulderLeft.SelectedIndex = -1;
        //    this.ComboBoxCreaturesShoulderRight.SelectedIndex = -1;
        //    this.ComboBoxCreaturesTorso.SelectedIndex = -1;
        //    this.ComboBoxCreaturesBackpack.SelectedIndex = -1;
        //    this.ComboBoxCreaturesWristLeft.SelectedIndex = -1;
        //    this.ComboBoxCreaturesWristRight.SelectedIndex = -1;
        //    this.ComboBoxCreaturesHandLeft.SelectedIndex = -1;
        //    this.ComboBoxCreaturesHandRight.SelectedIndex = -1;
        //    this.ComboBoxCreaturesBelt.SelectedIndex = -1;
        //    this.ComboBoxCreaturesLegs.SelectedIndex = -1;
        //    this.ComboBoxCreaturesFootLeft.SelectedIndex = -1;
        //    this.ComboBoxCreaturesFootRight.SelectedIndex = -1;

        //    //Fetch this item data from DB
        //    List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //    //Fill a list with the data so it can be shown on the DataGrid
        //    List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //    listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eCreatures)[list.Count], value)));
        //    this.DataGridCreatures.ItemsSource = list;
        //    //Set the creature main image
        //    string strImageName = listValues[4].ToString();
        //    string strImageTable = _selectedItem.TableName;
        //    App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strImageName, ref strImageTable, "images");
        //    strImageName = Path.GetFileNameWithoutExtension(strImageName);
        //    BitmapImage creature = new BitmapImage(new Uri(App.DB.GetImagePathFromMemory(strImageName, strImageTable, false)));
        //    this.ImageCreature.Source = new TransformedBitmap(App.ConvertImageDpi(creature, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //    //Now we need to fetch the treasure to equip the creature if it is human
        //    if (!listValues[10].ToString().Contains("577") &&
        //        !listValues[10].ToString().Contains("0:577")) //577 is th no sprites condition
        //    {  
        //        Random rand = new Random();
        //        Dictionary<int, List<ComboBoxEquipmentItem>> itemsNamesSprites = GetItemNamesAndSpritesFromTreasure(listValues[7].ToString(), _selectedItem.TableName);
        //        if (itemsNamesSprites.Count > 0)
        //        {
        //            this.StackPanelCreaturesEquipment.Visibility = Visibility.Visible;
        //            //Now we need to check for the parts that exist
        //            if (itemsNamesSprites.ContainsKey(17)) //Head
        //            {
        //                this.RowDefinitionCreaturesHead.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                itemsNamesSprites[17].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                this.ComboBoxCreaturesHead.ItemsSource = itemsNamesSprites[17];
        //                this.ComboBoxCreaturesHead.SelectedIndex = rand.Next(itemsNamesSprites[17].Count);
        //            }
        //            else
        //                this.RowDefinitionCreaturesHead.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(23)) //Neck
        //            {
        //                this.RowDefinitionCreaturesNeck.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                itemsNamesSprites[23].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                this.ComboBoxCreaturesNeck.ItemsSource = itemsNamesSprites[23];
        //                this.ComboBoxCreaturesNeck.SelectedIndex = rand.Next(itemsNamesSprites[23].Count);
        //            }
        //            else
        //                this.RowDefinitionCreaturesNeck.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(13) || itemsNamesSprites.ContainsKey(14)) //Shoulder left, shoulder right
        //            {
        //                this.RowDefinitionCreaturesShoulder.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                if (itemsNamesSprites.ContainsKey(13))
        //                {
        //                    itemsNamesSprites[13].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesShoulderLeft.ItemsSource = itemsNamesSprites[13];
        //                    this.ComboBoxCreaturesShoulderLeft.SelectedIndex = rand.Next(itemsNamesSprites[13].Count);
        //                }
        //                if (itemsNamesSprites.ContainsKey(14))
        //                {
        //                    itemsNamesSprites[14].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesShoulderRight.ItemsSource = itemsNamesSprites[14];
        //                    this.ComboBoxCreaturesShoulderRight.SelectedIndex = rand.Next(itemsNamesSprites[14].Count);
        //                }
        //            }
        //            else
        //                this.RowDefinitionCreaturesShoulder.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(11) || itemsNamesSprites.ContainsKey(22)) //Torso, Backpack
        //            {
        //                this.RowDefinitionCreaturesTorsoBackpack.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                if (itemsNamesSprites.ContainsKey(11))
        //                {
        //                    this.ComboBoxCreaturesTorso.IsEnabled = true; //enable the combo
        //                    itemsNamesSprites[11].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesTorso.ItemsSource = itemsNamesSprites[11];
        //                    this.ComboBoxCreaturesTorso.SelectedIndex = rand.Next(itemsNamesSprites[11].Count);
        //                }
        //                else
        //                    this.ComboBoxCreaturesTorso.IsEnabled = false; //disable the combo

        //                if (itemsNamesSprites.ContainsKey(22))
        //                {
        //                    this.ComboBoxCreaturesBackpack.IsEnabled = true; //enable the combo
        //                    itemsNamesSprites[22].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesBackpack.ItemsSource = itemsNamesSprites[22];
        //                    this.ComboBoxCreaturesBackpack.SelectedIndex = rand.Next(itemsNamesSprites[22].Count);
        //                }
        //                else
        //                    this.ComboBoxCreaturesBackpack.IsEnabled = false; //disable the combo
        //            }
        //            else
        //                this.RowDefinitionCreaturesTorsoBackpack.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(7) || itemsNamesSprites.ContainsKey(8)) //Wrist left, Wrist right
        //            {
        //                this.RowDefinitionCreaturesWrist.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                if (itemsNamesSprites.ContainsKey(7))
        //                {
        //                    itemsNamesSprites[7].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesWristLeft.ItemsSource = itemsNamesSprites[7];
        //                    this.ComboBoxCreaturesWristLeft.SelectedIndex = rand.Next(itemsNamesSprites[7].Count);
        //                }
        //                if (itemsNamesSprites.ContainsKey(8))
        //                {
        //                    itemsNamesSprites[8].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesWristRight.ItemsSource = itemsNamesSprites[8];
        //                    this.ComboBoxCreaturesWristRight.SelectedIndex = rand.Next(itemsNamesSprites[8].Count);
        //                }
        //            }
        //            else
        //                this.RowDefinitionCreaturesWrist.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(20) || itemsNamesSprites.ContainsKey(21)) //Hand left, Hand right
        //            {
        //                this.RowDefinitionCreaturesHand.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                if (itemsNamesSprites.ContainsKey(20))
        //                {
        //                    itemsNamesSprites[20].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesHandLeft.ItemsSource = itemsNamesSprites[20];
        //                    this.ComboBoxCreaturesHandLeft.SelectedIndex = rand.Next(itemsNamesSprites[20].Count);
        //                }
        //                if (itemsNamesSprites.ContainsKey(21))
        //                {
        //                    itemsNamesSprites[21].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesHandRight.ItemsSource = itemsNamesSprites[21];
        //                    this.ComboBoxCreaturesHandRight.SelectedIndex = rand.Next(itemsNamesSprites[21].Count);
        //                }
        //            }
        //            else
        //                this.RowDefinitionCreaturesHand.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(12)) //Belt
        //            {
        //                this.RowDefinitionCreaturesBelt.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                itemsNamesSprites[12].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                this.ComboBoxCreaturesBelt.ItemsSource = itemsNamesSprites[12];
        //                this.ComboBoxCreaturesBelt.SelectedIndex = rand.Next(itemsNamesSprites[12].Count);
        //            }
        //            else
        //                this.RowDefinitionCreaturesBelt.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(4)) //Legs
        //            {
        //                this.RowDefinitionCreaturesLegs.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                itemsNamesSprites[4].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                this.ComboBoxCreaturesLegs.ItemsSource = itemsNamesSprites[4];
        //                this.ComboBoxCreaturesLegs.SelectedIndex = rand.Next(itemsNamesSprites[4].Count);
        //            }
        //            else
        //                this.RowDefinitionCreaturesLegs.Height = new GridLength(0);// hide the row

        //            if (itemsNamesSprites.ContainsKey(2) || itemsNamesSprites.ContainsKey(3)) //Foot left, Foot right
        //            {
        //                this.RowDefinitionCreaturesFoot.Height = new GridLength(1, GridUnitType.Star); //show the row
        //                if (itemsNamesSprites.ContainsKey(2))
        //                {
        //                    itemsNamesSprites[2].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesFootLeft.ItemsSource = itemsNamesSprites[2];
        //                    this.ComboBoxCreaturesFootLeft.SelectedIndex = rand.Next(itemsNamesSprites[2].Count);
        //                }
        //                if (itemsNamesSprites.ContainsKey(3))
        //                {
        //                    itemsNamesSprites[3].Sort((a, b) => string.Compare(a.Name, b.Name));
        //                    this.ComboBoxCreaturesFootRight.ItemsSource = itemsNamesSprites[3];
        //                    this.ComboBoxCreaturesFootRight.SelectedIndex = rand.Next(itemsNamesSprites[3].Count);
        //                }
        //            }
        //            else
        //                this.RowDefinitionCreaturesFoot.Height = new GridLength(0);// hide the row
        //        }
        //        else
        //            this.StackPanelCreaturesEquipment.Visibility = Visibility.Hidden;
        //    }
        //    else
        //        this.StackPanelCreaturesEquipment.Visibility = Visibility.Hidden;
        //}

        //private void TabItemCreaturesources_Selected()
        //{
        //    //Fetch this item data from DB
        //    List<object> listValues = App.DB.GetAllDataOfAnItemFromMemory(_selectedItem.PrimaryKeyValue, _selectedItem.PrimaryKeyName, _selectedItem.TableName);

        //    //Fill a list with the data so it can be shown on the DataGrid
        //    List<ViewerDataGridItem> list = new List<ViewerDataGridItem>();
        //    listValues.ForEach(value => list.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eCreatureSources)[list.Count], value)));
        //    this.DataGridCreaturesources.ItemsSource = list;
        //}

        //private void ComboBoxCreatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count == 0) //if there are no items selected we must clear the previous image sprite
        //    {
        //        switch(((ComboBox)sender).Name)
        //        {
        //            case "ComboBoxCreaturesHead":
        //                this.ImageCreatureHead.Source = null;
        //                break;
        //            case "ComboBoxCreaturesNeck":
        //                this.ImageCreatureNeck.Source = null;
        //                break;
        //            case "ComboBoxCreaturesShoulderLeft":
        //                this.ImageCreatureShoulderLeft.Source = null;
        //                break;
        //            case "ComboBoxCreaturesShoulderRight":
        //                this.ImageCreatureShoulderRight.Source = null;
        //                break;
        //            case "ComboBoxCreaturesTorso":
        //                this.ImageCreatureTorso.Source = null;
        //                break;
        //            case "ComboBoxCreaturesBackpack":
        //                this.ImageCreatureBackpack.Source = null;
        //                break;
        //            case "ComboBoxCreaturesWristLeft":
        //                this.ImageCreatureWristLeft.Source = null;
        //                break;
        //            case "ComboBoxCreaturesWristRight":
        //                this.ImageCreatureWristRight.Source = null;
        //                break;
        //            case "ComboBoxCreaturesHandLeft":
        //                this.ImageCreatureHandLeft.Source = null;
        //                break;
        //            case "ComboBoxCreaturesHandRight":
        //                this.ImageCreatureHandRight.Source = null;
        //                break;
        //            case "ComboBoxCreaturesBelt":
        //                this.ImageCreatureBelt.Source = null;
        //                break;
        //            case "ComboBoxCreaturesLegs":
        //                this.ImageCreatureLegs.Source = null;
        //                break;
        //            case "ComboBoxCreaturesFootLeft":
        //                this.ImageCreatureFootLeft.Source = null;
        //                break;
        //            case "ComboBoxCreaturesFootRight":
        //                this.ImageCreatureFootRight.Source = null;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        ComboBoxEquipmentItem item = (ComboBoxEquipmentItem)e.AddedItems[0];

        //        string strSprite = item.Sprite;
        //        string strTable = item.TableName;
        //        //fetch the item sprite, seems that big images exist sometimes so let's try to fetch it and if it does not exist load the small
        //        string strSpritePath = App.DB.GetImagePathFromMemory(strSprite, strTable, true);
        //        bool bIsBig = true;
        //        if (File.Exists(strSpritePath) == false)
        //        {
        //            bIsBig = false;
        //            strSpritePath = App.DB.GetImagePathFromMemory(strSprite, strTable, false);
        //        }

        //        BitmapImage spriteImage = new BitmapImage(new Uri(strSpritePath));

        //        switch (item.BodyPart)
        //        {
        //            case 2: //Foot Left
        //                if (bIsBig)
        //                    this.ImageCreatureFootLeft.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureFootLeft.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 3: //Foot Right
        //                if (bIsBig)
        //                    this.ImageCreatureFootRight.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureFootRight.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 4: //Legs
        //                if (bIsBig)
        //                    this.ImageCreatureLegs.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureLegs.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 7: //Wrist Left
        //                if (bIsBig)
        //                    this.ImageCreatureWristLeft.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureWristLeft.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 8: //Wrist Right
        //                if (bIsBig)
        //                    this.ImageCreatureWristRight.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureWristRight.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 11: //Torso
        //                if (bIsBig)
        //                    this.ImageCreatureTorso.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureTorso.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 12: //Belt
        //                if (bIsBig)
        //                    this.ImageCreatureBelt.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureBelt.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 13: //Shoulder Left
        //                if (bIsBig)
        //                    this.ImageCreatureShoulderLeft.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureShoulderLeft.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 14: //Shoulder Right
        //                if (bIsBig)
        //                    this.ImageCreatureShoulderRight.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureShoulderRight.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 17: //Head
        //                if (bIsBig)
        //                    this.ImageCreatureHead.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureHead.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 20: //Hand Left
        //                if (bIsBig)
        //                    this.ImageCreatureHandLeft.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureHandLeft.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 21: //Hand Right
        //                if (bIsBig)
        //                    this.ImageCreatureHandRight.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureHandRight.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 22: //Backpack
        //                if (bIsBig)
        //                    this.ImageCreatureBackpack.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureBackpack.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //            case 23://Neck
        //                if (bIsBig)
        //                    this.ImageCreatureNeck.Source = App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX);
        //                else
        //                    this.ImageCreatureNeck.Source = new TransformedBitmap(App.ConvertImageDpi(spriteImage, _imageBackgroundFrame.DpiX, _imageBackgroundFrame.DpiX), new ScaleTransform(2, 2));
        //                break;
        //        }
        //    }
        //}
    }
}
