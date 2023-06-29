using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FlashAutoBackup.SystemTools
{
    internal static class NotificationTool
    {
        public static void ShowNotification(string title, string content)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText(title)
                .AddText(content)
                .Show();
        }
    }
}
