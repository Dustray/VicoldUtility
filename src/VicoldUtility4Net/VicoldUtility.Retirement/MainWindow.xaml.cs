using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VicoldUtility.Retirement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DateTime targetDate;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            LoadWindowPosition();
            this.Closing += MainWindow_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTargetDate();
        }

        private void LoadTargetDate()
        {
            string? savedDate = ConfigurationManager.AppSettings.Get("TargetDate");
            if (string.IsNullOrEmpty(savedDate) || !DateTime.TryParse(savedDate, out targetDate))
            {
                ShowSetDateWindow();
            }
            else
            {
                StartCountdown();
            }
        }

        private void ShowSetDateWindow()
        {
            SetDateWindow setDateWindow = new();
            if (setDateWindow.ShowDialog() == true)
            {
                targetDate = setDateWindow.SelectedDate??DateTime.Now;
                SaveTargetDate();
                StartCountdown();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void SaveTargetDate()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add("TargetDate", targetDate.ToString("yyyy-MM-dd"));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void StartCountdown()
        {
            if (targetDate > DateTime.Now)
            {
                timer.Start();
                UpdateDisplay();
            }
            else
            {
                CountdownDisplay.Text = "您已退休，请重生以设置新的日期";
            }
        }

        private void ChangeDateButton_Click(object? sender, RoutedEventArgs e)
        {
            ShowSetDateWindow();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            TimeSpan remainingTime = targetDate - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                timer.Stop();
                CountdownDisplay.Text = "倒计时结束，恭喜你退休啦！";
                return;
            }

            int years = remainingTime.Days / 365;
            int days = remainingTime.Days % 365;
            int hours = remainingTime.Hours;
            int minutes = remainingTime.Minutes;
            int seconds = remainingTime.Seconds;

            CountdownDisplay.Text = $"{years}年 {days}天 {hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // 新增: 加载窗体位置
        private void LoadWindowPosition()
        {
            if (double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out double left) && left < 10000 &&
                double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out double top) && top < 10000)
            {

                this.Left = left;
                this.Top = top;
            }

            if (double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out double width) && width < 10000 &&
                double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out double height) && height < 10000)
            {
                this.Width = width;
                this.Height = height;
            }
        }

        // 新增: 保存窗体位置
        private void SaveWindowPosition()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add("WindowLeft",this.Left.ToString());
            config.AppSettings.Settings.Add("WindowTop", this.Top.ToString());
            config.AppSettings.Settings.Add("WindowWidth", this.Width.ToString());
            config.AppSettings.Settings.Add("WindowHeight", this.Height.ToString());
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        // 新增: 窗口关闭事件处理
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowPosition();
        }
    }
}