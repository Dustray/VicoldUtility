using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VicoldGis.VMap
{
    public class MapManager
    {
        private Dictionary<long, Layer> _layerKeeper;
        public MapManager()
        {
            _layerKeeper = new Dictionary<long, Layer>();
        }

        internal Action<ICollection<FrameworkElement>> OnShowCallback;
        internal Action<ICollection<Visual>> OnShowVisualCallback;
        internal Action<ICollection<FrameworkElement>> OnDeleteCallback;

        public double ScaleX { get; set; }
        public double ScaleY { get; set; }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layer"></param>
        public void Add(Layer layer)
        {
            if (_layerKeeper.ContainsKey(layer.Id))
            {
                throw new Exception("地图中已存在相同ID的图层");
            }
            layer.Load();
            OnShowCallback.Invoke(layer.GetElements());//Visual
            _layerKeeper[layer.Id] = layer;
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layer"></param>
        public Layer GetLayer(long id)
        {
            if (_layerKeeper.TryGetValue(id, out var layer))
            {
                return layer;
            }
            return null;
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layer"></param>
        public void Update(Layer layer)
        {
            if (_layerKeeper.TryGetValue(layer.Id, out var oldLayer))
            {
                Delete(layer.Id);
            }
            Add(layer);
        }

        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="id"></param>
        public void Delete(long id)
        {
            if (_layerKeeper.TryGetValue(id, out Layer layer))
            {
                OnDeleteCallback.Invoke(layer.GetElements());
                _layerKeeper.Remove(id);
            }
            else
            {
                throw new Exception("地图中不存在指定ID的图层");
            }
        }

        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="id"></param>
        public void Visiable(long id, bool isShow)
        {
            if (_layerKeeper.TryGetValue(id, out Layer layer))
            {
                layer.SetVisiable(isShow);
            }
            else
            {
                throw new Exception("地图中不存在指定ID的图层");
            }
        }

        /// <summary>
        /// 图层是否存在
        /// </summary>
        /// <param name="id"></param>
        public void IsExist(long id) => _layerKeeper.ContainsKey(id);
    }
}
