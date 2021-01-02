using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VicoldGis.VMap
{
    public class Layer
    {
        private HashSet<FrameworkElement> _elements;
        private bool isLocked = false;

        public Layer()
        {
            _elements = new HashSet<FrameworkElement>();
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        public void Add(FrameworkElement element)
        {
            if (isLocked)
            {
                throw new Exception("图层已被锁定，无法添加");
            }

            _elements.Add(element);
        }

        internal ICollection<FrameworkElement> GetElements()
        {
            isLocked = true;
            return _elements;
        }

        internal void SetVisiable(bool isShow)
        {
            isLocked = true;
            foreach (var element in _elements)
            {
                element.Visibility = isShow ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
