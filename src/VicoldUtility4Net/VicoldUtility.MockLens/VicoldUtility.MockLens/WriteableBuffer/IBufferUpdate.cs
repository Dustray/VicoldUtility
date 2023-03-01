using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal interface IBufferUpdate<T>
    {
        void Update(T value);
    }
}
