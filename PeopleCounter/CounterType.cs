using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeopleCounter
{
    public class CounterType : IDisposable
    {
        public Color BackgroundColor { get; set; }
        public Brush ForegroundColor { get; set; }
        public int Counter { get; set; }

        public string IconText { get; set; }
        public NotifyIcon Icon { get; set; }

        public string CSVIdentifier { get; set; }
        public void Dispose()
        {
            Icon.Dispose();
        }
    }
}
