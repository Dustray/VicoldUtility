﻿using Newtonsoft.Json;
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
    /// 主List页面
    /// </summary>
    public partial class ToolListPage : Page
    {
        /// <summary>
        /// 信号颜色列表
        /// </summary>
        private (string, string)[] signalColors = new (string, string)[] {
            (((char)0xE871).ToString(), "#DCDCDC"),
            (((char)0xEC37).ToString(), "#D22222"),
            (((char)0xEC38).ToString(), "#FF8C00"),
            (((char)0xEC39).ToString(), "#FFD700"),
            (((char)0xEC3A).ToString(), "#7FFF00"),
            (((char)0xEC3B).ToString(), "#008000")
        };
        /// <summary>
        /// 连接类型图标
        /// </summary>
        private string[] _linkTypeChars = new string[] { ((char)0xED25).ToString(), ((char)0xE128).ToString(), ((char)0xEC50).ToString() };
        /// <summary>
        /// 分组颜色标识
        /// </summary>
        private string[] _linkGroupColors = new string[] { "#1E90FF", "#228B22", "#FFC107", "#D32F2F", "#00796B", "#512DA8", "#FF5722" };
        /// <summary>
        /// 分组实体类
        /// </summary>
        private ObservableCollection<ListDataEtt> _ettLists;
        /// <summary>
        /// 刷新信号量循环标识
        /// </summary>
        private bool _isReflushSignalLoopFlag = true;
        /// <summary>
        /// 子菜单窗体队列
        /// </summary>
        private Queue<PopupListWindow> _childWindowQueue;
        /// <summary>
        /// 源配置实体
        /// </summary>
        private SourceConfigEtt _sourceConfigEtt;
        /// <summary>
        /// 打开菜单回调事件
        /// </summary>
        private Action<bool> _isOpeningChildFolderCallback;

        /// <summary>
        /// 主List页面
        /// </summary>
        /// <param name="isOpeningChildFolderCallback"></param>
        public ToolListPage(Action<bool> isOpeningChildFolderCallback, string configPath)
        {
            InitializeComponent();
            InitData(configPath);
            _isOpeningChildFolderCallback = isOpeningChildFolderCallback;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public async void InitData(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath)) return;
            try
            {
                _sourceConfigEtt = await XMLUtil.LoadXMLToAsync<SourceConfigEtt>(configPath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Alert.Show("配置文件读取错误", "不是正确的配置格式。", AlertTheme.Error);
            }
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

        /// <summary>
        /// 根据链接获取对应类型图标
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
        #region 主窗体响应方法

        /// <summary>
        /// 窗体显示时
        /// </summary>
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
        /// <summary>
        /// 窗体隐藏时
        /// </summary>
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

        /// <summary>
        /// 链接打开时
        /// </summary>
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
        /// <summary>
        /// 窗体关闭时
        /// </summary>
        internal void OnWindowClose()
        {
            _isReflushSignalLoopFlag = false;
            _childWindowQueue?.Clear();
            _childWindowQueue = null;
        }

        #endregion

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

        /// <summary>
        /// 链接列表鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbLinkList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisBorder = sender as Border;
            var ett = thisBorder.DataContext as ListDataEtt;
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    //Process.Start(ett.Url);
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.FileName = ett.Url;
                    p.StartInfo.WorkingDirectory = "C:\\";
                    p.Start();
                    p.Dispose();
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
                //if (pointPosition.X > 1200)
                //{
                //    pointPosition.X -= this.WindowWidth-12;
                //}
                var linkEtt = _sourceConfigEtt.FindLink(ett.ID);
                if (null == linkEtt.Links)
                {
                    if (!ett.Url.StartsWith("http") && !ett.Url.StartsWith(@"\\"))
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
                        window.Owner = Window.GetWindow(this);
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
                    window.Owner = Window.GetWindow(this);
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
