using System;
using System.Collections.Generic;
using System.Text;

namespace VicoldUtility.Scr.ImageSlider
{
    internal class LoopQueue<T>:IDisposable
    {
        public bool IsRandom = false;
        private int _index = 0;
        private T[] _powerQueue;
        private int _queueLength = 0;

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="queue"></param>
        public void Add(T[] queue)
        {
            _powerQueue = queue;
            _queueLength = _powerQueue.Length;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// 下一个
        /// </summary>
        /// <returns></returns>
        public T Next()
        {
            if (IsRandom)
            {
                var r = new Random();
                while (true)
                {
                    var nextIndex = r.Next(_queueLength);
                    if (_index != nextIndex)
                    {
                        _index = nextIndex;
                        break;
                    }
                }
                return _powerQueue[_index];
            }
            else
            {
                if (_index >= _queueLength)
                {
                    _index = 0;
                }
                return _powerQueue[_index++];
            }
        }
    }
}
