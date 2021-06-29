using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace VicoldUtility.PhotoSelector.Entities
{
    /// <summary>
    /// 图片后缀（标签）VM
    /// </summary>
    public class ImageLabelViewModel : BaseProperty
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public Brush Background => App.Current.SZM.GetImageLabelColor(Text);
    }
}
