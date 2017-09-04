using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileSync
{
    public static class R
    {
        public static BitmapImage folderIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\folder.png"));
        public static BitmapImage fileIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\file.png"));

        public static BitmapImage homeIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\home.png"));
        public static BitmapImage sdIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\sd.png"));
        public static BitmapImage backIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\back.png"));

        public static BitmapImage musicIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\music.png"));
        public static BitmapImage imageIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\image.png"));
        public static BitmapImage videoIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\video.png"));
        public static BitmapImage archiveIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\archive.png"));
        public static BitmapImage apkIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\apk.png"));
        public static BitmapImage wordIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\word.png"));
        public static BitmapImage excelIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\excel.png"));
        public static BitmapImage powerPointIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\powerpoint.png"));
        public static BitmapImage accessIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\access.png"));
        public static BitmapImage txtIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\txt.png"));
        public static BitmapImage pdfIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\pdf.png"));
        public static BitmapImage exeIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\exe.png"));
        public static BitmapImage psdIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\psd.png"));
        public static BitmapImage psbIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\psb.png"));
        public static BitmapImage csIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\cs.png"));
        public static BitmapImage djvuIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\djvu.png"));
        public static BitmapImage binIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\bin.png"));
        public static BitmapImage xmlIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\xml.png"));
        public static BitmapImage xamlIco = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\resources\\icons\\xaml.png"));

        public static BitmapImage GetIconByFormat(string format)
        {
            if (format.ToLower().EndsWith(".mp3") ||
                format.ToLower().EndsWith(".wav") ||
                format.ToLower().EndsWith(".aiff") ||
                format.ToLower().EndsWith(".ape") ||
                format.ToLower().EndsWith(".flac") ||
                format.ToLower().EndsWith(".ogg"))
            {
                return musicIco;
            }
            if (format.ToLower().EndsWith(".jpg") ||
                format.ToLower().EndsWith(".png") ||
                format.ToLower().EndsWith(".bmp") ||
                format.ToLower().EndsWith(".jpeg"))
            {
                return imageIco;
            }
            if (format.ToLower().EndsWith(".avi") ||
                format.ToLower().EndsWith(".wmv") ||
                format.ToLower().EndsWith(".mov") ||
                format.ToLower().EndsWith(".mkv") ||
                format.ToLower().EndsWith(".3gp") ||
                format.ToLower().EndsWith(".mp4"))
            {
                return videoIco;
            }
            if (format.ToLower().EndsWith(".zip") ||
                format.ToLower().EndsWith(".rar") ||
                format.ToLower().EndsWith(".tar") ||
                format.ToLower().EndsWith(".7z") ||
                format.ToLower().EndsWith(".arj") ||
                format.ToLower().EndsWith(".lzh") ||
                format.ToLower().EndsWith(".ace") ||
                format.ToLower().EndsWith(".gzip") ||
                format.ToLower().EndsWith(".uue") ||
                format.ToLower().EndsWith(".chm") ||
                format.ToLower().EndsWith(".cpio") ||
                format.ToLower().EndsWith(".deb") ||
                format.ToLower().EndsWith(".dmg") ||
                format.ToLower().EndsWith(".hfs") ||
                format.ToLower().EndsWith(".lzma") ||
                format.ToLower().EndsWith(".msi") ||
                format.ToLower().EndsWith(".msis") ||
                format.ToLower().EndsWith(".rpm") ||
                format.ToLower().EndsWith(".udf") ||
                format.ToLower().EndsWith(".wim") ||
                format.ToLower().EndsWith(".xar") ||
                format.ToLower().EndsWith(".iso") ||
                format.ToLower().EndsWith(".bzip2") ||
                format.ToLower().EndsWith(".z") ||
                format.ToLower().EndsWith(".cab") ||
                format.ToLower().EndsWith(".cso"))
            {
                return archiveIco;
            }
            if (format.ToLower().EndsWith(".apk"))
            {
                return apkIco;
            }
            if (format.ToLower().EndsWith(".doc") ||
                format.ToLower().EndsWith(".docx") ||
                format.ToLower().EndsWith(".dot") ||
                format.ToLower().EndsWith(".wdk") ||
                format.ToLower().EndsWith(".docm") ||
                format.ToLower().EndsWith(".dotx") ||
                format.ToLower().EndsWith(".dotm") ||
                format.ToLower().EndsWith(".docb"))
            {
                return wordIco;
            }
            if (format.ToLower().EndsWith(".xls") ||
                format.ToLower().EndsWith(".xlt") ||
                format.ToLower().EndsWith(".xlm") ||
                format.ToLower().EndsWith(".xlsx") ||
                format.ToLower().EndsWith(".xlsm") ||
                format.ToLower().EndsWith(".xltx") ||
                format.ToLower().EndsWith(".xltm") ||
                format.ToLower().EndsWith(".xlsb") ||
                format.ToLower().EndsWith(".xla") ||
                format.ToLower().EndsWith(".xlam") ||
                format.ToLower().EndsWith(".xll") ||
                format.ToLower().EndsWith(".xlw"))
            {
                return excelIco;
            }
            if (format.ToLower().EndsWith(".ppt") ||
                format.ToLower().EndsWith(".pot") ||
                format.ToLower().EndsWith(".pps") ||
                format.ToLower().EndsWith(".pptx") ||
                format.ToLower().EndsWith(".pptm") ||
                format.ToLower().EndsWith(".potx") ||
                format.ToLower().EndsWith(".potm") ||
                format.ToLower().EndsWith(".ppam") ||
                format.ToLower().EndsWith(".ppsx") ||
                format.ToLower().EndsWith(".ppsm") ||
                format.ToLower().EndsWith(".sldx") ||
                format.ToLower().EndsWith(".sldm"))
            {
                return powerPointIco;
            }
            if (format.ToLower().EndsWith(".accdb") ||
                format.ToLower().EndsWith(".accde") ||
                format.ToLower().EndsWith(".accdt") ||
                format.ToLower().EndsWith(".accdr") ||
                format.ToLower().EndsWith(".adn") ||
                format.ToLower().EndsWith(".accda") ||
                format.ToLower().EndsWith(".mdw") ||
                format.ToLower().EndsWith(".mam") ||
                format.ToLower().EndsWith(".maq") ||
                format.ToLower().EndsWith(".mar") ||
                format.ToLower().EndsWith(".mat") ||
                format.ToLower().EndsWith(".maf") ||
                format.ToLower().EndsWith(".laccdb") ||
                format.ToLower().EndsWith(".ade") ||
                format.ToLower().EndsWith(".adp") ||
                format.ToLower().EndsWith(".mdb") ||
                format.ToLower().EndsWith(".cdb") ||
                format.ToLower().EndsWith(".mda") ||
                format.ToLower().EndsWith(".mdn") ||
                format.ToLower().EndsWith(".mdt") ||
                format.ToLower().EndsWith(".mdf") ||
                format.ToLower().EndsWith(".mde") ||
                format.ToLower().EndsWith(".ldb"))
            {
                return accessIco;
            }
            if (format.ToLower().EndsWith(".txt"))
            {
                return txtIco;
            }
            if (format.ToLower().EndsWith(".pdf"))
            {
                return pdfIco;
            }
            if (format.ToLower().EndsWith(".exe"))
            {
                return exeIco;
            }
            if (format.ToLower().EndsWith(".psd"))
            {
                return psdIco;
            }
            if (format.ToLower().EndsWith(".psb"))
            {
                return psbIco;
            }
            if (format.ToLower().EndsWith(".cs"))
            {
                return csIco;
            }
            if (format.ToLower().EndsWith(".djvu"))
            {
                return djvuIco;
            }
            if (format.ToLower().EndsWith(".bin"))
            {
                return binIco;
            }
            if (format.ToLower().EndsWith(".xaml"))
            {
                return xamlIco;
            }
            return fileIco;
        }

        public static void OpenFile(string fileName)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(fileName);
            p.Start();
        }
    }
}
