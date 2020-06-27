using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using VicoldUtility.FastLink.Entities;
using VicoldUtility.FastLink.Utilities;

namespace VicoldUtility.FastLink.Views
{
    /// <summary>
    /// ToolListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolListPage : Page
    {
        public ToolListPage()
        {
            InitializeComponent();
            InitData();
        }

        private async void InitData()
        {
            var configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\LinkSource.xml");
            var sourceConfig = await XMLUtil.LoadXMLToAsync<SourceConfigEtt>(configPath).ConfigureAwait(false);

            var ettLists = new ObservableCollection<ListDataEtt>();
            foreach (var group in sourceConfig.Groups)
            {
                foreach (var link in group.Links)
                {
                    ettLists.Add(new ListDataEtt()
                    {
                        Display = link.Display,
                        Tint = link.Tint,
                        Url = link.Url,
                    });
                }
            }
            lbLinkList.ItemsSource = ettLists;
        }

        private void lbLinkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ett = lbLinkList.SelectedItem as ListDataEtt;
            Process.Start(ett.Url);
        }
    }
}
