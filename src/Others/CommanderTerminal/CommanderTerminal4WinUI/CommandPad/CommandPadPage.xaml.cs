// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommanderTerminal.Adding;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CommanderTerminal.CommandPad
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommandPadPage : Page
    {
        private SSHHostItemConfigEtt _itemConfig;
        public CommandPadPage(CommanderTerminalCore.Configuration.Entities.SSHHostItemConfigEtt itemConfig)
        {
            this.InitializeComponent();
            _itemConfig = itemConfig;
        }

        public async  void Init()
        {
            var addDialog = new ContentDialog();
            addDialog.XamlRoot = this.XamlRoot;
            addDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            addDialog.Title = $"Input Password for {_itemConfig.Host}:{_itemConfig.Port}";
            addDialog.IsPrimaryButtonEnabled = true;
            addDialog.IsSecondaryButtonEnabled = false;
            addDialog.PrimaryButtonText = "Login";
            //addDialog.SecondaryButtonText = "Don't Save";
            addDialog.CloseButtonText = "Cancel";
            addDialog.DefaultButton = ContentDialogButton.Primary;
            var addPage = new AddPage();

            addDialog.Content = addPage;
            addDialog.CloseButtonClick += (sender, e) =>
            {
                addPage.IsReadyToOpenHost = true;
            };
            addDialog.Closing += (sender, e) =>
            {
                if (!addPage.IsReadyToOpenHost)
                {
                    addPage.Log("Please choosing a host.", severity: InfoBarSeverity.Error);
                    e.Cancel = true;
                }
            };

            var dialogResult = await addDialog.ShowAsync();
            if (dialogResult == ContentDialogResult.None)
            {
                return;
            }

        }
    }
}
