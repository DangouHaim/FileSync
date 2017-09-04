using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

namespace FileSync
{
    /*
     * ===BUGS===
     * 
     * memory checking
     * fix filesending
     * rename
     * folder upload 
     * folder/file creating/rename (on android app)
     * download list
     * upload list
     * gif support
     * remote apk installer
     * total search
     * sorting by size and type
     * folder sizes
     * preview zone
     * cache clean
     * pdf icons
     * закладки
     * tree
     * 
     */
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IPAddress remoteIP = IPAddress.Any;

        private List<string> _downloadList = new List<string>();
        private List<string> _uploadList = new List<string>();
        private List<FileItem> _filesDrop = new List<FileItem>();
        private ObservableCollection<FileItem> _files = new ObservableCollection<FileItem>();

        private bool setLast = false;
        private char currentDevice = 'z';
        private int itemFirstClickIndex = -1;
        private object LOCK = new object();
        private object FLLOCK = new object();
        private string last = "";
        private string cacheDir = Directory.GetCurrentDirectory() + "\\cache\\";

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitIcons();
            FileList.ItemsSource = _files;
            Server();
        }

        private void InitIcons()
        {
            Image Root = new Image();
            Image Sd = new Image();
            Image Back = new Image();

            Root.Source = R.homeIco;
            Sd.Source = R.sdIco;
            Back.Source = R.backIco;

            RootBtn.Content = Root;
            SdcardBtn.Content = Sd;
            BackBtn.Content = Back;
            
        }

        private void CreateCacheDirectory()
        {
            if(!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
        }


        bool deviceExists = false;
        private void SetVirtualDevice()
        {
            if(!deviceExists)
            {
                try
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\DeviceLog");
                }
                catch { }
                CreateCacheDirectory();
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "cmd /c subst " + currentDevice.ToString() + ": \"" + cacheDir.Remove(cacheDir.Length - 1) + "\" >> \"" +
                    Directory.GetCurrentDirectory() + "\\DeviceLog\"";
                process.StartInfo = startInfo;
                process.Start();
                Thread.Sleep(500);
                if (new FileInfo(Directory.GetCurrentDirectory() + "\\DeviceLog").Length > 0)
                {
                    currentDevice--;
                    SetVirtualDevice();
                }
                else
                {
                    deviceExists = true;
                }
            }
        }

        private void RemoveVirtualDevice()
        {
            if(deviceExists)
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "cmd /c subst " + currentDevice.ToString() + ": /D";
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        private void Refrash(string refrashAddress)
        {
            if (string.IsNullOrWhiteSpace(Address.Text))
            {
                Address.Text = "/sdcard";
            }
            if (Address.Text == refrashAddress)
            {
                Request("[command]dir:" + Address.Text);
            }
        }

        private void Request(string requset, bool open = false)
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    if (remoteIP != IPAddress.Any)
                    {
                        string[] data = requset.Split('*');
                        Message(data[0], true);
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(data[1]))
                            {
                                data[0] += ":" + data[1].Split('\\')[data[1].Split('\\').Length - 1];
                            }
                        }
                        catch { }

                        if (data[0].IndexOf("[command]download:") > -1)
                        {
                            Thread load = new Thread(() =>
                            {
                                lock (LOCK)
                                {
                                    TcpClient c = new TcpClient(remoteIP.ToString(), 25566);
                                    StreamWriter sw = new StreamWriter(c.GetStream());
                                    sw.WriteLine(data[0]);
                                    sw.Close();
                                    _downloadList.Add(data[0].Split(':')[1]);
                                    DownloadFile(data[0], data[1], open);
                                    _downloadList.Remove(data[0]);
                                }
                            });
                            load.IsBackground = true;
                            load.Start();
                        }
                        else
                            if (data[0].IndexOf("[command]upload:") > -1)
                            {
                                Thread load = new Thread(() =>
                                {
                                    lock (LOCK)
                                    {
                                        TcpClient c = new TcpClient(remoteIP.ToString(), 25566);
                                        StreamWriter sw = new StreamWriter(c.GetStream());
                                        sw.WriteLine(data[0]);
                                        sw.Close();
                                        _uploadList.Add(data[1]);
                                        UploadFile(data[1]);
                                        _uploadList.Remove(data[1]);
                                        Dispatcher.Invoke(() =>
                                        {
                                            Refrash(data[0].Split(':')[1]);
                                        });
                                    }
                                });
                                load.IsBackground = true;
                                load.Start();
                            }
                            else
                            {
                                TcpClient c = new TcpClient(remoteIP.ToString(), 25566);
                                StreamWriter sw = new StreamWriter(c.GetStream());
                                sw.WriteLine(data[0]);
                                sw.Close();
                            }
                    }
                }
                catch (Exception ex)
                {
                    Message(ex.Message, true);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private void DownloadFile(string s, string path, bool open)
        {
            try
            {
                TcpListener l = new TcpListener(IPAddress.Any, 25567);
                l.Start();
                try
                {
                    string name = s.Split(':')[1];
                    name = name.Split('/')[name.Split('/').Length - 1];

                    Message("downloading : " + name, true);


                    TcpClient client = l.AcceptTcpClient();
                    Message("downloading start : " + name, true);

                    using (FileStream file = new FileStream(path + "\\" + name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        client.GetStream().CopyTo(file);
                    }
                    client.Close();
                    l.Stop();
                    Message("downloading end : " + name, true);
                    if (open)
                    {
                        R.OpenFile(path + "\\" + name);
                    }
                }
                catch (Exception ex)
                {
                    Message(ex.Message, true);
                }
                finally
                {
                    l.Stop();
                }
            }
            catch { }
        }

        private void UploadFile(string data)
        {
            try
            {
                TcpListener l = new TcpListener(IPAddress.Any, 25568);
                l.Start();

                try
                {
                    Message("uploading...", true);
                    string name = data;
                    TcpClient client = l.AcceptTcpClient();
                    Message("uploading start...", true);

                    using (FileStream file = new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        file.CopyTo(client.GetStream());
                    }
                    client.Close();
                    l.Stop();
                    Message("uploading end", true);
                }
                catch (Exception except)
                {
                    Message(except.Message, true);
                }
                finally
                {
                    l.Stop();
                }
            }
            catch { }
        }

        private void Server()
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    Message(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), true);
                    TcpListener server = new TcpListener(IPAddress.Any, 25565);
                    server.Start();
                    int f = 0;
                    while (true)
                    {
                        using (TcpClient c = server.AcceptTcpClient())
                        {
                            using (StreamReader sr = new StreamReader(c.GetStream()))
                            {
                                string s = sr.ReadToEnd();
                                if (f == 0)
                                {
                                    f++;
                                    IPAddress.TryParse(s, out remoteIP);
                                    Dispatcher.Invoke(() =>
                                    {
                                        Refrash("/sdcard");
                                    });
                                    SetVirtualDevice();
                                }
                                if (s.IndexOf("[filelist]") > -1)
                                {
                                    lock(FLLOCK)
                                    {
                                        GetFiles(s);
                                    }
                                }
                                else
                                if (s.IndexOf("[downloadfilelist]") > -1)
                                {
                                    lock(FLLOCK)
                                    {
                                        DownloadFiles(s);
                                    }
                                }
                                else
                                {
                                    Message(s, true);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message(ex.Message, true);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private void Message(string s, bool invoke = false)
        {
            if(invoke)
            {
                Dispatcher.Invoke(() =>
                {
                    Status.Content = s;
                });
            }
            else
            {
                Status.Content = s;
            }
        }

        public void SetAddress(string path, bool invoke)
        {
            if(invoke)
            {
                Dispatcher.Invoke(() =>
                {
                    Address.Text = path;
                });
            }
            else
            {
                Address.Text = path;
            }
        }

        private void GetFiles(string s)
        {
            string[] fdata = s.Split(new string[] { "[filelist]" }, StringSplitOptions.None);

            Dispatcher.Invoke(() =>
            {
                _files.Clear();
            });
            SetAddress(fdata[0], true);


            if (!string.IsNullOrWhiteSpace(fdata[1]))
            {
                string[] files = fdata[1].Trim().Split('\n');
                List<FileItem> flist = new List<FileItem>();
                foreach (string f in files)
                {
                    bool isFile = f.Split(':')[0].Trim().IndexOf("file") > -1;
                    string path = "";
                    string size = "";
                    if (isFile)
                    {
                        path = f.Split(':')[3].Trim();
                        size = f.Split(':')[2].Trim();
                    }
                    else
                    {
                        path = f.Split(':')[1].Trim();
                    }
                    FileItem i = new FileItem()
                    {
                        Name = path.Split('/')[path.Split('/').Length - 1],
                        FullPath = path,
                        IsFile = isFile,
                    };
                    if (!i.IsFile)
                    {
                        i.IsCached = CacheDirectoryExists(i);
                        i.ItemIcon = R.folderIco;
                        flist.Insert(0, i);
                    }
                    else
                    {
                        i.IsCached = CacheExists(i);
                        i.ItemIcon = R.GetIconByFormat(i.Name);
                        i.Size = size;
                        flist.Add(i);
                    }
                }

                flist = SortFileList(flist);
                foreach (var v in flist)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _files.Add(v);
                    });
                }
                SetLast(true);
            }
            Dispatcher.Invoke(() =>
            {
                if (Search.Text != "")
                {
                    Search.Text = "";
                }
                if (FileList.ItemsSource != _files)
                {
                    FileList.ItemsSource = _files;
                }
            });
        }

        private void DownloadFiles(string s)
        {
            string[] fdata = s.Split(new string[] { "[downloadfilelist]" }, StringSplitOptions.None);


            if (!string.IsNullOrWhiteSpace(fdata[1]))
            {
                string[] files = fdata[1].Trim().Split('\n');
                List<FileItem> flist = new List<FileItem>();
                foreach (string f in files)
                {
                    bool isFile = f.Split(':')[0].Trim().IndexOf("file") > -1;
                    string path = "";
                    string size = "";
                    if (isFile)
                    {
                        path = f.Split(':')[3].Trim();
                        size = f.Split(':')[2].Trim();
                    }
                    else
                    {
                        path = f.Split(':')[1].Trim();
                    }
                    FileItem i = new FileItem()
                    {
                        Name = path.Split('/')[path.Split('/').Length - 1],
                        FullPath = path,
                        IsFile = isFile,
                    };
                    if (!i.IsFile)
                    {
                        i.IsCached = CacheDirectoryExists(i);
                        i.ItemIcon = R.folderIco;
                        flist.Insert(0, i);
                    }
                    else
                    {
                        i.IsCached = CacheExists(i);
                        i.ItemIcon = R.GetIconByFormat(i.Name);
                        i.Size = size;
                        flist.Add(i);
                    }
                }

                CacheFiles(flist.ToArray(), true);
            }
        }

        private void SetLast(bool invoke = false)
        {
            if (setLast)
            {
                setLast = false;
                int idx = FindByAddress(_files.ToArray(), last);
                if (_files.Count > idx)
                {
                    if(invoke)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            FileList.SelectedIndex = idx;
                        });
                    }
                    else
                    {
                        FileList.SelectedIndex = idx;
                    }
                }
            }
        }

        private List<FileItem> SortFileList(List<FileItem> files)
        {
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < files.Count- 1; ++i)
                {
                    if (files[i].Name.CompareTo(files[i + 1].Name) > 0)
                    {
                        var buf = files[i];
                        files[i] = files[i + 1];
                        files[i + 1] = buf;
                        flag = true;
                    }
                }
            }
            List<FileItem> res = new List<FileItem>();
            foreach (var v in files)
            {
                if (!v.IsFile)
                {
                    res.Add(v);
                }
            }
            foreach (var v in files)
            {
                if (v.IsFile)
                {
                    res.Add(v);
                }
            }
            return res;
        }

        private int FindByAddress(FileItem[] items, string address)
        {
            int res = -1;
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i].FullPath.ToLower().Trim() == address.ToLower().Trim())
                {
                    res = i;
                }
            }
            return res;
        }

        private void RootBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Request("[command]root");
            }
            catch { }
        }

        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (itemFirstClickIndex == FileList.SelectedIndex)
            {
                EnterFile();
            }
            itemFirstClickIndex = -1;
        }

        private FileItem Current()
        {
            return FileList.SelectedItem as FileItem;
        }

        private void EnterFile()
        {
            try
            {
                if (!Current().IsFile)
                {
                    Request("[command]dir:" + Current().FullPath);
                }
                else
                {
                    if (!Directory.Exists(cacheDir))
                    {
                        Directory.CreateDirectory(cacheDir);
                    }
                    if (!CacheExists(Current()))
                    {
                        CacheFile(Current(), true);
                    }
                    else
                    {
                        R.OpenFile(cacheDir + Current().GetCachePostfix() + Current().Name);
                    }
                }
            }
            catch { }
        }

        private void SdcardBtn_Click(object sender, RoutedEventArgs e)
        {
            Request("[command]sdcard");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            SetBackAddress();
        }

        private void SetBackAddress()
        {
            setLast = true;
            last = Address.Text;
            Request("[command]dir:" + BackAddress());
        }

        private string BackAddress()
        {
            string s = "";
            string[] tp = Address.Text.Split('/');
            for (int i = 1; i < tp.Length - 1; i++ )
            {
                s += "/" + tp[i];
            }
            if(string.IsNullOrWhiteSpace(s))
            {
                return "/";
            }
            return s;
        }

        private bool posDialog = false;
        private void FileList_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(!posDialog)
                {
                    EnterFile();
                }
                else
                {
                    posDialog = false;
                }
            }
            if(e.Key == Key.Back)
            {
                SetBackAddress();
            }
            if(e.Key == Key.Delete)
            {
                if(FileList.SelectedIndex > -1)
                {
                    posDialog = true;
                    if(MessageBox.Show("Delete element : '" + _files[FileList.SelectedIndex].Name + "'?", "Delete element?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        DeleteFiles(GetSelectedItems());
                    }
                }
            }
        }

        private void DeleteFiles(FileItem[] files)
        {
            int si = FileList.SelectedIndex;
            foreach(var v in files)
            {
                DeleteFile(v);
            }
            FileList.SelectedIndex = si;
        }

        private void DeleteFile(FileItem i)
        {
            Request("[command]delete:" + i.FullPath);
            _files.Remove(i);
        }

        private void Address_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Request("[command]dir:" + Address.Text);
            }
        }


        private void FileList_Drop(object sender, DragEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Address.Text))
            {
                foreach (var v in e.Data.GetData(DataFormats.FileDrop) as string[])
                {
                    Request("[command]upload:" + Address.Text + "*" + v);
                }
            }
        }

        
        private void FileList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _filesDrop.Clear();
            foreach(var v in GetSelectedItems())
            {
                _filesDrop.Add(v);
            }
        }

        private void FileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Request("[command]sdcard");
            }
            if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Request("[command]root");
            }
            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Refrash(Address.Text);
            }
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (FileList.SelectedIndex > -1)
                {
                    CacheFiles(GetSelectedItems());
                }
            }
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if(FileList.SelectedIndex > -1)
                {
                    CacheFiles(GetSelectedItems());
                    Clipboard.SetDataObject(new DataObject(DataFormats.FileDrop, GetSelectedFiles()));
                }
            }

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if(Clipboard.GetDataObject().GetDataPresent(DataFormats.FileDrop))
                {
                    UploadFiles((string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop));
                }
            }
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                FileItem folder = new FileItem()
                {
                    FullPath = Address.Text + "/asd",
                    IsFile = false,
                    Name = "asd",
                };
                CreateOrRename(folder);
                folder.ItemIcon = R.GetIconByFormat(folder.Name);
                Refrash(Address.Text);
            }
        }

        private void UploadFiles(string[] files)
        {
            if(!string.IsNullOrWhiteSpace(Address.Text))
            {
                foreach(var v in files)
                {
                    Request("[command]upload:" + Address.Text + "*" + v);
                }
            }
        }

        private string[] GetSelectedFiles(bool invoke = false)
        {
            List<string> res = new List<string>();
            if (invoke)
            {
                Dispatcher.Invoke(() =>
                {
                    if (FileList.SelectedIndex > -1)
                    {
                        foreach (FileItem v in FileList.SelectedItems)
                        {
                            res.Add(cacheDir +  v.GetCachePostfix() + v.Name);
                        }
                    }
                });
            }
            else
            {
                if (FileList.SelectedIndex > -1)
                {
                    foreach (FileItem v in FileList.SelectedItems)
                    {
                        res.Add(cacheDir + v.GetCachePostfix() + v.Name);
                    }
                }
            }
            return res.ToArray();
        }

        private FileItem[] GetSelectedItems(bool invoke = false)
        {
            List<FileItem> res = new List<FileItem>();
            if(invoke)
            {
                Dispatcher.Invoke(() =>
                {
                    if (FileList.SelectedIndex > -1)
                    {
                        foreach (var v in FileList.SelectedItems)
                        {
                            res.Add(v as FileItem);
                        }
                    }
                });
            }
            else
            {
                if (FileList.SelectedIndex > -1)
                {
                    foreach (var v in FileList.SelectedItems)
                    {
                        res.Add(v as FileItem);
                    }
                }
            }
            return res.ToArray();
        }

        private bool CacheExists(FileItem i)
        {
            if(!i.IsFile)
            {
                return false;
            }
            if (!Directory.Exists(cacheDir + i.GetCachePostfix()))
            {
                return false;
            }
            return File.Exists(cacheDir + i.GetCachePostfix() + i.Name);
        }

        private void CacheFiles(FileItem[] files, bool invoke = false)
        {
            foreach(var v in files)
            {
                if (v.IsFile)
                {
                    CacheFile(v, false, invoke);
                }
                else
                {
                    CacheDirectory(v);
                }
            }
        }

        private void CacheFile(FileItem i, bool open = false, bool invoke = false)
        {
            if(!CacheExists(i))
            {
                Directory.CreateDirectory(cacheDir + i.GetCachePostfix());
                i.IsCached = true;
                if(invoke)
                {
                    Dispatcher.Invoke(() =>
                    {
                        FileList.Items.Refresh();
                    });
                }
                else
                {
                    FileList.Items.Refresh();
                }
                Request("[command]download:" + i.FullPath + "*" + cacheDir + i.GetCachePostfix(), open);
            }
        }

        private bool CacheDirectoryExists(FileItem i)
        {
            if (!Directory.Exists(cacheDir + i.GetCachePostfix()))
            {
                return false;
            }
            return Directory.Exists(cacheDir + i.GetCachePostfix() + i.Name);
        }

        private void CacheDirectory(FileItem i)
        {
            if(!CacheDirectoryExists(i))
            {
                i.IsCached = true;
                Request("[command]download-dir:" + i.FullPath);
            }
        }



        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(Search.Text))
            {
                ObservableCollection<FileItem> temp = new ObservableCollection<FileItem>();
                FileList.ItemsSource = temp;
                string[] factors = Search.Text.Split(' ');
                foreach(var v in _files)
                {
                    bool cenAdd = true;
                    foreach(string s in factors)
                    {
                        if(v.ToString().ToLower().IndexOf(s) < 0)
                        {
                            cenAdd = false;
                        }
                    }
                    if(cenAdd)
                    {
                        temp.Add(v);
                    }
                }
            }
            else
            {
                FileList.ItemsSource = _files;
            }

        }

        private bool cenDrug = false;
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && cenDrug)
            {
                if (_filesDrop.Count > 0)
                {
                    List<string> files = new List<string>();
                    bool err = false;
                    foreach (var v in _filesDrop)
                    {
                        if (v.IsCached)
                        {
                            files.Add(cacheDir + v.GetCachePostfix() + v);
                        }
                        else
                        {
                            err = true;
                            CacheFiles(GetSelectedItems());
                        }
                    }
                    if (!err)
                    {
                        DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, files.ToArray()), DragDropEffects.Copy);
                    }
                    else
                    {
                        DragDrop.DoDragDrop(this, new DataObject(), DragDropEffects.None);
                    }
                }
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            cenDrug = true;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            cenDrug = false;
        }

        private void FileList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(itemFirstClickIndex == -1)
            {
                itemFirstClickIndex = FileList.SelectedIndex;
            }
        }

        private void CreateOrRename(FileItem i, bool rename = false, string to = "")
        {
            foreach (var v in _files)
            {
                if(i.IsFile)
                {
                    if (v.IsFile && v.Name.ToLower() == i.Name.ToLower())
                    {
                        MessageBox.Show("File allready exist");
                        return;
                    }
                }
                else
                {
                    if (!v.IsFile && v.Name.ToLower() == i.Name.ToLower())
                    {
                        MessageBox.Show("Directory allready exist");
                        return;
                    }
                }
            }
            if(rename)
            {
                Request("[command]rename:" + i.Name + ":" + to);
            }
            else
            {
                if (i.IsFile)
                {
                    Request("[command]mkfile:" + i.Name);
                }
                else
                {
                    Request("[command]mkdir:" + i.Name);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveVirtualDevice();
        }
    }
}
