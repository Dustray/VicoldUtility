namespace VicoldUtility.PingDashboard.Tools
{
    using System;
    using System.Linq;
    using static Vanara.PInvoke.IpHlpApi;

    internal sealed class SpeedTool
    {
        private MIB_IFTABLE _iftable1;
        private long _lastInFlow = -1;
        private long _lastOutFlow = -1;

        public SpeedTool()
        {
        }

        public void Flush()
        {
            _iftable1 = GetIfTable();
        }

        public void GetUpSpeed(out double speed, out string unit)
        {
            long outSpeed1 = _iftable1.Sum(m => m.dwOutOctets);
            if (_lastOutFlow == -1)
            {
                ChangeSpeedAndUnit(0, out speed, out unit);
                _lastOutFlow = outSpeed1;
                return;
            }

            ChangeSpeedAndUnit(outSpeed1 - _lastOutFlow, out speed, out unit);
            _lastOutFlow = outSpeed1;
        }

        public void GetDownSpeed(out double speed, out string unit)
        {
            long inSpeed1 = _iftable1.Sum(m => m.dwInOctets);
            if (_lastInFlow == -1)
            {
                ChangeSpeedAndUnit(0, out speed, out unit);
                _lastInFlow = inSpeed1;
                return;
            }
            ChangeSpeedAndUnit(inSpeed1 - _lastInFlow, out speed, out unit);
            _lastInFlow = inSpeed1;
        }

        private void ChangeSpeedAndUnit(double sourceSpeed, out double speed, out string unit)
        {
            sourceSpeed /= 8;
            if (sourceSpeed < 1024)
            {
                speed = sourceSpeed;
                unit = "B/s";
            }
            else if ((sourceSpeed = sourceSpeed / 1024) < 1024)
            {
                speed = sourceSpeed;
                unit = "KB/s";
            }
            else if ((sourceSpeed = sourceSpeed / 1024) < 1024)
            {
                speed = sourceSpeed;
                unit = "MB/s";
            }
            else
            {
                sourceSpeed = sourceSpeed / 1024;
                speed = sourceSpeed;
                unit = "GB/s";
            }
        }
    }
}
