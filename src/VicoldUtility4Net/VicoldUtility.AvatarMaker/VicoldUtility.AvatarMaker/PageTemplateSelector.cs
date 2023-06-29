using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using VicoldUtility.AvatarMaker.Pages;

namespace VicoldUtility.AvatarMaker
{
    public class PageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PreviewPage)
            {
                return (container as FrameworkElement).FindResource("PreviewPageTemplate") as DataTemplate;
            }
            else if (item is AdjustPage)
            {
                return (container as FrameworkElement).FindResource("AdjustPageTemplate") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
