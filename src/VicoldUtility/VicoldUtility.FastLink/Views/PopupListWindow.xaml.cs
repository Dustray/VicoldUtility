using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vicold.Popup;
using VicoldUtility.FastLink.Entities;

namespace VicoldUtility.FastLink.Views
{
    /// <summary>
    /// PopupListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupListWindow : Window
    {
        private IList<SourceConfigLinkEtt> _ettLists;
        private string _rootUrl;
        private bool _isHasChildFolder = false;

        public Action OnWindowClosed;
        private Action _onLinkOpened;
        private Queue<PopupListWindow> _windowQueue;
        private bool _beginClose = false;
        public PopupListWindow(IList<SourceConfigLinkEtt> list, double x, double y, Queue<PopupListWindow> windowQueue, Action onLinkOpened)
        {
            InitializeComponent();
            _ettLists = list;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Top = y - 6;
            Left = x - 6;
            _windowQueue = windowQueue;
            _onLinkOpened = onLinkOpened;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            lbLinkList.ItemsSource = _ettLists;

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _beginClose = true;
            OnWindowClosed?.Invoke();

        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isHasChildFolder && !_beginClose)
                Close();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisBorder = sender as Border;
            var ett = thisBorder.DataContext as SourceConfigLinkEtt;
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    Process.Start(ett.Url);
                    _onLinkOpened?.Invoke();
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
                if (null == ett.Links)
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
                        _isHasChildFolder = true;
                        var window = new PopupListWindow(ettLists, pointPosition.X, pointPosition.Y, _windowQueue, _onLinkOpened);
                        window.OnWindowClosed = () =>
                        {
                            _isHasChildFolder = false;
                        };
                        _windowQueue.Enqueue(window);
                        window.Show();
                    }
                }
                else
                {
                    _isHasChildFolder = true;
                    var window = new PopupListWindow(ett.Links, pointPosition.X, pointPosition.Y, _windowQueue, _onLinkOpened);
                    window.OnWindowClosed = () =>
                    {
                        _isHasChildFolder = false;
                    };
                    _windowQueue.Enqueue(window);
                    window.Show();
                }
            }
        }
    }
}
