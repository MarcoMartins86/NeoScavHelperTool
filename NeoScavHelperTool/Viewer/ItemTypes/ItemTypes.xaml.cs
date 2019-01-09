using NeoScavHelperTool;
using NeoScavHelperTool.DBTableAttributes;
using NeoScavHelperTool.TableObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
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

namespace NeoScavHelperTool.Viewer.ItemTypes
{
    /// <summary>
    /// Interaction logic for ItemTypes.xaml
    /// </summary>
    public partial class ItemTypes : UserControl, IChangeGUIType
    {
        private enum EImageUsage
        {
            eStored, //when the item is in a grid
            eStoredFull, //when the item's container slots are occupied and it's in a grid
            eEquiped, //when the item is in an allowed equip slot
            eEquipedFull, //when the item's container slots are occupied and it's in an allowed equip slot
            eHeldMouse, //when the item is picked up by your cursor
            eHeldFullMouse //when the item's container slots are occupied and it's picked up by your cursor
        }
        private enum EItemMode
        {
            eEmpty,
            eFull
        }
        private enum EEquipSlots
        {
            eInvalid = -1,
            eFootLeft = 2,
            eFootRight = 3,
            eLegs = 4,
            eWristLeft = 5,
            eWristRight = 6,
            eHandLeft = 7,
            eHandRight = 8,
            eTorso = 11,
            eBelt = 12,
            eShoulderLeft = 13,
            eShoulderRight = 14,
            eHead = 17,
            eHoldLeft = 20,
            eHoldRight = 21,
            eBackpack = 22,
            eNeck = 23
        }
        private readonly BackgroundWorker _loadItemsWorker = new BackgroundWorker();
        private readonly BackgroundWorker _changeGUITypeWorker = new BackgroundWorker();

        private bool _isOnBigGUI = true;
        private bool _alreadyLoaded = false;
        private object[] _arrayDBValues;
        private List<ViewerDataGridItem> _dataGridItems = new List<ViewerDataGridItem>();
        private static EItemMode _itemMode = EItemMode.eEmpty;

        public ItemTypes()
        {
            InitializeComponent();

            _loadItemsWorker.DoWork += LoadItemsWoker_DoWork;
            _loadItemsWorker.RunWorkerCompleted += LoadItemsWoker_RunWorkerCompleted;

            _changeGUITypeWorker.DoWork += ChangeGUIType_DoWork;
            _changeGUITypeWorker.RunWorkerCompleted += ChangeGUIType_RunWorkerCompleted;
        }

