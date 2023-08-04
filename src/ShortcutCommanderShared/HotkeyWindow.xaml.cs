using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ShortcutCommander
{
    /// <summary>
    /// Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window
    {
        public HotkeyWindow()
        {
            InitializeComponent();
            CloseAfterSomeTimeAsync();
        }

        public async Task CloseAfterSomeTimeAsync()
        {
            await Task.Delay(5000);
            Close();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var handle = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(handle);
        }
    }
}
