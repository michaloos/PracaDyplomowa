using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Inzynierka
{
    public class Config
    {
        public string language { get; set; }
        public int fontSize { get; set; }
        public string fontFamily { get; set; }
        public Brush fontColor { get; set; }
        public Brush backgroundColor { get; set; }
        public int timeOut { get; set; }
    }
}
