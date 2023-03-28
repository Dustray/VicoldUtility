using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.Utilities
{
    internal class RunningLocker
    {
        private bool _isLocked = false;
        public RunningLocker()
        { }

        public async void Lock(Func<Task> action)
        {
            if (_isLocked)
            {
                return;
            }
            _isLocked = true;
            await action();
            _isLocked = false;
        }
    }
}
