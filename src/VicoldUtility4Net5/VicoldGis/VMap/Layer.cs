using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using VicoldGis.Adapters;

namespace VicoldGis.VMap
{
    public class Layer
    {
        private HashSet<FrameworkElement> _elements;
        private HashSet<Visual> _vElements;
        private bool isLocked = false;



        public Layer()
        {
            _elements = new HashSet<FrameworkElement>();
            _vElements = new HashSet<Visual>();
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        public IAdapter DataSource { get; set; }

        internal void Load()
        {
            if (DataSource != null)
            {
                _elements.Clear();
                _vElements.Clear();
                var views = DataSource.GetViewData();
                foreach (var view in views)
                {
                    Add(view);
                }
            }
        }

        public void Add(FrameworkElement element)
        {
            if (isLocked)
            {
                //throw new Exception("图层已被锁定，无法添加");
            }

            _elements.Add(element);
        }

        public void Add(Visual element)
        {
            if (isLocked)
            {
                //throw new Exception("图层已被锁定，无法添加");
            }

            _vElements.Add(element);
        }
        internal ICollection<Visual> GetVisualElements()
        {
            isLocked = true;
            return _vElements;
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
