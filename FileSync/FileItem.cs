using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileSync
{
    public class FileItem
    {
        public BitmapImage ItemIcon { get; set; }
        public Brush Background { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string Size { get; set; }
        public bool IsFile { get; set; }

        bool Checked = false;
        public bool IsCached
        {
            get
            {
                return Checked;
            }
            set
            {
                Checked = value;
                if (value)
                {
                    Background = Brushes.LightSkyBlue;
                }
            }
        }

        public string GetCachePostfix()
        {
            return FullPath.Remove(FullPath.LastIndexOf(FullPath.Split('/')[FullPath.Split('/').Length - 1])).Replace("/", "\\");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
