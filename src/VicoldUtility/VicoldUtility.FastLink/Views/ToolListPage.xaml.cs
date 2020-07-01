using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vicold.Popup;
using VicoldUtility.FastLink.Entities;
using VicoldUtility.FastLink.Utilities;

namespace VicoldUtility.FastLink.Views
{
    /// <summary>
    /// ToolListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolListPage : Page
    {
        private (string, string)[] signalColors = new (string, string)[] {
            (((char)0xE871).ToString(), "#DCDCDC"),
            (((char)0xEC37).ToString(), "#D22222"),
            (((char)0xEC38).ToString(), "#FF8C00"),
            (((char)0xEC39).ToString(), "#FFD700"),
            (((char)0xEC3A).ToString(), "#7FFF00"),
            (((char)0xEC3B).ToString(), "#008000")
        };

        private string[] _linkTypeChars = new string[] { ((char)0xED25).ToString(), ((char)0xE128).ToString(), ((char)0xEC50).ToString() };
        private string[] _linkGroupColors = new string[] { "#1E90FF", "#228B22", "#FFC107", "#D32F2F", "#00796B", "#512DA8", "#FF5722" };
        private ObservableCollection<ListDataEtt> _ettLists;
        private bool _isReflushSignalLoopFlag = true;
        private Queue<PopupListWindow> _childWindowQueue;
        private SourceConfigEtt _sourceConfigEtt;
        private Action<bool> _isOpeningChildFolderCallback;

        public ToolListPage(Action<bool> isOpeningChildFolderCallback)
        {
            InitializeComponent();
            InitData();
            _isOpeningChildFolderCallback = isOpeningChildFolderCallback;
        }

        private async void InitData()
        {
            var configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\LinkSource.xml");
            _sourceConfigEtt = await XMLUtil.LoadXMLToAsync<SourceConfigEtt>(configPath).ConfigureAwait(false);
            _childWindowQueue = new Queue<PopupListWindow>();
            _ettLists = new ObservableCollection<ListDataEtt>();
            var colorIndex = 0;
            foreach (var group in _sourceConfigEtt.Groups)
            {
                if (colorIndex == _linkGroupColors.Length) colorIndex = 0;
                foreach (var link in group.Links)
                {
                    _ettLists.Add(new ListDataEtt()
                    {
                        ID = link.ID,
                        Display = link.Display,
                        Tint = link.Tint,
                        Url = link.Url,
                        TagColor = _linkGroupColors[colorIndex],
                        LinkTypeIcon = GetLinkTypeChar(link.Url),
                        LinkTypeContent = group.Display,
                    });
                }
                colorIndex++;
            }
            lbLinkList.ItemsSource = _ettLists;
        }

        private string GetLinkTypeChar(string url)
        {
            var icon = _linkTypeChars[0];
            if (url.StartsWith("http"))
            {
                icon = _linkTypeChars[1];
            }
            else if (url.StartsWith(@"\\"))
            {
                icon = _linkTypeChars[0];
            }
            else
            {
                icon = _linkTypeChars[2];
            }
            return icon;
        }
        private void lbLinkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var ett = lbLinkList.SelectedItem as ListDataEtt;
            //try
            //{
            //    Process.Start(ett.Url);
            //}
            //catch { }
        }

        internal void OnWindowShow()
        {
            _isReflushSignalLoopFlag = true;
            Task.Run(() =>
            {
                Parallel.For(0, _ettLists.Count, async (index) =>
                {
                    //var pin = GCHandle.ToIntPtr(GCHandle.Alloc(listDataEtt));
                    var ping = new Ping();
                    while (_isReflushSignalLoopFlag)
                    {
                        ReflushSignal(index, ping);
                        await Task.Delay(1000);
                    }
                });
            });
        }
        internal void OnWindowHide()
        {
            _isReflushSignalLoopFlag = false;
            if (null != _childWindowQueue)
            {
                while (_childWindowQueue.Count > 0)
                {
                    var win = _childWindowQueue.Dequeue();
                    win.Close();
                }
            }
        }

