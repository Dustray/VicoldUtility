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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
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
            SetDateWindow setDateWindow = new SetDateWindow();
            if (setDateWindow.ShowDialog() == true)
            {
                targetDate = setDateWindow.SelectedDate.Value ;
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
                CountdownDisplay.Text = "目标日期已过,请设置新的日期";
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
                CountdownDisplay.Text = "倒计时结束！";
                return;
            }

            int years = remainingTime.Days / 365;
            int days = remainingTime.Days % 365;
            int hours = remainingTime.Hours;
            int minutes = remainingTime.Minutes;
            int seconds = remainingTime.Seconds;

            CountdownDisplay.Text = $"{years}年 {days}天 {hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}