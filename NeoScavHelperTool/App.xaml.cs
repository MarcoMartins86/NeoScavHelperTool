﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        public App() : base()
        {
            _isStartingUp = true;
            _I = this;
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

        public static BitmapSource ConvertImageDpi(BitmapImage image, double dpiX, double dpiY)
        {
            //hack to convert to same dpi //maybe if this is to slow consider using transforms  
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = width * 4; // 4 bytes per pixel
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
    }
}