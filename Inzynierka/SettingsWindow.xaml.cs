using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inzynierka
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Config _config;
        private Config _startConfig;
        private MainWindow _mainWindow;
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _config = Config.Instance();
            _startConfig = Config.Instance();
            _mainWindow = mainWindow;

            previewTextBlock.Foreground = _startConfig.getFontColor();
            previewTextBlock.Background = _startConfig.getBackgroundColor();
            previewTextBlock.FontSize = _startConfig.getFontSize();
            previewTextBlock.FontStyle = _startConfig.getFontStyle();
            fontSizeSlider.Value = _startConfig.getFontSize();
            intTimeOut.Text = _startConfig.getTimeOut().ToString();
            checkFontStyle();
            checkLanguage();
        }

        private void colorPickerFont_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerFont.SelectedColor.HasValue)
            {
                Color color = colorPickerFont.SelectedColor.Value;
                Brush brush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                previewTextBlock.Foreground = brush;
            }
        }

        private void colorPickerBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerBackground.SelectedColor.HasValue)
            {
                Color color = colorPickerBackground.SelectedColor.Value;
                Brush brush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                previewTextBlock.Background = brush;   
            }
        }

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {

            setFontStyle();
            setLanguage();

            _config.setFontColor(previewTextBlock.Foreground);
            _config.setBackgroundColor(previewTextBlock.Background);

            _config.setFontSize((int)fontSizeSlider.Value);
            if(intTimeOut.Text != "")
            {
                int value = int.Parse(intTimeOut.Text);
                if (value > 60 && value <= 15)
                    _config.setTimeOut(value);
            }

            _mainWindow.MainTextBox.Background = _config.getBackgroundColor();
            _mainWindow.MainTextBox.Foreground = _config.getFontColor();
            _mainWindow.MainTextBox.FontSize = _config.getFontSize();
            _mainWindow.MainTextBox.FontStyle = _config.getFontStyle();
            Close();
        }

        private void cancelChanges_Click(object sender, RoutedEventArgs e)
        {
            previewTextBlock.Foreground = _startConfig.getFontColor();
            previewTextBlock.Background = _startConfig.getBackgroundColor();
            previewTextBlock.FontSize = _startConfig.getFontSize();
            previewTextBlock.FontStyle = _startConfig.getFontStyle();
            fontSizeSlider.Value = _startConfig.getFontSize();
            Close();
        }

        private void intFontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void intTimeOut_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void setLanguage()
        {
            if (englishUS.IsSelected)
            {
                _config.setLanguage("en-US_BroadbandModel");
            }
            else if (englishUK.IsSelected)
            {
                _config.setLanguage("en-GB_BroadbandModel");
            }
            else if (englishAUSIE.IsSelected)
            {
                _config.setLanguage("en-AU_BroadbandModel");
            }
            else if (german.IsSelected)
            {
                _config.setLanguage("de-DE_BroadbandModel");
            }
            else if (french.IsSelected)
            {
                _config.setLanguage("fr-FR_BroadbandModel");
            }
            else if (frenchCANAD.IsSelected)
            {
                _config.setLanguage("fr-CA_BroadbandModel");
            }
            else if (italian.IsSelected)
            {
                _config.setLanguage("it-IT_BroadbandModel");
            }
            else if (spanish.IsSelected)
            {
                _config.setLanguage("es-ES_BroadbandModel");
            }
            else if (portugese.IsSelected)
            {
                _config.setLanguage("pt-BR_BroadbandModel");
            }
            else if (dutch.IsSelected)
            {
                _config.setLanguage("nl-NL_BroadbandModel");
            }
            else if (chinese.IsSelected)
            {
                _config.setLanguage("zh-CN_BroadbandModel");
            }
        }

        private void setFontStyle()
        {
            if (italic.IsSelected)
            {
                _config.setFontStyle(FontStyles.Italic);
            }
            else if (normal.IsSelected)
            {
                _config.setFontStyle(FontStyles.Normal);
            }
            else if (oblique.IsSelected)
            {
                _config.setFontStyle(FontStyles.Oblique);
            }
        }

        private void intTimeOut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (intTimeOut.Text != "")
            {
                int value = int.Parse(intTimeOut.Text);
                if (value > 60 || value < 15)
                {
                    intTimeOut.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    saveChanges.IsEnabled = false;
                }
                else
                {
                    intTimeOut.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    saveChanges.IsEnabled = true;
                }
            }
        }

        private void fontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(fontSizeSlider != null && previewTextBlock != null)
            {
                previewTextBlock.FontSize = (int)fontSizeSlider.Value;
            }
        }

        private void checkLanguage()
        {
            if (_config.getLanguage().Equals("en-US_BroadbandModel"))
            {
                englishUS.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("en-GB_BroadbandModel"))
            {
                englishUK.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("en-AU_BroadbandModel"))
            {
                englishAUSIE.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("de-DE_BroadbandModel"))
            {
                german.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("fr-FR_BroadbandModel"))
            {
                french.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("fr-CA_BroadbandModel"))
            {
                frenchCANAD.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("it-IT_BroadbandModel"))
            {
                italian.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("es-ES_BroadbandModel"))
            {
                spanish.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("pt-BR_BroadbandModel"))
            {
                portugese.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("nl-NL_BroadbandModel"))
            {
                dutch.IsSelected = true;
            }
            else if (_config.getLanguage().Equals("zh-CN_BroadbandModel"))
            {
                chinese.IsSelected = true;
            }
        }

        private void checkFontStyle()
        {
            if (_config.getFontStyle().Equals(FontStyles.Italic))
            {
                italic.IsSelected = true;
            }
            else if (_config.getFontStyle().Equals(FontStyles.Normal))
            {
                normal.IsSelected = true;
            }
            else if (_config.getFontStyle().Equals(FontStyles.Oblique))
            {
                oblique.IsSelected = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (italic.IsSelected)
            {
                previewTextBlock.FontStyle = FontStyles.Italic;
            }
            else if (normal.IsSelected)
            {
                previewTextBlock.FontStyle = FontStyles.Normal;
            }
            else if (oblique.IsSelected)
            {
                previewTextBlock.FontStyle = FontStyles.Oblique;
            }
        }
    }
}
