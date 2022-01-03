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
            checksmartFormatting();
            checkTextMode();
        }

        private void colorPickerFont_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerFont.SelectedColor.HasValue)
            {
                Color color = colorPickerFont.SelectedColor.Value;
                Brush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                previewTextBlock.Foreground = brush;
            }
        }

        private void colorPickerBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPickerBackground.SelectedColor.HasValue)
            {
                Color color = colorPickerBackground.SelectedColor.Value;
                Brush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                previewTextBlock.Background = brush;   
            }
        }

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {

            setFontStyle();
            setLanguage();
            setSmartFormatting();
            setTextMode();

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
            _config.setTextMode(_startConfig.getTextMode());
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

        private void checksmartFormatting()
        {
            if(englishUS.IsSelected || spanish.IsSelected)
            {
                smartFormattingCheckBox.IsChecked = true;
            }
            else
            {
                smartFormattingCheckBox.IsChecked = false;
            }
        }

        private void setSmartFormatting()
        {
            if(smartFormattingCheckBox.IsChecked == true)
            {
                _config.setSmartFormatting(true);
            }
            else
            {
                _config.setSmartFormatting(false);
            }
        }

        private void setLanguage()
        {
            if (englishUS.IsSelected)
            {
                _config.setLanguage(ModelID.englishUS);
            }
            else if (englishUK.IsSelected)
            {
                _config.setLanguage(ModelID.englishUK);
            }
            else if (englishAUSIE.IsSelected)
            {
                _config.setLanguage(ModelID.englishAUSIE);
            }
            else if (german.IsSelected)
            {
                _config.setLanguage(ModelID.german);
            }
            else if (french.IsSelected)
            {
                _config.setLanguage(ModelID.french);
            }
            else if (frenchCANAD.IsSelected)
            {
                _config.setLanguage(ModelID.frenchCANAD);
            }
            else if (italian.IsSelected)
            {
                _config.setLanguage(ModelID.italian);
            }
            else if (spanish.IsSelected)
            {
                _config.setLanguage(ModelID.spanish);
            }
            else if (portugese.IsSelected)
            {
                _config.setLanguage(ModelID.portugese);
            }
            else if (dutch.IsSelected)
            {
                _config.setLanguage(ModelID.dutch);
            }
            else if (chinese.IsSelected)
            {
                _config.setLanguage(ModelID.chinese);
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
            if (_config.getLanguage().Equals(ModelID.englishUS))
            {
                englishUS.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.englishUK))
            {
                englishUK.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.englishAUSIE))
            {
                englishAUSIE.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.german))
            {
                german.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.french))
            {
                french.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.frenchCANAD))
            {
                frenchCANAD.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.italian))
            {
                italian.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.spanish))
            {
                spanish.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.portugese))
            {
                portugese.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.dutch))
            {
                dutch.IsSelected = true;
            }
            else if (_config.getLanguage().Equals(ModelID.chinese))
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

        private void englishUS_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = true;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void englishUK_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void englishAUSIE_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void german_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void french_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void frenchCANAD_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void italian_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void spanish_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = true;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void portugese_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void dutch_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void chinese_Selected(object sender, RoutedEventArgs e)
        {
            smartFormattingCheckBox.IsEnabled = false;
            smartFormattingCheckBox.IsChecked = false;
        }

        private void checkTextMode()
        {
            if(_startConfig.getTextMode() == 1)
            {
                allTextRadioButton.IsChecked = true;
            }
            else if (_startConfig.getTextMode() == 2)
            {
                partTextRadioButton.IsChecked = true;   
            }
        }

        private void setTextMode()
        {
            if(allTextRadioButton.IsChecked == true)
            {
                _config.setTextMode(1);
            }
            else if(partTextRadioButton.IsChecked == true)
            {
                _config.setTextMode(2);
            }
        }

        private void allTextRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            partTextRadioButton.IsChecked = false;
        }

        private void partTextRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            allTextRadioButton.IsChecked = false;
        }
    }
}
