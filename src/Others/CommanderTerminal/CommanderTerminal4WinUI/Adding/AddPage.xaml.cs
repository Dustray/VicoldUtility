// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommanderTerminal.Adding.ViewModels;
using CommanderTerminal.Utilities;
using CommanderTerminalCore.Configuration.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CommanderTerminal.Adding
{
    public enum EditTitle
    {
        Detail,
        AddDetail,
        EditDetail,
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        private HostListItemVM _hostListItemVM;
        private ObservableCollection<HostListItemVM> _hostListItems;
        private bool _isEditMode = false;

        public AddPage()
        {
            this.InitializeComponent();
            _hostListItems = new();
            _hostListItemVM = new();
            SSHHostList.ItemsSource = _hostListItems;

            SetEditEnabled(false);
            InitListData();
        }

        public SSHHostItemConfigEtt? HostItemConfigEtt { get; private set; }

        private void InitListData()
        {
            var config = TerminalCore.Current.HostConfig.GetConfigEtt();
            List<SSHHostItemConfigEtt> hostEtts = config.HostList;
            if (hostEtts is not { })
            {
                return;
            }

            foreach (var host in hostEtts)
            {
                _hostListItems.Add(host.ToVM());
            }
        }

        #region 事件

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UpdateTitle(EditTitle.AddDetail);
            SetEditEnabled(true);
            _hostListItemVM.Clear();
            _hostListItemVM.Name = "New Host";
            HostName.Focus(FocusState.Pointer);
            HostName.SelectAll();
            BindCurrentVM();
            ClearLog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEditGrid())
            {
                return;
            }

            // sync vm list
            HostListItemVM hostListItemVM;
            var configHosts = _hostListItems.Where(v => v.ID == _hostListItemVM.ID);
            if (configHosts.Count() != 0)
            {
                hostListItemVM = configHosts.First();
            }
            else
            {
                hostListItemVM = new HostListItemVM();
                _hostListItems.Add(hostListItemVM);
            }

            hostListItemVM.CopyFrom(_hostListItemVM);

            // sync config
            SyncDataToConfig(hostListItemVM);
            ClearLog();

            ResetEditRegion();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetEditRegion();
        }

        private void ItemEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is HostListItemVM vm)
            {
                _isEditMode = true;
                UpdateTitle(EditTitle.EditDetail);
                SetEditEnabled(true);
                _hostListItemVM.CopyFrom(vm);
                BindCurrentVM();
                ClearLog();

                //select item
                var index = _hostListItems.IndexOf(vm);
                if (index >= 0)
                {
                    SSHHostList.SelectedIndex = index;
                }
            }
        }

        private void SSHHostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isEditMode)
            {
                Log("Please [clear] Edit State first.");
                return;
            }

            if (SSHHostList.SelectedIndex < 0 || SSHHostList.SelectedIndex >= _hostListItems.Count)
            {
                Log("Select Error.", severity: InfoBarSeverity.Error);
                return;
            }

            var vm = _hostListItems[SSHHostList.SelectedIndex];
            var config = TerminalCore.Current.HostConfig.GetConfigEtt();
            HostItemConfigEtt = config.SelectHostItemByID(vm.ID);
            if (HostItemConfigEtt is not { })
            {
                Log("Cannot find host from configuration.", severity: InfoBarSeverity.Error);
            }
        }

        #endregion

        #region tool

        private static void SyncDataToConfig(HostListItemVM hostListItemVM)
        {
            var config = TerminalCore.Current.HostConfig.GetConfigEtt();
            SSHHostItemConfigEtt? configHost = config.SelectHostItemByID(hostListItemVM.ID);
            if (configHost is not { })
            {
                configHost = new SSHHostItemConfigEtt();
                config.HostList.Add(configHost);
            }

            configHost.Name = hostListItemVM.Name;
            configHost.Host = hostListItemVM.Host;
            configHost.Port = hostListItemVM.Port;
            configHost.RememberedPasswd = hostListItemVM.Password;

            config.Save();
        }

        private void BindCurrentVM()
        {
            //(this.Content as FrameworkElement).DataContext = _hostListItemVM;
            //_hostListItemVM.
        }

        private void Log(string message, string? title = null, InfoBarSeverity severity = InfoBarSeverity.Informational)
        {
            LogInfo.IsOpen = true;
            LogInfo.Message = message;
            LogInfo.Title = title;
            LogInfo.Severity = severity;
        }

        private void ClearLog()
        {
            LogInfo.IsOpen = false;
        }

        private void SetEditEnabled(bool enabled)
        {
            foreach (var element in EditGrid.Children)
            {
                if (element is Control ctl)
                {
                    ctl.IsEnabled = enabled;
                }
            }
        }

        private void ClearEditGrid()
        {
            foreach (var element in EditGrid.Children)
            {
                if (element is TextBox tb)
                {
                    tb.Text = string.Empty;
                }
                else if (element is PasswordBox pw)
                {
                    pw.Password = string.Empty;
                }
                else if (element is CheckBox cb)
                {
                    cb.IsChecked = false;
                }
            }
        }

        private bool CheckEditGrid()
        {
            foreach (var element in EditGrid.Children)
            {
                if (element is TextBox tb)
                {
                    if (string.IsNullOrWhiteSpace(tb.Text))
                    {
                        Log("Name, Host and Port must not be empty.", severity: InfoBarSeverity.Error);
                        return false;
                    }
                }
                //else if (element is PasswordBox pw)
                //{
                //    if (string.IsNullOrWhiteSpace(pw.Password))
                //    {
                //        Log("密码不得为空", severity: InfoBarSeverity.Error);
                //        return false;
                //    }
                //}
            }

            return true;
        }

        private void UpdateTitle(EditTitle detail)
        {
            switch (detail)
            {
                case EditTitle.Detail:
                    EditTitleText.Text = "Detail";
                    break;
                case EditTitle.AddDetail:
                    EditTitleText.Text = "Add Detail";
                    break;
                case EditTitle.EditDetail:
                    EditTitleText.Text = "Edit Detail";
                    break;
            }
        }

        private void ResetEditRegion()
        {
            _isEditMode = false;
            UpdateTitle(EditTitle.Detail);
            ClearEditGrid();
            ClearLog();
            SetEditEnabled(false);
            _hostListItemVM.Clear();
        }

        #endregion
    }
}
