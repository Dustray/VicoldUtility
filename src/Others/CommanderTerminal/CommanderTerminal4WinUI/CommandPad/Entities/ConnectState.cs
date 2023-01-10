using CommanderTerminal.Utilities;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.CommandPad.Entities
{
    internal class ConnectState : NotifyPropertyHandler
    {
        public enum CState
        {
            NotConnect = 0,
            Connecting = 1,
            Connected = 2,
            ErrorConnect = 2,
        }

        private static string[] _displayList = {
            "连接失败，点击重连",
            "正在连接...",
            "已连接",
            "错误，无法连接",
        };

        private static Brush[] _colorList = {
            new SolidColorBrush(Colors.DarkRed),
            new SolidColorBrush(Colors.DarkOrange),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Purple),
        };

        private CState _state = CState.NotConnect;
        public CState State
        {
            get => _state;
            set {
                _state = value;
                SetProperty(nameof(Display));
                SetProperty(nameof(Color));
            }
        }
        public string Display
        {
            get => _displayList[(int)State];
            set => SetProperty();
        }

        public Brush Color
        {
            get => _colorList[(int)State];
            set => SetProperty();
        }
    }

}
