using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NeoScavModHelperTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DBOperations _dbOp = new DBOperations();
        public static DBOperations DB => _dbOp;
        private static bool _isStartingUp;
        public static ISplashScreen _splashScreen;

        private ManualResetEvent _resetSplashCreated;
        public ManualResetEvent SplashSyncEvent => _resetSplashCreated;

        private static App _I = null;
        public static App I => _I;

        private List<string> _listMods = new List<string>();
        public List<string> Mods => _listMods;

        private List<string> _listModsPrefix = new List<string>();
        public List<string> ModsPrefix => _listModsPrefix;

        private int _dpiY;
        public int DpiY => _dpiY;
        private int _dpiX;
        public int DpiX => _dpiX;

        public App() : base()
        {  
            _isStartingUp = true;
            _I = this;

            //Find the current Dpi and save them to be used later on when drawing images
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            _dpiX = (int)dpiXProperty.GetValue(null, null);
            _dpiY = (int)dpiYProperty.GetValue(null, null);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // ManualResetEvent acts as a block. It waits for a signal to be set.
            _resetSplashCreated = new ManualResetEvent(false);

            // Create a new thread for the splash screen to run on
            Thread _splashThread = new Thread(ShowSplash);
            _splashThread.SetApartmentState(ApartmentState.STA);
            _splashThread.IsBackground = true;
            _splashThread.Name = "Splash Screen";
            _splashThread.Start();

            // Wait for the blocker to be signaled before continuing. This is essentially the same as: while(ResetSplashCreated.NotSet) {}
            SplashSyncEvent.WaitOne();
            
            //Now that the splash screen is shown let's continue

            //Register this method for displaying unhandled exception in the appropriate window
            DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(
                    DispatcherUnhandledExceptionHandler);

            //Inits the database
            _dbOp.Init();

            base.OnStartup(e);

            //Closes the splash screen since everything is done
            App._splashScreen.LoadComplete();
            _isStartingUp = false;
            SplashSyncEvent.Reset();
            //Will wait once more until the splash window is really closed
            SplashSyncEvent.WaitOne();
            SplashSyncEvent.Close();
        }

        private void ShowSplash()
        {
            // Create the window
            AnimatedSplashScreenWindow animatedSplashScreenWindow = new AnimatedSplashScreenWindow();
            _splashScreen = animatedSplashScreenWindow;

            // Show it
            animatedSplashScreenWindow.Show();

            // Now that the window is created, allow the rest of the startup to run
            SplashSyncEvent.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //close the db connection
            _dbOp.Close();
            DispatcherUnhandledException -=
                new DispatcherUnhandledExceptionEventHandler(
                    DispatcherUnhandledExceptionHandler);
            base.OnExit(e);
        }

        //Catch all unhandled exceptions and display a dialog with it, then exits
        public void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                //while is starting up (showing the splash screen) the message box needs to be createad there
                if (_isStartingUp)
                {
                    App._splashScreen.DisplayMessageDialog(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Shutdown(1);
                }
                else
                {
                    MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Shutdown(1);
                }
            }
        }

        public void DisplayMessageDialog(string message, string title, MessageBoxButton button, MessageBoxImage icon, bool exit)
        {
            //while is starting up (showing the splash screen) the message box needs to be createad there
            if (_isStartingUp)
            {
                App._splashScreen.DisplayMessageDialog(message, title, button, icon);
                if (exit)
                {
                    App._splashScreen.LoadComplete();
                    Application.Current.Shutdown(0);
                }
            }
            else
            {
                MessageBox.Show(message, title, button, icon);
                if (exit)
                    Application.Current.Shutdown(0);
            }

        }

        public static BitmapSource CopyImageRectWithDpi(BitmapSource image, Int32Rect rect, double dpiX, double dpiY)
        {
            int width = rect.Width;
            int height = rect.Height;
            var stride = (width * image.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[stride * height];
            image.CopyPixels(rect, pixelData, stride, 0);
            return BitmapSource.Create(width, height, dpiX, dpiY, image.Format, image.Palette, pixelData, stride);
        }

        public static BitmapSource ConvertImageDpi(BitmapSource image, double dpiX, double dpiY)
        {
            //hack to convert to same dpi //maybe if this is to slow consider using transforms  
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            var stride = (width * image.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[stride * height];
            image.CopyPixels(pixelData, stride, 0);
            return BitmapSource.Create(width, height, dpiX, dpiY, image.Format, image.Palette, pixelData, stride);
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
        
        public static BitmapImage GetItemDisplayImage(string item_id, string item_id_origin_table, bool big_image)
        {
            //Find the correct DB to fetch the information
            App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref item_id, ref item_id_origin_table, "itemtypes");
            string[] itemIDSplitted = item_id.Split('.');
            //Ask for item imageList
            string strImageList = App.DB.GetColumnValueFromMemoryTableMultipleAndConditions(new List<string> { "nGroupID", "nSubgroupID" },
            itemIDSplitted.ToList(), item_id_origin_table, "vImageList");
            //Finally fetch the display image
            string strImageName = strImageList.Split(',')[0];
            string strImageTable = item_id_origin_table;
            App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref strImageName, ref strImageTable, "images");
            strImageName = Path.GetFileNameWithoutExtension(strImageName);
            return new BitmapImage(new Uri(App.DB.GetImagePathFromMemory(strImageName, strImageTable, big_image)));
        }
    }
}
