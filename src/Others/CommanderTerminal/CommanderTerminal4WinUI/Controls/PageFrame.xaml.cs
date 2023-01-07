// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CommanderTerminal.Controls
{
    public sealed partial class PageFrame : Grid
    {
        private Page? _lastPage;
        public PageFrame()
        {
            this.InitializeComponent();
        }

        public void Navigate(Page page)
        {
            //var ani = new DoubleAnimation();
            //ani.AutoReverse = false;
            //ani.Duration = TimeSpan.FromMilliseconds(300);
            //ani.SetValue(Storyboard.TargetPropertyProperty, "Height");
            //Storyboard.SetTarget(ani, page);
            //Storyboard.SetTargetProperty(da, "Angle");
            if (_lastPage is { })
            {
                Children.Remove(_lastPage);
            }


            Children.Add(page);

            _lastPage = page;
        }
    }
}
