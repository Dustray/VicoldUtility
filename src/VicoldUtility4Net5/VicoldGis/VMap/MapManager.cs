using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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
        internal Action<ICollection<FrameworkElement>> OnDeleteCallback;

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
            OnShowCallback.Invoke(layer.GetElements());
            _layerKeeper[layer.Id] = layer;
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
