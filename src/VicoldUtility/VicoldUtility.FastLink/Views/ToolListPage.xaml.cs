﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
            (((char)0xEC37).ToString(), "#B22222"),
            (((char)0xEC38).ToString(), "#FF8C00"),
            (((char)0xEC39).ToString(), "#FFD700"),
            (((char)0xEC3A).ToString(), "#7FFF00"),
            (((char)0xEC3B).ToString(), "#008000")
        };
        private ObservableCollection<ListDataEtt> _ettLists;
        private bool _isReflushSignalLoopFlag = true;
        public ToolListPage()
        {
            InitializeComponent();
            InitData();
        }

        private async void InitData()
        {
            var configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\LinkSource.xml");
            var sourceConfig = await XMLUtil.LoadXMLToAsync<SourceConfigEtt>(configPath).ConfigureAwait(false);

            _ettLists = new ObservableCollection<ListDataEtt>();
            foreach (var group in sourceConfig.Groups)
            {
                foreach (var link in group.Links)
                {
                    _ettLists.Add(new ListDataEtt()
                    {
                        Display = link.Display,
                        Tint = link.Tint,
                        Url = link.Url,
                        TagColor = GetLinkTypeColor(link.Url),
                    });
                }
            }
            lbLinkList.ItemsSource = _ettLists;
        }

        private string GetLinkTypeColor(string url)
        {
            var color = "#fff";
            if (url.StartsWith("http"))
            {
                color = "#1E90FF";
            }
            else if (url.StartsWith(@"\\"))
            {
                color = "#228B22";
            }
            return color;
        }
        private void lbLinkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ett = lbLinkList.SelectedItem as ListDataEtt;
            Process.Start(ett.Url);
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
        internal void OnWindowClose()
        {
            _isReflushSignalLoopFlag = false;
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
            }),null);
        }
    }
}
