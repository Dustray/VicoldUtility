using LightX.Algorithm.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightX.Algorithm.Model.Isoline
{
    public class IsolineBuilder
    {
        private float[] _data;
        private float[] _analyzeValue;

        private int _startX;
        private int _startY;
        private int _width;
        private int _height;
        private int _pitch;

        private bool _isCrossoverAutoOffset = true;
        private byte _smoothCount = 0;
        private float _invalidValue = 9999;
        private float _lon = 0;
        private float _lat = 0;
        private float _lonInterval = 1;
        private float _latInterval = 1;

        internal IsolineBuilder(float[] data, int startX, int startY, int width, int height)
        {
            if (data == null || data.Length == 0)
            {
                throw new Exception("Data cannot be empty!");
            }

            if (data.Length != width * height)
            {
                throw new Exception("Wrong data dimension!");
            }

            _data = data;
            _startX = startX;
            _startY = startY;
            _width = width;
            _height = height;
            _pitch = width;
        }

        /// <summary>
        /// Set analyze values.
        /// </summary>
        /// <param name="analyzeValue"></param>
        /// <returns></returns>
        public IsolineBuilder SetAnalyzeValue(float[] analyzeValue)
        {
            _analyzeValue = analyzeValue;
            return this;
        }

        /// <summary>
        /// [optional] Set pitch.
        /// <para>default is width.</para>
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns>Loader</returns>
        public IsolineBuilder SetPitch(int pitch)
        {
            _pitch = pitch;
            return this;
        }

        /// <summary>
        /// [optional] Set crossover auto offset.
        /// <para>default is true.</para>
        /// </summary>
        /// <param name="isCrossoverAutoOffset"></param>
        /// <returns>Loader</returns>
        public IsolineBuilder SetCrossoverAutoOffset(bool isCrossoverAutoOffset)
        {
            _isCrossoverAutoOffset = isCrossoverAutoOffset;
            return this;
        }

        /// <summary>
        /// [optional] Set smooth count.
        /// <para>default is 0, greater than 1 is available.</para>
        /// </summary>
        /// <param name="smoothCount"></param>
        /// <returns>Loader</returns>
        public IsolineBuilder SetSmoothCount(byte smoothCount)
        {
            _smoothCount = smoothCount;
            return this;
        }

        /// <summary>
        /// [optional] Set invalid value.
        /// <para>default is 9999.</para>
        /// </summary>
        /// <param name="invalidValue">Invalid value</param>
        /// <returns>Loader</returns>
        public IsolineBuilder SetInvalidValue(byte invalidValue)
        {
            _invalidValue = invalidValue;
            return this;
        }

        /// <summary>
        /// [optional] Set geographical location.
        /// <para>default lon is 0;</para>
        /// <para>default lat is 0;</para>
        /// <para>default lonInterval is 1;</para>
        /// <para>default latInterval is 1;</para>
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="lonInterval"></param>
        /// <param name="latInterval"></param>
        /// <returns>Loader</returns>
        public IsolineBuilder SetGeographicalLocation(float lon, float lat, float lonInterval, float latInterval)
        {
            _lon = lon;
            _lat = lat;
            _lonInterval = lonInterval;
            _latInterval = latInterval;
            return this;
        }

        public ValueLine[] Build()
        {
            if (_analyzeValue == null || _analyzeValue.Length == 0)
            {
                throw new Exception("You have not set the analysis values.");
            }

            var Lines = Connector.CreateValueLines(_data, _startX, _startY, _width, _height, _pitch, _analyzeValue,
                  _isCrossoverAutoOffset, _smoothCount, _invalidValue, _lon, _lat, _lonInterval, _latInterval, null);

            return Lines;
        }

        public void BuildInRealTime(Action<ValueLine> callback)
        {
            if (_analyzeValue == null || _analyzeValue.Length == 0)
            {
                throw new Exception("You have not set the analysis values.");
            }

            Connector.CreateValueLines(_data, _startX, _startY, _width, _height, _pitch, _analyzeValue,
                  _isCrossoverAutoOffset, _smoothCount, _invalidValue, _lon, _lat, _lonInterval, _latInterval, callback);
        }
    }
}
