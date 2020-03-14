using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ProxyAccess
{
    public class IPRequireMod
    {
        private bool _SuccessMod = true;
        private int _planToSuccessCount = 0;
        private int _planToTimesCount = 0;

        public void ChangeToSuccessMod(int planToSuccessCount)
        {
            _planToSuccessCount = planToSuccessCount;
            _SuccessMod = true;
        }

        public void ChangeToTimesMod(int planToTimesCount)
        {
            _planToTimesCount = planToTimesCount;
            _SuccessMod = false;
        }

        /// <summary>
        /// 判断成功执行目标是否达到预定目标
        /// </summary>
        /// <param name="successCount"></param>
        /// <param name="timesCount"></param>
        /// <returns></returns>
        public bool IsExecuteOver(int successCount, int timesCount)
        {
            if (_SuccessMod)
            {
                return successCount >= _planToSuccessCount;
            }
            else
            {
                return timesCount >= _planToTimesCount;
            }
        }

    }
}
