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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NeoScavModHelperTool
{
    /// <summary>
    /// Interaction logic for AnimatedSplashScreenWindow.xaml
    /// </summary>
    public partial class AnimatedSplashScreenWindow : Window, ISplashScreen
    {
        private string _message;
        private string Message
        {
            get { return _message; }
            set { _message = value; TbMessage.Text = _message; }
        }

        public AnimatedSplashScreenWindow()
        {
            InitializeComponent();
        }

        public void UpdateMessage(string message)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                this.Message = message;
            });
        }

        public void LoadComplete()
        {
            Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;
            Dispatcher.InvokeShutdown();
        }

        private void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            App.I.SplashSyncEvent.Set(); //sync with close of this dialog
        }

        public void DisplayMessageDialog(string message, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                System.Windows.MessageBox.Show(this, message, title, button, icon);
            });
        }

        public string AskNeoScavengerGameFolder()
        {
            string strGameFolder = "";
            Dispatcher.Invoke((Action)delegate ()
            {
                OpenFileDialog dlg = new OpenFileDialog();                
                dlg.Title = "Select your NEO Scavenger game folder";
                dlg.Filter = "|NEOScavenger.exe";
                dlg.DefaultExt = "exe";
                dlg.AddExtension = true;
                dlg.FileName = "NEOScavenger";//for some reason if I insert "NEOScavenger.exe", in the dialog only appears "OScavenger.exe"
                if (dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
                {
                    if ((string.Compare(dlg.SafeFileName.ToLower(), "NEOScavenger.exe".ToLower()) == 0) && File.Exists(dlg.FileName))
                    {
                        strGameFolder = System.IO.Path.GetDirectoryName(dlg.FileName);
                    }
                }
            });          

            return strGameFolder;
        }
    }

    public interface ISplashScreen
    {
        void UpdateMessage(string message);
        void LoadComplete();
        void DisplayMessageDialog(string message, string title, MessageBoxButton button, MessageBoxImage icon);
        string AskNeoScavengerGameFolder();
    }
}
