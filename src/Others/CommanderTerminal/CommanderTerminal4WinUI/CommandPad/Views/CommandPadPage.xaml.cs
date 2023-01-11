// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommanderTerminal.Adding;
using CommanderTerminal.Adding.ViewModels;
using CommanderTerminal.CommandPad.Entities;
using CommanderTerminal.CommandPad.ViewModels;
using CommanderTerminal.Utilities;
using CommanderTerminalCore.Configuration.Entities;
using CommanderTerminalCore.SSHConnection;
using Microsoft.UI;
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CommanderTerminal.CommandPad
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommandPadPage : Page
    {
        internal class HostEntity
        {
            public HostEntity(SSHHostItemConfigEtt config)
            {
                Name = config.Name ?? "Unnamed";
                Host = config.Host ?? "Unnamed";
                if (int.TryParse(config.Port, out var port))
                {
                    Port = port;
                }
                else
                {
                    Port = 22;
                }

                Password = config.RememberedPasswd;
            }

            public string Name { get; set; }

            public string Host { get; set; }

            public int Port { get; set; }

            public string HostPort => $"{Host}:{Port}";

            public string? Password { get; set; }
        }

        private ISSHHandle _handle;
        private ObservableCollection<CommandListItemMV> _commandListItems = new ObservableCollection<CommandListItemMV>();
        private SSHHostItemConfigEtt _itemConfig;
        public CommandPadPage(SSHHostItemConfigEtt itemConfig)
        {
            _itemConfig = itemConfig;
            HostEtt = new HostEntity(itemConfig);
            _handle = SSHHandleFactory.Create(HostEtt.Host, HostEtt.Port);
            this.InitializeComponent();
            Task.Run(async () =>
            {
                await Task.Delay(100);
                DispatcherQueue.TryEnqueue(() =>
                {
                    Init();
                });
            });
        }

        internal HostEntity HostEtt { get; }

        internal ConnectState ConnectionState { get; } = new ConnectState();

        internal bool IsConnected => ConnectionState.State == ConnectState.CState.Connected;

        public async void Init()
        {
            bool isWrong = false;
            while (true)
            {
                var isContinue = await OpenPasswordInputDialog(isWrong);
                if (!isContinue)
                {
                    break;
                }

                isWrong = true;
            }

            if (IsConnected && _itemConfig.Commands is { })
            {
                foreach (var command in _itemConfig.Commands)
                {
                    _commandListItems.Add(command.ToVM());
                }
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 重连按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Reconnect_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ConnectionState.State == ConnectState.CState.NotConnect)
            {
                if (HostEtt.Password == null)
                {
                    await OpenPasswordInputDialog();
                }

                await Connect().ConfigureAwait(false);
            }
        }

        private void SSHCommandList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task<bool> OpenPasswordInputDialog(bool isWrong = false)
        {
            if (string.IsNullOrWhiteSpace(HostEtt.Password))
            {
                var addDialog = new ContentDialog();
                addDialog.XamlRoot = this.Content.XamlRoot;
                addDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                addDialog.Title = $"Input Password for {HostEtt.HostPort}";
                addDialog.IsPrimaryButtonEnabled = true;
                addDialog.IsSecondaryButtonEnabled = false;
                addDialog.PrimaryButtonText = "Login";
                //addDialog.SecondaryButtonText = "Don't Save";
                addDialog.CloseButtonText = "Cancel";
                addDialog.DefaultButton = ContentDialogButton.Primary;
                var passwdPage = new PasswordInputPage(isWrong);

                addDialog.Content = passwdPage;
                addDialog.PrimaryButtonClick += (sender, e) =>
                {
                    passwdPage.Check();
                };
                addDialog.CloseButtonClick += (sender, e) =>
                {
                    passwdPage.IsGoOn = true;
                };
                addDialog.Closing += (sender, e) =>
                {
                    if (!passwdPage.IsGoOn)
                    {
                        e.Cancel = true;
                    }
                };

                var dialogResult = await addDialog.ShowAsync();
                if (dialogResult == ContentDialogResult.Primary)
                {
                    bool isSuccess = await Connect();
                    if (isSuccess)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public Task<bool> Connect()
        {
            ConnectionState.State = ConnectState.CState.Connecting;
            return Task.Run(async () =>
            {
                bool is_success = false;
                await Task.Delay(1000);
                DispatcherQueue.TryEnqueue(() =>
                {
                    ConnectionState.State = is_success ? ConnectState.CState.Connected : ConnectState.CState.NotConnect;
                });

                if (!is_success)
                {
                    HostEtt.Password = null;
                }

                return is_success;
            });
        }

        internal void Close()
        {
            _handle.Close();
        }

        private void SaveConfig()
        {
            TerminalCore.Current.HostConfig.GetConfigEtt().Save();
        }

        private void SyncDataToConfig()
        {
            var config = _itemConfig;
            if (config.Commands is not { })
            {
                config.Commands = new();
            }

            for (var i = 0; i < _commandListItems.Count; i++)
            {
                var vm = _commandListItems[i];
                SSHCommandConfigEtt command;
                if (config.Commands.Count <= i)
                {
                    command = new();
                    config.Commands.Add(command);
                }
                else
                {
                    command = config.Commands[i];
                }

                command.ID = vm.ID;
                command.Name = vm.Name;
                command.Command = vm.Command;
            }

            SaveConfig();
        }


    }
}
