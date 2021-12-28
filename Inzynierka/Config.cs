using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Inzynierka
{
    public sealed class Config
    {
        private string language { get; set; }
        private int fontSize { get; set; }
        private FontStyle fontStyle { get; set; }
        private Brush fontColor { get; set; }
        private Brush backgroundColor { get; set; }
        private int timeOut { get; set; }
        private bool smartFormatting { get; set; }

        private Config()
        {
            language = "en-US_BroadbandModel";
            fontSize = 15;
            fontStyle = FontStyles.Normal;
            fontColor = new SolidColorBrush(Color.FromRgb(0,0,0));
            backgroundColor = new SolidColorBrush(Color.FromRgb(255,255,255));
            timeOut = 30;
            smartFormatting = true;
        }

        private static Config config = new Config();

        public static Config Instance()
        {
            return config;
        }
        public string getLanguage()
        {
            return language;
        }
        public int getFontSize()
        {
            return fontSize;
        }
        public FontStyle getFontStyle()
        {
            return fontStyle;
        }
        public Brush getFontColor()
        {
            return fontColor;
        }
        public Brush getBackgroundColor()
        {
            return backgroundColor;
        }
        public int getTimeOut()
        {
            return timeOut;
        }
        public bool getSmartFormatting()
        {
            return smartFormatting;
        }
        public void setLanguage(string _language)
        {
            language = _language;
        }
        public void setFontSize(int _fontSize)
        {
            fontSize = _fontSize;
        }
        public void setFontStyle(FontStyle _fontStyle)
        {
            fontStyle= _fontStyle;
        }
        public void setFontColor(Brush _brush)
        {
            fontColor = _brush;
        }
        public void setBackgroundColor(Brush _brush)
        {
            backgroundColor = _brush;
        }
        public void setTimeOut(int _timeOut)
        {
            timeOut = _timeOut;
        }
        public void setSmartFormatting(bool _smartFormatting)
        {
            smartFormatting = _smartFormatting;
        }
    }
}
