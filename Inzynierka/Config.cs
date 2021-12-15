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

        private Config()
        {
            this.language = "en-US_BroadbandModel";
            this.fontSize = 15;
            this.fontStyle = FontStyles.Normal;
            this.fontColor = new SolidColorBrush(Color.FromRgb(0,0,0));
            this.backgroundColor = new SolidColorBrush(Color.FromRgb(255,255,255));
            this.timeOut = 30;
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
        public void setLanguage(string _language)
        {
            this.language = _language;
        }
        public void setFontSize(int _fontSize)
        {
            this.fontSize = _fontSize;
        }
        public void setFontStyle(FontStyle _fontStyle)
        {
            this.fontStyle= _fontStyle;
        }
        public void setFontColor(Brush _brush)
        {
            this.fontColor = _brush;
        }
        public void setBackgroundColor(Brush _brush)
        {
            this.backgroundColor = _brush;
        }
        public void setTimeOut(int _timeOut)
        {
            this.timeOut = _timeOut;
        }
    }
}
