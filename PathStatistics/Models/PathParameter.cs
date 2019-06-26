using System.ComponentModel;
using YPY.Winform.Library.UserControls.DataGridViewColumn;

namespace PathStatistics.Models
{
    /// <summary>
    ///     路径详细设置
    /// </summary>
    public class PathParameter
    {
        /// <summary>
        ///     路径序号
        /// </summary>
        [Order(Order = 0)]
        [Description("序号")]
        public int Index { get; set; }


        /// <summary>
        ///     路径，
        /// </summary>
        [Description("路径")]
        [Order(Order = 1)]
        public string Route { get; set; }

        /// <summary>
        ///     路径概率计算的描述
        /// </summary>
        [Description("截住概率")]
        [Order(Order = 2)]
        public string Probability { get; set; }


        /// <summary>
        ///     概率值
        /// </summary>
        public double ProbabilityValue { get; set; }
    }
}