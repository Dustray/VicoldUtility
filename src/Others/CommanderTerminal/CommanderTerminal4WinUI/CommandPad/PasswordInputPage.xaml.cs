// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommanderTerminalCore.SSHConnection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Renci.SshNet.Messages;
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
    public sealed partial class PasswordInputPage : Page
    {
        public PasswordInputPage(ISSHHandle handle, bool isWrong=false)
        {
            this.InitializeComponent();
            if (isWrong)
            {
                Log("Wrong password, please try again.", severity: InfoBarSeverity.Error);
            }
        }


        public string InputPasswd { get; set; } = string.Empty;

        public bool Check()
        {
            return true;
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
    }
}