        internal void OnLinkOpened()
        {
            if (null != _childWindowQueue)
            {
                while (_childWindowQueue.Count > 0)
                {
                    var win = _childWindowQueue.Dequeue();
                    win.Close();
                }
            }
        }
        internal void OnWindowClose()
        {
            _childWindowQueue?.Clear();
            _childWindowQueue = null;
        }
        /// <summary>
        /// 刷新信号量
        /// </summary>
        /// <param name="listDataEtt"></param>
        /// <param name="ping"></param>
        private void ReflushSignal(int index, Ping ping)
        {
            var listDataEtt = _ettLists[index];
            var address = listDataEtt.Url;
            if (address.StartsWith(@"\\"))
            {
                address = address.Replace(@"\\", "");
                address = address.Split('\\')[0];
            }
            else if (address.StartsWith(@"http"))
            {
                address = address.Replace(@"http://", "");
                address = address.Replace(@"https://", "");
                address = address.Split('/')[0];
                address = address.Split(':')[0];
            }
            else
            {
                return;
            }
            PingReply p = null;
            var time = 6666L;
            try
            {
                p = ping.Send(address);
            }
            catch
            {
            }

            var signalIndex = 1;
            if (p != null && p.Status == IPStatus.Success)
            {
                time = p.RoundtripTime;
                //成功的情况下
                if (time < 61)
                {
                    signalIndex = 5;
                }
                else if (time < 200)
                {
                    signalIndex = 4;
                }
                else if (time < 600)
                {
                    signalIndex = 3;
                }
                else if (time < 3000)
                {
                    signalIndex = 2;
                }
                else if (time >= 3000)
                {
                    signalIndex = 1;
                }
            }

            lbLinkList.Dispatcher.BeginInvoke(new Action(() =>
            {
                listDataEtt.SignalContent = signalColors[signalIndex].Item1;
                listDataEtt.SignalColor = signalColors[signalIndex].Item2;
                listDataEtt.SignalTime = $"{time}ms";
            }), null);
        }

        private void lbLinkList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisBorder = sender as Border;
            var ett = thisBorder.DataContext as ListDataEtt;
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    Process.Start(ett.Url);
                }
                catch (Exception ex)
                {
                    Alert.Show("错误", ex.Message.ToString(), AlertTheme.Error);
                }
            }
            else
            {
                Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
                var pointPosition = (e.Source as FrameworkElement).PointToScreen(pp);
                var linkEtt = _sourceConfigEtt.FindLink(ett.ID);
                if (null == linkEtt.Links)
                {
                    if (!ett.Url.StartsWith("http"))
                    {
                        if (!Directory.Exists(ett.Url))
                        {
                            Alert.Show("错误", "找不到路径", AlertTheme.Error);
                            return;
                        }
                        var root = new DirectoryInfo(ett.Url);
                        var files = root.GetDirectories();
                        if (0 == files.Length) return;
                        var ettLists = new List<SourceConfigLinkEtt>();
                        foreach (var dir in files)
                        {
                            ettLists.Add(new SourceConfigLinkEtt()
                            {
                                Display = dir.Name,
                                Url = dir.FullName,
                                Tint = dir.FullName,
                            });
                        }
                        _isOpeningChildFolderCallback?.Invoke(true);
                        var window = new PopupListWindow(ettLists, pointPosition.X, pointPosition.Y, _childWindowQueue, OnLinkOpened);
                        window.OnWindowClosed = () =>
                        {
                            //_isHasChildFolder = false;
                            _isOpeningChildFolderCallback?.Invoke(false);
                        };
                        _childWindowQueue.Enqueue(window);
                        window.Show();
                    }
                }
                else
                {
                    _isOpeningChildFolderCallback?.Invoke(true);
                    var window = new PopupListWindow(linkEtt.Links, pointPosition.X, pointPosition.Y, _childWindowQueue, OnLinkOpened);
                    window.OnWindowClosed = () =>
                    {
                        _isOpeningChildFolderCallback?.Invoke(false);
                    };
                    _childWindowQueue.Enqueue(window);
                    window.Show();
                }
            }
        }
    }
}
