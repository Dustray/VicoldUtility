using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
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
using VicoldGis.Adapters;
using VicoldGis.Algorithms.Entities;
using VicoldGis.VMap.Projections;
using VicoldGis.VMap.Symbols;

namespace VicoldGis.Pages
{
    /// <summary>
    /// ToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolPage : Page
    {
        public ToolPage()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择数据源文件";
            openFileDialog.Filter = "source文件|*.source";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "source";
            if (openFileDialog.ShowDialog() == true)
            {
                string txtFile = openFileDialog.FileName;
                Task.Run(() =>
                {
                    LoadToMap(txtFile);
                });
            }
        }


        private void CbOffset_Checked(object sender, RoutedEventArgs e)
        {
            var layer = App.Current.Map2.Manager.GetLayer(12138);
            if (layer != null)
            {
                var data = layer.DataSource as TempAdapter;
                data.IsCrossoverAutoOffset = (bool)CbOffset.IsChecked;
                App.Current.Map2.Manager.Update(layer);
            }
        }


        private void LoadToMap(string file)
        {
            var adapter = new TempAdapter(file);
            adapter.IsCrossoverAutoOffset = false;
            this.Dispatcher.Invoke(() =>
            {
                var layer = new VMap.Layer();
                layer.Id = 12138;
                layer.DataSource = adapter;
                var oldlayer = App.Current.Map2.Manager.GetLayer(layer.Id);
                if (oldlayer != null)
                {
                    App.Current.Map2.Manager.Delete(layer.Id);
                }
                App.Current.Map2.Manager.Add(layer);
            });
        }

        private void SldLineWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (App.Current.Map2 == null)
            {
                return;
            }
            var layer = App.Current.Map2.Manager.GetLayer(12138);
            if (layer != null)
            {
                var data = layer.DataSource as TempAdapter;
                data.LineWidth = (int)SldLineWidth.Value;
                App.Current.Map2.Manager.Update(layer);
            }
        }

        private void SldSmoothCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (App.Current.Map2 == null)
            {
                return;
            }
            var layer = App.Current.Map2.Manager.GetLayer(12138);
            if (layer != null)
            {
                var data = layer.DataSource as TempAdapter;
                data.SmoothCount = (byte)SldSmoothCount.Value;
                App.Current.Map2.Manager.Update(layer);
            }
        }
    }
}
