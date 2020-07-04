using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Utilities
{
    class DesktopLinkUtil
    {
        /// <summary>
        /// 快速创建
        /// </summary>
        /// <returns></returns>
        public static bool FastCreate(bool forceCreate = false)
        {
            string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var lu = new DesktopLinkUtil();
            var b1 = lu.CreateDeskTopLik("快速链接", "快速打开配置好的链接", Path.Combine(System.Windows.Forms.Application.StartupPath, $"快速链接.exe"), deskTop, "logo", forceCreate);
            return b1;
        }
        /// <summary>
        /// 创建开机启动快捷方式
        /// </summary>
        /// <returns></returns>
        public static bool CreateLauncherLink()
        {
            string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var lu = new DesktopLinkUtil();
            var b1 = lu.CreateDeskTopLik("快速链接", "快速打开配置好的链接", Path.Combine(System.Windows.Forms.Application.StartupPath, $"快速链接.exe"), deskTop, "logo", false);
            return b1;
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="name">目标名称</param>
        /// <param name="description">目标描述</param>
        /// <param name="sourceFilePath">源文件路径</param>
        /// <param name="logoName">logo名称</param>
        /// <returns></returns>
        public bool CreateDeskTopLik(string name, string description, string sourceFilePath, string targetFolderPath, string logoName, bool forceCreate)
        {
            #region 创建桌面快捷方式  

            string dirPath = System.Windows.Forms.Application.StartupPath;
            string exePath = Assembly.GetExecutingAssembly().Location;
            if (!forceCreate && System.IO.File.Exists($@"{targetFolderPath}\{name}.lnk"))
            {
                return false;
            }
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut($@"{targetFolderPath }\{name}.lnk");
            shortcut.TargetPath = sourceFilePath;   //目标文件
            shortcut.WorkingDirectory = dirPath;    //目标文件夹
            shortcut.WindowStyle = 1;               //目标应用程序的窗口状态分为普通、最大化、最小化【1,3,7】
            shortcut.Description = description;     //描述
            shortcut.IconLocation = $@"{dirPath}\Assets\{logoName}.ico";  //快捷方式图标
            shortcut.Arguments = "";
            //shortcut.Hotkey = "ALT+X";              // 快捷键
            shortcut.Save();
            return true;
            #endregion
        }
    }
}
