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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inzynierka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Config config;
        public MainWindow()
        {
            InitializeComponent();
            config = Config.Instance();
            MainTextBox.FontSize = config.getFontSize();
            MainTextBox.FontStyle = config.getFontStyle();
            MainTextBox.Foreground = config.getFontColor();
            MainTextBox.Background = config.getBackgroundColor();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this, config);
            settingsWindow.Show();
        }

        private void StartListenning_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void StopListening_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
