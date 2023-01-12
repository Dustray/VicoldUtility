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

        public bool IsReadyToOpenHost { get; set; }
        public SSHHostItemConfigEtt? HostItemConfigEtt { get; private set; }
        public Action? OnHostChecked { get; internal set; }

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

        #region ÊÂ¼þ

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UpdateTitle(EditTitle.AddDetail);
            SetEditEnabled(true);
            _hostListItemVM.Clear();
            _hostListItemVM.Name = "New Host";
            _hostListItemVM.Port = "22";
            HostName.Focus(FocusState.Pointer);
            HostName.SelectAll();
            ClearLog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEditGrid())
            {
                IsReadyToOpenHost = false;
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
                HostPassword.Password = _hostListItemVM.Password;
                RememberPasswd.IsChecked = _hostListItemVM.IsSavePassword;
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
            //if (_isEditMode)
            //{
            //    Log("Please [clear] Edit State first.");
            //    IsReadyToOpenHost = false;
            //    return;
            //}

            if (SSHHostList.SelectedIndex < 0 || SSHHostList.SelectedIndex >= _hostListItems.Count)
            {
                Log("Select Error.", severity: InfoBarSeverity.Error);
                IsReadyToOpenHost = false;
                return;
            }

            var vm = _hostListItems[SSHHostList.SelectedIndex];
            _hostListItemVM.CopyFrom(vm);
            HostPassword.Password = _hostListItemVM.Password;
            RememberPasswd.IsChecked = _hostListItemVM.IsSavePassword;
            var config = TerminalCore.Current.HostConfig.GetConfigEtt();
            HostItemConfigEtt = config.SelectHostItemByID(vm.ID);
            if (HostItemConfigEtt is { })
            {
                OnHostChecked?.Invoke();
                IsReadyToOpenHost = true;
            }
            else
            {
                Log("Cannot find host from configuration.", severity: InfoBarSeverity.Error);
                IsReadyToOpenHost = false;
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

            configHost.ID = hostListItemVM.ID;
            configHost.Name = hostListItemVM.Name;
            configHost.Host = hostListItemVM.Host;
            configHost.Port = hostListItemVM.Port;
            configHost.User = hostListItemVM.User;
            configHost.RememberedPasswd = hostListItemVM.Password;

            config.Save();
        }

        public void Log(string message, string? title = null, InfoBarSeverity severity = InfoBarSeverity.Informational)
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
            HostName.IsEnabled = enabled;
            HostHost.IsEnabled = enabled;
            HostPort.IsEnabled = enabled;

            if (!enabled && string.IsNullOrWhiteSpace(HostPassword.Password))
            {
                HostUser.IsEnabled = true;
                HostPassword.IsEnabled = true;
                RememberPasswd.IsEnabled = true;
                SaveBtn.IsEnabled = true;
            }
            else
            {

                HostUser.IsEnabled = enabled;
                HostPassword.IsEnabled = enabled;
                RememberPasswd.IsEnabled = enabled;
                SaveBtn.IsEnabled = enabled;
            }
            ClearBtn.IsEnabled = enabled;
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
            do
            {
                if (string.IsNullOrWhiteSpace(HostName.Text))
                {
                    Log("Name must not be empty.", severity: InfoBarSeverity.Error);
                    break;
                }

                if (string.IsNullOrWhiteSpace(HostHost.Text))
                {
                    Log("Host must not be empty.", severity: InfoBarSeverity.Error);
                    break;
                }

                if (string.IsNullOrWhiteSpace(HostPort.Text))
                {
                    Log("Port must not be empty.", severity: InfoBarSeverity.Error);
                    break;
                }

                return true;
            } while (true);

            return false;
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
