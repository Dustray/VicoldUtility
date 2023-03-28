using CommanderTerminal.Adding;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.Utilities
{
    internal static class DialogUtility
    {
        public static async Task<bool> ShowMessage(this UIElement element, string message, string? title=null, string? primaryText = null, string? secondaryText = null, string? closeText = null)
        {
            var addDialog = new ContentDialog();
            addDialog.XamlRoot = element.XamlRoot;
            addDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            addDialog.Title = title;
            addDialog.IsPrimaryButtonEnabled = primaryText is not null;
            addDialog.IsSecondaryButtonEnabled = secondaryText is not null; ;
            addDialog.PrimaryButtonText = primaryText;
            addDialog.SecondaryButtonText = secondaryText;
            addDialog.CloseButtonText = closeText;
            addDialog.DefaultButton = ContentDialogButton.Primary;
            addDialog.Content = message;

            var dialogResult = await addDialog.ShowAsync();
            return dialogResult == ContentDialogResult.Primary;
        }
    }
}
