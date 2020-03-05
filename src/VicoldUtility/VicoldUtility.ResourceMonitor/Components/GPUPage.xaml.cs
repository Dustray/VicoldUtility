using System.Windows.Controls;
using VicoldUtility.ResourceMonitor.Entities;
using VicoldUtility.ResourceMonitor.Properties;

namespace VicoldUtility.ResourceMonitor.Components
{
    /// <summary>
    /// GPUPage.xaml 的交互逻辑
    /// </summary>
    public partial class GPUPage : Page
    {
        private float _inv;
        private bool _loadedCoreGrid = false;
        public GPUPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
        }
        public void ImportData(GPUEtt ett)
        {
            if (ett == null) return;
            Dispatcher.Invoke(() =>
            {

            });
        }
    }
}
