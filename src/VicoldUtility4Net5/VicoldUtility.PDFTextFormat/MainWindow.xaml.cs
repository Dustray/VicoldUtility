using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VicoldUtility.PDFTextFormat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] _endSymbol = new char[] { '.', '。' };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbInput.Text))
            {
                return;
            }

            var output = Format(TbInput.Text);
            //Clipboard.SetText(output);
            TbInput.Clear();
            TbOutput.Text = output;
            TbOutput.Focus();
            TbOutput.SelectAll();

            Task.Run(async () =>
            {
                keybd_event(Keys.ControlKey, 0, 0, 0);
                keybd_event(Keys.C, 0, 0, 0);
                await Task.Delay(50);
                keybd_event(Keys.C, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(Keys.C, 0, 0, 0);
                await Task.Delay(50);
                keybd_event(Keys.C, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);
            });

        }
        public const int KEYEVENTF_KEYUP = 2;
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private string Format(string input)
        {
            var builder = new StringBuilder();
            var testLines = Regex.Split(input, "\r\n", RegexOptions.IgnoreCase);
            foreach (var line in testLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                builder.Append(line);
                var lastSymbol = line[line.Length - 1];
                if (_endSymbol.Any(v => v == lastSymbol))
                {
                    // 如果是结束符
                    builder.Append("\r\n");
                }
                else
                {
                    if (lastSymbol != ' ')
                    {
                        builder.Append(" ");
                    }
                }
            }
            return builder.ToString();
        }
    }
}