        private void ItemTypesControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.I != null && _alreadyLoaded == false)
            {
                MainWindow.I.StartWaitSpinner();

                _loadItemsWorker.RunWorkerAsync();
            }
        }

        public BitmapSource CreateCellsGrid(int width, int heigth, bool big_gui)
        {
            //Select the cell image we will use
            BitmapSource cell = big_gui ? Images.Images.GUICellBigImage : Images.Images.GUICellSmallImage;
            //Draw it the needed times
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                for (int row = 0; row < heigth; row++)
                {
                    for (int column = 0; column < width; column++)
                    {
                        drawingContext.DrawImage(cell, new Rect(column * cell.PixelWidth, row * cell.PixelHeight, cell.PixelWidth, cell.PixelHeight));
                    }
                }
            }
            //render this to a bitmap 
            RenderTargetBitmap mergedImage = new RenderTargetBitmap(width * cell.PixelWidth, heigth * cell.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            mergedImage.Render(visual);
            // convert to our dpi
            BitmapSource cellGrid = Images.Images.ConvertImageDpi(mergedImage, App.I.DpiX, App.I.DpiY);
            // freeze the image so it can be used in multiple threads
            cellGrid.Freeze();
            return cellGrid;
        }

        private void RenderControl(FrameworkElement control)
        {
            if (control.IsMeasureValid == false)
            {
                control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size visualSize = control.DesiredSize;
                control.Arrange(new Rect(new Point(0, 0), visualSize));
                control.UpdateLayout();
            }
        }

        public static void GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref string str_item, ref string str_table, string str_table_sufix)
        {
            string[] strItemSplitted = str_item.Split(':');
            if (strItemSplitted.Length == 1) //normal case
            {
                str_table = str_table.Split('_')[0] + "_" + str_table_sufix;
            }
            else //case when image is referenced from another mod like 0:imagename.png
            {
                str_item = strItemSplitted[1];
                str_table = strItemSplitted[0] + "_" + str_table_sufix;
            }
        }

        private BitmapSource GetEquipSlotsItemImages(string [] image_list, string [] image_usage, string [] equip_slots, bool b_use_empty_default, bool b_use_full_default, bool b_equiped, EItemMode item_mode, string table, bool big_gui, bool is_mirrored)
        {
            string strImage;
            if (item_mode == EItemMode.eEmpty)
            {
                if (b_use_empty_default)
                    strImage = image_list[Convert.ToInt32(image_usage[(int)(b_equiped ? EImageUsage.eEquiped : EImageUsage.eStored)])];
                else
                {
                    //it seems that indexes can also have Shoulder:0 and it is valid for some reason
                    string[] strIndexSplit = equip_slots[1].Split(':');
                    int index = Convert.ToInt32(strIndexSplit.Length > 1 ? strIndexSplit[1] : strIndexSplit[0]);
                    strImage = image_list[index];
                }
            }
            else
            {
                if (b_use_full_default)
                    strImage = image_list[Convert.ToInt32(image_usage[(int)(b_equiped ? EImageUsage.eEquipedFull : EImageUsage.eStoredFull)])];
                else
                {
                    //it seems that indexes can also have Shoulder:0 and it is valid for some reason
                    string[] strIndexSplit = equip_slots[2].Split(':');
                    int index = Convert.ToInt32(strIndexSplit.Length > 1 ? strIndexSplit[1] : strIndexSplit[0]);
                    strImage = image_list[index];
                }
            }

            return GetItemImage(strImage, table, big_gui, is_mirrored);
        }

        private BitmapSource GetItemImage(string image, string table, bool big_gui, bool is_mirrored)
        {
            GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref image, ref table, "images");
            return Images.Images.GetImageToDraw(System.IO.Path.GetFileNameWithoutExtension(image), table, big_gui, is_mirrored);
        }

        private void CenterControlAOnBOnCanvas(FrameworkElement control_a, FrameworkElement control_b)
        {
            //We need to render the controls so we can get it's measures
            RenderControl(control_a);
            RenderControl(control_b);
            double controlBLeft = Canvas.GetLeft(control_b);
            double controlBTop = Canvas.GetTop(control_b);            
            double controlBWidth = control_b.ActualWidth;
            double controlBHeight = control_b.ActualHeight;
            double controlAWidth = control_a.ActualWidth;
            double controlAHeight = control_a.ActualHeight;

            double dLeft = (controlAWidth - controlBWidth) * 0.5;
            double dTop = (controlAHeight - controlBHeight) * 0.5;

            Canvas.SetLeft(control_a, controlBLeft - dLeft);
            Canvas.SetTop(control_a, controlBTop - dTop);
        }

        private void CreateUpdateCanvas()
        {
            // we only neet to Load/create all needed images used on the inventory screen on first run and on switching GUI mode, otherwise we will only change the items images
            bool bNeedToPositionControls = false;
            if (_isOnBigGUI != MainWindow.I.IsBigGUISelected ||
                _alreadyLoaded == false)
            {
                _isOnBigGUI = MainWindow.I.IsBigGUISelected;
                bNeedToPositionControls = true;

                BitmapSource inventoryCells = CreateCellsGrid(20, 29, _isOnBigGUI);
                BitmapSource neck = Images.Images.GetImageToDraw("btn_inv_neck", "0_images", _isOnBigGUI, false);
                BitmapSource torsoInventoryCells = CreateCellsGrid(3, 4, _isOnBigGUI);
                BitmapSource handR = Images.Images.GetImageToDraw("btn_inv_body_handr", "0_images", _isOnBigGUI, false);
                BitmapSource wristR = Images.Images.GetImageToDraw("btn_inv_body_wristr", "0_images", _isOnBigGUI, false);
                BitmapSource shoulderR = Images.Images.GetImageToDraw("btn_inv_body_shoulderr", "0_images", _isOnBigGUI, false);
                BitmapSource head = Images.Images.GetImageToDraw("btn_inv_body_head", "0_images", _isOnBigGUI, false);
                BitmapSource torso = Images.Images.GetImageToDraw("btn_inv_body_torso", "0_images", _isOnBigGUI, false);
                BitmapSource belt = Images.Images.GetImageToDraw("btn_inv_body_belt", "0_images", _isOnBigGUI, false);
                BitmapSource legs = Images.Images.GetImageToDraw("btn_inv_body_legs", "0_images", _isOnBigGUI, false);
                BitmapSource footR = Images.Images.GetImageToDraw("btn_inv_body_shoer", "0_images", _isOnBigGUI, false);
                BitmapSource shoulderL = Images.Images.GetImageToDraw("btn_inv_body_shoulderl", "0_images", _isOnBigGUI, false);
                BitmapSource wristL = Images.Images.GetImageToDraw("btn_inv_body_wristl", "0_images", _isOnBigGUI, false);
                BitmapSource handL = Images.Images.GetImageToDraw("btn_inv_body_handl", "0_images", _isOnBigGUI, false);
                BitmapSource footL = Images.Images.GetImageToDraw("btn_inv_body_shoel", "0_images", _isOnBigGUI, false);
                BitmapSource backpack = Images.Images.GetImageToDraw("btn_inv_backpack", "0_images", _isOnBigGUI, false);
                BitmapSource holdR = Images.Images.GetImageToDraw("btn_inv_body_holdr", "0_images", _isOnBigGUI, false);
                BitmapSource holdL = Images.Images.GetImageToDraw("btn_inv_body_holdl", "0_images", _isOnBigGUI, false);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //initialize our auxiliary variable that will keep track of where we will draw
                    double dTotalWidth = Canvas.GetLeft(InventoryCellsImage);
                    //set the cell grid image and calculate its width
                    InventoryCellsImage.Source = inventoryCells;
                    RenderControl(InventoryCellsImage);
                    dTotalWidth += InventoryCellsImage.ActualWidth;
                    // find the width and height of a single cell on screen and let's use these measures to position everything
                    double dCellWidth = InventoryCellsImage.ActualWidth / 20;
                    double dCellHeight = InventoryCellsImage.ActualHeight / 29;

                    double dTotalHeight = 2.5 * dCellHeight;
                    Canvas.SetTop(InventoryCellsImage, dTotalHeight);
                    //let's draw the item in the iventory
                    Canvas.SetTop(InventoryItemImage, dTotalHeight);

                    //Update the total height
                    dTotalHeight += InventoryCellsImage.ActualHeight;

                    // Set the neck image and push down the inventory grid
                    double dNeckLeft = dTotalWidth + 2 * dCellWidth;
                    NeckImage.Source = neck;
                    Canvas.SetLeft(NeckImage, dNeckLeft);
                    Canvas.SetTop(NeckImage, 0);

                    //set all the body parts
                    double dBodyLeft = dTotalWidth - 2.5 * dCellWidth;
                    double dBodyTop = Canvas.GetTop(InventoryCellsImage) + 0.5 * dCellHeight;
                    HandRImage.Source = handR;
                    Canvas.SetLeft(HandRImage, dBodyLeft);
                    Canvas.SetTop(HandRImage, dBodyTop);
                    WristRImage.Source = wristR;
                    Canvas.SetLeft(WristRImage, dBodyLeft);
                    Canvas.SetTop(WristRImage, dBodyTop);
                    ShoulderRImage.Source = shoulderR;
                    Canvas.SetLeft(ShoulderRImage, dBodyLeft);
                    Canvas.SetTop(ShoulderRImage, dBodyTop);
                    double shoulderRItemInventoryCellsLeft = dBodyLeft + 3.5 * dCellWidth;
                    double shoulderRItemInventoryCellsTop = dBodyTop + dCellHeight;
                    Canvas.SetLeft(ShoulderRItemInventoryCellsImage, shoulderRItemInventoryCellsLeft);
                    Canvas.SetTop(ShoulderRItemInventoryCellsImage, shoulderRItemInventoryCellsTop);                   
                    HeadImage.Source = head;
                    Canvas.SetLeft(HeadImage, dBodyLeft);
                    Canvas.SetTop(HeadImage, dBodyTop);
                    TorsoImage.Source = torso;
                    Canvas.SetLeft(TorsoImage, dBodyLeft);
                    Canvas.SetTop(TorsoImage, dBodyTop);
                    double torsoItemInventoryCellsTop = dBodyTop + 6 * dCellHeight;
                    Canvas.SetLeft(TorsoItemInventoryCellsImage, shoulderRItemInventoryCellsLeft);
                    Canvas.SetTop(TorsoItemInventoryCellsImage, torsoItemInventoryCellsTop);
                    BeltImage.Source = belt;
                    Canvas.SetLeft(BeltImage, dBodyLeft);
                    Canvas.SetTop(BeltImage, dBodyTop);
                    double beltItemInventoryCellsTop = dBodyTop + 17 * dCellHeight;
                    Canvas.SetLeft(BeltItemInventoryCellsImage, shoulderRItemInventoryCellsLeft);
                    Canvas.SetTop(BeltItemInventoryCellsImage, beltItemInventoryCellsTop);
                    LegsImage.Source = legs;
                    Canvas.SetLeft(LegsImage, dBodyLeft);
                    Canvas.SetTop(LegsImage, dBodyTop);
                    double legsItemInventoryCellsTop = dBodyTop + 22 * dCellHeight;
                    Canvas.SetLeft(LegsItemInventoryCellsImage, shoulderRItemInventoryCellsLeft);
                    Canvas.SetTop(LegsItemInventoryCellsImage, legsItemInventoryCellsTop);
                    FootRImage.Source = footR;
                    Canvas.SetLeft(FootRImage, dBodyLeft);
                    Canvas.SetTop(FootRImage, dBodyTop);
                    ShoulderLImage.Source = shoulderL;
                    Canvas.SetLeft(ShoulderLImage, dBodyLeft);
                    Canvas.SetTop(ShoulderLImage, dBodyTop);
                    double dShoulderLItemInventoryCellsLeft = dBodyLeft + 14 * dCellHeight;
                    double dShoulderLItemInventoryCellsTop = 0.6 * dCellHeight;
                    Canvas.SetLeft(ShoulderLItemInventoryCellsImage, dShoulderLItemInventoryCellsLeft);
                    Canvas.SetTop(ShoulderLItemInventoryCellsImage, dShoulderLItemInventoryCellsTop);
                    WristLImage.Source = wristL;
                    Canvas.SetLeft(WristLImage, dBodyLeft);
                    Canvas.SetTop(WristLImage, dBodyTop);
                    HandLImage.Source = handL;
                    Canvas.SetLeft(HandLImage, dBodyLeft);
                    Canvas.SetTop(HandLImage, dBodyTop);
                    FootLImage.Source = footL;
                    Canvas.SetLeft(FootLImage, dBodyLeft);
                    Canvas.SetTop(FootLImage, dBodyTop);
                    RenderControl(FootLImage);
                    dTotalWidth += FootLImage.ActualWidth;

                    double dBackpackLeft = dTotalWidth - 8.25 * dCellWidth;
                    double dBackpackTop = dBodyTop + 0.5 * dCellHeight;
                    BackpackImage.Source = backpack;
                    Canvas.SetLeft(BackpackImage, dBackpackLeft);
                    Canvas.SetTop(BackpackImage, dBackpackTop);
                    double dHoldBackpackItemInventoryCellsLeft = dTotalWidth - 1.7 * dCellWidth;
                    double dBackpackItemInventoryCellsTop = 0.6 * dCellHeight;
                    Canvas.SetLeft(BackpackItemInventoryCellsImage, dHoldBackpackItemInventoryCellsLeft);
                    Canvas.SetTop(BackpackItemInventoryCellsImage, dBackpackItemInventoryCellsTop);


                    double dHoldLeft = dTotalWidth - 6.25 * dCellWidth;
                    double dHoldRTop = dBackpackTop + 10 * dCellHeight;
                    HoldRImage.Source = holdR;
                    Canvas.SetLeft(HoldRImage, dHoldLeft);
                    Canvas.SetTop(HoldRImage, dHoldRTop);
                    
                    double dHoldRItemInventoryCellsTop = dHoldRTop - 2.6 * dCellHeight;
                    Canvas.SetLeft(HoldRItemInventoryCellsImage, dHoldBackpackItemInventoryCellsLeft);
                    Canvas.SetTop(HoldRItemInventoryCellsImage, dHoldRItemInventoryCellsTop);

                    double dHoldLTop = dHoldRTop + 10 * dCellHeight;
                    HoldLImage.Source = holdL;
                    Canvas.SetLeft(HoldLImage, dHoldLeft);
                    Canvas.SetTop(HoldLImage, dHoldLTop);
                    RenderControl(HoldLImage);
                    double dHoldLItemInventoryCellsTop = dHoldLTop - 2.25 * dCellHeight;
                    Canvas.SetLeft(HoldLItemInventoryCellsImage, dHoldBackpackItemInventoryCellsLeft);
                    Canvas.SetTop(HoldLItemInventoryCellsImage, dHoldLItemInventoryCellsTop);

                    ItemTypesContainerCanvas.Width = dTotalWidth;
                    ItemTypesContainerCanvas.Height = dTotalHeight;
                }));                
            }

            string[] strImageList = _arrayDBValues[(int)EDBItemTypesTableColumns.eVImageList].ToString().Split(',');
            string[] strImageUsage = _arrayDBValues[(int)EDBItemTypesTableColumns.eVImageUsage].ToString().Split(',');
            string[] strEquipSlots = _arrayDBValues[(int)EDBItemTypesTableColumns.eVEquipSlots].ToString().Split(',');

            string strItemTable = Viewer.I.SelectedItem.TableName;            
            string strStoredImage = _itemMode == EItemMode.eEmpty ? strImageList[Convert.ToInt32(strImageUsage[(int)EImageUsage.eStored])] : strImageList[Convert.ToInt32(strImageUsage[(int)EImageUsage.eStoredFull])];
            bool bIsMirrored = Convert.ToBoolean(_arrayDBValues[(int)EDBItemTypesTableColumns.eBMirrored]);
            BitmapSource inventoryItem = GetItemImage(strStoredImage, strItemTable, _isOnBigGUI, bIsMirrored);

            // Create a bitmap source to be reused with the item capacity inventory cells if it exists
            BitmapSource itemCapacityInventoryCells = null;
            string strCapacity = _arrayDBValues[(int)EDBItemTypesTableColumns.eACapacities].ToString();
            if(string.IsNullOrEmpty(strCapacity) == false)
            {
                string[] strCapacitySplitted = strCapacity.Split('x');
                itemCapacityInventoryCells = CreateCellsGrid(Convert.ToInt32(strCapacitySplitted[0]), Convert.ToInt32(strCapacitySplitted[1]), _isOnBigGUI);
            }

            BitmapSource footLItem = null;
            BitmapSource footRItem = null;
            BitmapSource holdLItem = null;
            BitmapSource holdLItemInventoryCells = null;
            BitmapSource holdRItem = null;
            BitmapSource holdRItemInventoryCells = null;
            BitmapSource backpackItem = null;
            BitmapSource backpackItemInventoryCells = null;
            BitmapSource torsoItem = null;
            BitmapSource torsoItemInventoryCells = null;
            BitmapSource neckItem = null;
            BitmapSource wristLItem = null;
            BitmapSource wristRItem = null;
            BitmapSource legsItem = null;
            BitmapSource legsItemInventoryCells = null;
            BitmapSource shoulderLItem = null;
            BitmapSource shoulderLItemInventoryCells = null;
            BitmapSource shoulderRItem = null;
            BitmapSource shoulderRItemInventoryCells = null;
            BitmapSource headItem = null;
            BitmapSource handLItem = null;
            BitmapSource handRItem = null;
            BitmapSource beltItem = null;
            BitmapSource beltItemInventoryCells = null;
            
            //Check if this item can be equiped
            if (strEquipSlots.Length > 0 && string.IsNullOrEmpty(strEquipSlots[0]) == false)
            {
                foreach (string strEquipSlot in strEquipSlots)
                {
                    string[] strEquipSlotAndImages = strEquipSlot.Split('=');
                    //just a quick check to see if it is a valid slot
                    int nSlotNumber = Convert.ToInt32(strEquipSlotAndImages[0]);
                    if (Convert.ToInt32(strEquipSlotAndImages[0]) != (int)EEquipSlots.eInvalid)
                    {
                        bool bUseImageListEmptyDefault = true;
                        bool bUseImageListFullDefault = true;
                        switch (strEquipSlotAndImages.Length)
                        {
                            case 1: //case where we only have the slot number and no image
                                bUseImageListEmptyDefault = true;
                                bUseImageListFullDefault = true;
                                break;
                            case 2: //case where we have the slot number and empty image
                                bUseImageListEmptyDefault = false;
                                bUseImageListFullDefault = true;
                                break;
                            case 3: //case where we have the slot number, empty and full image
                                bUseImageListEmptyDefault = false;
                                bUseImageListFullDefault = false;
                                break;
                        }

                        switch ((EEquipSlots)nSlotNumber)
                        {
                            case EEquipSlots.eFootLeft:
                                footLItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, true);
                                break;
                            case EEquipSlots.eFootRight:
                                footRItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                break;
                            case EEquipSlots.eLegs:
                                legsItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                legsItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eWristLeft:
                                wristLItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, true);
                                break;
                            case EEquipSlots.eWristRight:
                                wristRItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                break;
                            case EEquipSlots.eHandLeft:
                                handLItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, true);
                                break;
                            case EEquipSlots.eHandRight:
                                handRItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                break;
                            case EEquipSlots.eTorso:
                                torsoItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                torsoItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eBelt:
                                beltItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                beltItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eShoulderLeft:
                                shoulderLItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, true);
                                shoulderLItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eShoulderRight:
                                shoulderRItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                shoulderRItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eHead:
                                headItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                break;
                            case EEquipSlots.eHoldLeft:
                                holdLItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, false, _itemMode, strItemTable, _isOnBigGUI, bIsMirrored);
                                holdLItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eHoldRight:
                                holdRItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, false, _itemMode, strItemTable, _isOnBigGUI, bIsMirrored);
                                holdRItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eBackpack:
                                backpackItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                backpackItemInventoryCells = itemCapacityInventoryCells;
                                break;
                            case EEquipSlots.eNeck:
                                neckItem = GetEquipSlotsItemImages(strImageList, strImageUsage, strEquipSlotAndImages, bUseImageListEmptyDefault, bUseImageListFullDefault, true, _itemMode, strItemTable, _isOnBigGUI, false);
                                break;
                            case (EEquipSlots)100: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)101: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)102: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)103: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)104: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)105: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)106: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)107: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)108: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)109: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)110: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)111: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)112: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)113: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)114: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)115: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)116: //TODO: what's this slot? (wounds)
                                break;
                            case (EEquipSlots)200: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)201: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)202: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)203: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)204: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)205: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)206: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)207: //TODO: vehicle
                                break;
                            case (EEquipSlots)208: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)209: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)210: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)213: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)214: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)215: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)216: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)217: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)218: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)219: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)220: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)221: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)222: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)223: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)224: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)225: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)226: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)227: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)228: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)229: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)230: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)231: //TODO: what's this slot?
                                break;
                            case (EEquipSlots)232: //TODO: what's this slot?
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                InventoryItemImage.Source = inventoryItem;
                FootLItemImage.Source = footLItem;
                FootRItemImage.Source = footRItem;
                TorsoItemImage.Source = torsoItem;
                TorsoItemInventoryCellsImage.Source = torsoItemInventoryCells;
                HoldLItemImage.Source = holdLItem;
                HoldLItemInventoryCellsImage.Source = holdLItemInventoryCells;
                HoldRItemImage.Source = holdRItem;
                HoldRItemInventoryCellsImage.Source = holdRItemInventoryCells;
                BackpackItemImage.Source = backpackItem;
                BackpackItemInventoryCellsImage.Source = backpackItemInventoryCells;
                NeckItemImage.Source = neckItem;
                WristLItemImage.Source = wristLItem;
                WristRItemImage.Source = wristRItem;
                LegsItemImage.Source = legsItem;
                LegsItemInventoryCellsImage.Source = legsItemInventoryCells;
                ShoulderLItemImage.Source = shoulderLItem;
                ShoulderLItemInventoryCellsImage.Source = shoulderLItemInventoryCells;
                ShoulderRItemImage.Source = shoulderRItem;
                ShoulderRItemInventoryCellsImage.Source = shoulderRItemInventoryCells;
                HeadItemImage.Source = headItem;
                HandLItemImage.Source = handLItem;
                HandRItemImage.Source = handRItem;
                BeltItemImage.Source = beltItem;
                BeltItemInventoryCellsImage.Source = beltItemInventoryCells;
                //Let's position the controls only once
                if (bNeedToPositionControls)
                {
                    if (footLItem != null)
                        CenterControlAOnBOnCanvas(FootLItemImage, FootLImage);
                    if (footRItem != null)
                        CenterControlAOnBOnCanvas(FootRItemImage, FootRImage);
                    if (torsoItem != null)
                        CenterControlAOnBOnCanvas(TorsoItemImage, TorsoImage);
                    if (holdLItem != null)
                        CenterControlAOnBOnCanvas(HoldLItemImage, HoldLImage);
                    if (holdRItem != null)
                        CenterControlAOnBOnCanvas(HoldRItemImage, HoldRImage);
                    if (BackpackItemImage != null)
                        CenterControlAOnBOnCanvas(BackpackItemImage, BackpackImage);
                    if (neckItem != null)
                        CenterControlAOnBOnCanvas(NeckItemImage, NeckImage);
                    if (wristLItem != null)
                        CenterControlAOnBOnCanvas(WristLItemImage, WristLImage);
                    if (wristRItem != null)
                        CenterControlAOnBOnCanvas(WristRItemImage, WristRImage);
                    if (legsItem != null)
                        CenterControlAOnBOnCanvas(LegsItemImage, LegsImage);
                    if (shoulderLItem != null)
                        CenterControlAOnBOnCanvas(ShoulderLItemImage, ShoulderLImage);
                    if (shoulderRItem != null)
                        CenterControlAOnBOnCanvas(ShoulderRItemImage, ShoulderRImage);
                    if (headItem != null)
                        CenterControlAOnBOnCanvas(HeadItemImage, HeadImage);
                    if (handLItem != null)
                        CenterControlAOnBOnCanvas(HandLItemImage, HandLImage);
                    if (handRItem != null)
                        CenterControlAOnBOnCanvas(HandRItemImage, HandRImage);
                    if(beltItem != null)
                        CenterControlAOnBOnCanvas(BeltItemImage, BeltImage);

                    // Check if we need to recalculate the canvas width, inventory cells from hold items will take precedence over hold items
                    Image controlInventoryCellsReCalculateCanvasWidth = BackpackItemInventoryCellsImage.Source != null ? BackpackItemInventoryCellsImage : HoldLItemInventoryCellsImage.Source != null ? HoldLItemInventoryCellsImage : HoldRItemInventoryCellsImage.Source != null ? HoldRItemInventoryCellsImage : null;
                    Image controlHoldReCalculateCanvasWidth = HoldLItemImage.Source != null ? HoldLItemImage : HoldRItemImage.Source != null ? HoldRItemImage : null;
                    double dInventoryCellsCanvasWidth = 0;
                    double dHoldItemCanvasWigth = 0;
                    if (controlInventoryCellsReCalculateCanvasWidth != null)
                    {
                        RenderControl(controlInventoryCellsReCalculateCanvasWidth);
                        double dLeft = Canvas.GetLeft(controlInventoryCellsReCalculateCanvasWidth);
                        double dWidth = controlInventoryCellsReCalculateCanvasWidth.ActualWidth;
                        dInventoryCellsCanvasWidth = dLeft + dWidth;
                    }
                    if (controlHoldReCalculateCanvasWidth != null)
                    {
                        RenderControl(controlHoldReCalculateCanvasWidth);
                        double dLeft = Canvas.GetLeft(controlHoldReCalculateCanvasWidth);
                        double dWidth = controlHoldReCalculateCanvasWidth.ActualWidth;
                        dHoldItemCanvasWigth = dLeft + dWidth;                        
                    }

                    // let's update with the max width
                    ItemTypesContainerCanvas.Width = Math.Max(Math.Max(ItemTypesContainerCanvas.Width, dHoldItemCanvasWigth), dInventoryCellsCanvasWidth);
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
                _dataGridItems.Add(new ViewerDataGridItem(DBTableAttributtesFetcher.GetColumnsNames(EDBTable.eItemTypes)[_dataGridItems.Count], columnValue));
            }
            //2nd - Set the checked item mode
            Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (_itemMode)
                {
                    case EItemMode.eEmpty:
                        RadioButtonEmpty.IsChecked = true;
                        break;
                    case EItemMode.eFull:
                        RadioButtonFull.IsChecked = true;
                        break;
                }
            }));
            //3rd - Display the information on canvas
            CreateUpdateCanvas();
        }

        private void LoadItemsWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _alreadyLoaded = true;
            // Update the GUI
            string strDesc = string.IsNullOrEmpty(_arrayDBValues[(int)EDBItemTypesTableColumns.eStrDescAlt].ToString()) ? _arrayDBValues[(int)EDBItemTypesTableColumns.eStrDesc].ToString() : _arrayDBValues[(int)EDBItemTypesTableColumns.eStrDescAlt].ToString();
            ItemTypesTitle.Text = string.Format("{0}: {1}", _arrayDBValues[(int)EDBItemTypesTableColumns.eStrName], strDesc); ;
            DataGridItemTypes.ItemsSource = _dataGridItems;
            ItemTypesMainGrid.Visibility = Visibility.Visible;
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

        private void ItemFullnessRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            //Update the control variable with correct day time
            switch (radioButton.Content.ToString())
            {
                case "Item Empty":
                    _itemMode = EItemMode.eEmpty;
                    break;
                case "Item Full":
                    _itemMode = EItemMode.eFull;
                    break;
            }
            //Refresh the GUI to show reflect the change
            ChangeGUIType();
        }

        private void DataGridItemTypes_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Let's set the value column to the style that wraps the text
            DataGridTextColumn dgTextC = (DataGridTextColumn)e.Column;
            if (dgTextC != null && string.Equals("Value", dgTextC.Header))
                dgTextC.ElementStyle = this.FindResource("WrapText") as Style;
        }
    }
}
