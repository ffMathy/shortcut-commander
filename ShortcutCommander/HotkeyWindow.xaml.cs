using System;
using System.Collections.Generic;
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

namespace Techmatic.ShortcutCommander
{
    /// <summary>
    /// Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window
    {
        public HotkeyWindow()
        {
            InitializeComponent();
            CloseAfterSomeTime();
        }

        public async void CloseAfterSomeTime()
        {
            await Task.Delay(5000);
            Hide();
        }
    }
}
