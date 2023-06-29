using FileHeadFormatter.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileHeadFormatter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _lockEditor = false;
        private FileWatcher _fileWatcher;
        private UserCfgEtt _userCfg;
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            _userCfg = UserCfgEtt.Load();
            _fileWatcher = new FileWatcher(OnFileUpdate);
            _fileWatcher.EnableCreated = true;
            this.AddHandler(MainWindow.DragEnterEvent, new DragEventHandler(Window_DragOver), true);
            this.AddHandler(MainWindow.DragLeaveEvent, new DragEventHandler(Window_DragLeave), true);
            this.AddHandler(MainWindow.DropEvent, new DragEventHandler(Window_Drop), true);

            WatchPathText.Text = _userCfg.WatcherPath;
            WatchFilterText.Text = _userCfg.WatcherFilter;
            // 获取绝对路径
            ResetEditor();
            ReflushWatcher();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _fileWatcher.Dispose();
        }

        #region 文件拖拽处理

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            MaskGrid.Visibility = Visibility.Collapsed;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
                MaskGrid.Visibility = Visibility.Visible;
                e.Handled = false;
            }
            else
            {
                e.Effects = DragDropEffects.Link;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    string? fileName = e.Data.GetData(DataFormats.Text).ToString();
                    if (fileName is { })
                    {
                        TemplateTool.Process(fileName, TemplateText.Text, ParameterText.Text);
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string fileName in fileNames)
                    {
                        TemplateTool.Process(fileName, TemplateText.Text, ParameterText.Text);
                    }
                }
                MaskGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MaskGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LockBtn_Click(object sender, RoutedEventArgs e)
        {
            _lockEditor = !_lockEditor;
            LockBtn.Content = $"编辑锁定：{(_lockEditor ? "关" : "开")}";
            TemplateText.IsEnabled = _lockEditor;
            ParameterText.IsEnabled = _lockEditor;
            WatchPathText.IsEnabled = _lockEditor;
            WatchFilterText.IsEnabled = _lockEditor;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, @"data/template.txt"), TemplateText.Text);
            File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, @"data/parameter.txt"), ParameterText.Text);
            _userCfg.WatcherPath = WatchPathText.Text;
            _userCfg.WatcherFilter = WatchFilterText.Text;
            _userCfg.Save();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetEditor();
        }

        private void ResetEditor()
        {
            TemplateText.Text = File.ReadAllText(System.IO.Path.Combine(Environment.CurrentDirectory, @"data/template.txt"));
            ParameterText.Text = File.ReadAllText(System.IO.Path.Combine(Environment.CurrentDirectory, @"data/parameter.txt"));
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ctrl v
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                try
                {
                    string pastingText = Clipboard.GetText(TextDataFormat.Text);
                    if (File.Exists(pastingText))
                    {
                        TemplateTool.Process(pastingText, TemplateText.Text, ParameterText.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Error");
                }
            }
        }

        #endregion

        #region 文件监控

        private void OnFileUpdate(string fileName)
        {
            Dispatcher.Invoke(() =>
            {
                TemplateTool.Process(fileName, TemplateText.Text, ParameterText.Text, false);
            });
        }

        private void DirCBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                return;
            }

            if (DirCBox.IsChecked == true)
            {
                ReflushWatcher();
            }
            else
            {
                _fileWatcher.Stop();
            }
        }

        private void ReflushWatcher()
        {
            if (WatchPathText.Text is { })
            {
                _fileWatcher.Filter = WatchFilterText.Text;
                _fileWatcher.Restart(WatchPathText.Text);
            }


        }

        #endregion 

    }
}
