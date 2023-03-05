using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace VicoldUtility.MockLens.WriteableBuffer.OperatorControls
{

    public class SliderParams
    {
        public SliderParams(double min = 0, double max = 100, double value = 0, double step = 1, byte digits = 2)
        {
            var count = (int)((max - min) / step + 1);
            Params = new double[count];
            Digits = digits;

            for (int i = 0; i < count; i++)
            {
                Params[i] = Math.Round(min + i * step, digits, MidpointRounding.AwayFromZero);
                if (Params[i] == value)
                {
                    CurrentIndex = i;
                }
            }
        }
        public SliderParams(double[] paramsArray, int currentIndex = 0, byte digits = 2)
        {
            if (currentIndex >= paramsArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(currentIndex));
            }
            Params = paramsArray;
            CurrentIndex = currentIndex;
            Digits = digits;
        }

        public double[] Params { get; private set; }

        public int Count => Params.Length;

        public int CurrentIndex { get; set; }

        public byte Digits { get; private set; }

        public double GetCurrentValue()
        {
            return Params[CurrentIndex];
        }
    }

    /// <summary>
    /// BaseSlider.xaml 的交互逻辑
    /// </summary>
    public partial class BaseSlider: BaseControl
    {
        private int _minIndex = 0;
        private int _maxIndex;
        private string? _display;
        private SliderParams _sliderParams;
        private int _id;
        public BaseSlider(int id, string display, SliderParams sliderParams)
        {
            InitializeComponent();
            _id = id;
            _sliderParams = sliderParams;
            _maxIndex = _sliderParams.Count-1;
            _display = display;
            DataContext = this;
        }

        public string? Display
        {
            get => _display;
            set => SetProperty(ref _display, value);
        }

        public int MinIndex
        {
            get => _minIndex;
            set => SetProperty(ref _minIndex, value);
        }

        public int MaxIndex
        {
            get => _maxIndex;
            set => SetProperty(ref _maxIndex, value);
        }

        public int CurrentIndex
        {
            get => _sliderParams.CurrentIndex;
            set
            {
                _sliderParams.CurrentIndex = value;
                SetProperty();
                SetProperty("CurrentValueStr");
                App.Current.OperatorManager.ExecuteCalculation(_id);
            }
        }

        public double CurrentValue => _sliderParams.GetCurrentValue();

        public string CurrentValueStr
        {
            get => CurrentValue.ToString("f2");
            set => SetProperty();
        }

        private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
