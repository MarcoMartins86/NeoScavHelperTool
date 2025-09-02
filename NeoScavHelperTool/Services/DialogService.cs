using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using FormsMessageBox = System.Windows.Forms.MessageBox;
using FormsOpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace NeoScavHelperTool.Services
{
    public class DialogService
    {
        private readonly ILogger<DialogService> _logger;
        private readonly Dispatcher _dispatcher;

        public DialogService(ILogger<DialogService> logger, Dispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public void MessageBox(
            string messageBoxText,
            string caption,
            MessageBoxButtons button,
            MessageBoxIcon icon
        )
        {
            _logger.LogTrace(
                $"Lanching MessageBox with:{Environment.NewLine}"
                    + $" - messageBoxText: \"{{messageBoxText}}\",{Environment.NewLine}"
                    + $" - caption: \"{{caption}}\",{Environment.NewLine}"
                    + $" - button: \"{{button}}\",{Environment.NewLine}"
                    + $" - icon: \"{{icon}}\"",
                messageBoxText,
                caption,
                button.ToString(),
                icon.ToString()
            );
            _dispatcher.Invoke(() => FormsMessageBox.Show(messageBoxText, caption, button, icon));
        }

        public string OpenFileDialog(
            string title,
            string fileName,
            string filter,
            string defaultExt,
            bool addExtension
        )
        {
            return _dispatcher.Invoke(() =>
            {
                _logger.LogTrace(
                    $"Lanching OpenFileDialog with:{Environment.NewLine}"
                        + $" - title: \"{{title}}\",{Environment.NewLine}"
                        + $" - fileName: \"{{fileName}}\",{Environment.NewLine}"
                        + $" - filter: \"{{filter}}\",{Environment.NewLine}"
                        + $" - defaultExt: \"{{defaultExt}}\",{Environment.NewLine}"
                        + $" - addExtension: \"{{addExtension}}\"",
                    title,
                    fileName,
                    filter,
                    defaultExt,
                    addExtension
                );

                using (FormsOpenFileDialog dlg = new FormsOpenFileDialog())
                {
                    dlg.Title = title;
                    dlg.FileName = fileName;
                    dlg.Filter = filter;
                    dlg.DefaultExt = defaultExt;
                    dlg.AddExtension = addExtension;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        return dlg.FileName;
                    }
                }
                return string.Empty;
            });
        }
    }
}
