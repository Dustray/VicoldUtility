using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.ChannelColor
{
    /** 
     *        | Alpha | Blue | Green | Red |     Channels    | BCD |
     * --------------------------------------------------------------
     *        |   0   |   0  |   0   |  1  |     Red channel |  1  |
     *        |   0   |   0  |   1   |  0  |   Green channel |  2  |
     *        |   0   |   1  |   0   |  0  |    Blue channel |  4  |
     *  bits  |   0   |   0  |   1   |  1  |  Yellow channel |  3  |
     *        |   0   |   1  |   0   |  1  | Magenta channel |  5  |
     *        |   0   |   1  |   1   |  0  |    Cyan channel |  6  |
     *        |   0   |   1  |   1   |  1  |     All channel |  7  |
     */

    public enum ChannelType
    {
        /// <summary>
        /// 红色通道
        /// </summary>
        [System.ComponentModel.Description("红色")]
        Red = 1,
        
        /// <summary>
        /// 绿色通道
        /// </summary>
        [System.ComponentModel.Description("绿色")]
        Green = 2,
        
        /// <summary>
        /// 蓝色通道
        /// </summary>
        [System.ComponentModel.Description("蓝色")]
        Blue = 4,

        /// <summary>
        /// 透明通道
        /// </summary>
        [System.ComponentModel.Description("透明")]
        Alpha = 8,

        /// <summary>
        /// 黄色通道 = 红色通道+绿色通道
        /// </summary>
        [System.ComponentModel.Description("黄色")]
        Yellow = 3,

        /// <summary>
        /// 洋红通道 = 红色通道+蓝色通道
        /// </summary>
        [System.ComponentModel.Description("洋红")]
        Magenta = 5,

        /// <summary>
        /// 青色通道 = 绿色通道+蓝色通道
        /// </summary
        [System.ComponentModel.Description("青色")]
        Cyan = 6,

        /// <summary>
        /// RGB通道 = 红色通道+绿色通道+蓝色通道
        /// </summary
        [System.ComponentModel.Description("RGB通道")]
        All = 7
    }
}
