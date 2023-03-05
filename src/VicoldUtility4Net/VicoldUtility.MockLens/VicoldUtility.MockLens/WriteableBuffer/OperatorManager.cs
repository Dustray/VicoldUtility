using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using VicoldUtility.MockLens.Utilities;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal class OperatorManager
    {
        private RunningLocker locker = new RunningLocker();
        private IList<IBufferOperator> _bufferOperators = new List<IBufferOperator>();
        //private Dictionary<Type, IBufferOperator> _bufferOperatorDictionary = new Dictionary<Type, IBufferOperator>();
        private bool _isExecuting = false;
        private IAddChild? _addChildFunc;
        private int _lastChangedId;

        public void RegisterContainer(IAddChild content)
        {
            _addChildFunc = content;
        }

        public Action? OnUpdated { get; set; }

        public void AddOperator(IBufferOperator bufferOperator)
        {
            _bufferOperators.Add(bufferOperator);
            var control = bufferOperator.GetOperatorControl();
            if (_addChildFunc is { } && control is { })
            {
                _addChildFunc.AddChild(control);
            }
        }

        public ImageRuntimeBuffer? RuntimeBuffer { get; set; }

        private void ExecAsync(int id)
        {
            if (RuntimeBuffer is not { })
            {
                return;
            }

            BufferIterator bufferIterator= new BufferIterator(RuntimeBuffer.SourceData, RuntimeBuffer.Data);

            foreach (var opt in _bufferOperators)
            {
                opt.Compute(ref bufferIterator);
                bufferIterator = new BufferIterator(RuntimeBuffer.Data, RuntimeBuffer.Data);
            }

            _lastChangedId = id;
        }

        internal void ExecuteCalculation(int id = 0)
        {
            locker.Lock(() =>
            {
                return Task.Run(() =>
                {
                    ExecAsync(id);
                    OnUpdated?.Invoke();
                });
            });
        }

    }
}
