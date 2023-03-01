using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal class OperatorManager
    {
        private IList<IBufferOperator> _bufferOperators = new List<IBufferOperator>();
        private Dictionary<Type, IBufferOperator> _bufferOperatorDictionary = new Dictionary<Type, IBufferOperator>();
        private bool _isExecuting = false;
        public void AddOperator(IBufferOperator bufferOperator)
        {
            _bufferOperators.Add(bufferOperator);
            _bufferOperatorDictionary.Add(bufferOperator.GetType(), bufferOperator);
        }
        
        public ImageRuntimeBuffer? RuntimeBuffer { get; set; }

        public Task ExecAsync()
        {
            if(RuntimeBuffer is not { })
            {
                return Task.CompletedTask;
            }

            if (_isExecuting)
            {
                return Task.CompletedTask;
            }
            
            return Task.Run(() =>
            {
                _isExecuting = true;
                BufferIterator bufferIterator = new BufferIterator(RuntimeBuffer.SourceData, RuntimeBuffer.Data);
                foreach(var opt in _bufferOperators)
                {
                    opt.Compute(ref bufferIterator);
                    bufferIterator = new BufferIterator(RuntimeBuffer.Data, RuntimeBuffer.Data);
                }
                _isExecuting = false;
            });
        }

        internal IBufferOperator GetOperator<T>() where T : IBufferOperator
        {
            return _bufferOperatorDictionary[typeof(T)];
        }
    }
}
