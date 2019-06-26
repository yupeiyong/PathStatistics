using System.Drawing;
using YPY.Winform.Library.UserControls;

namespace PathStatistics.Models
{
    /// <summary>
    ///     路径展示
    /// </summary>
    public class PathShower
    {
        /// <summary>
        /// 用一字母表示的路径名称
        /// </summary>
        public char PathName { get; set; }

        /// <summary>
        /// 从0开始的索引，指示当前路径展示的顺序
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     开始点
        /// </summary>
        public Point StartPoint { get; set; }


        /// <summary>
        ///     结束点
        /// </summary>
        public Point EndPoint { get; set; }


        /// <summary>
        ///     当前点
        /// </summary>
        public Point CurrentPoint { get; set; }

        /// <summary>
        ///     横向线条
        /// </summary>
        public LineX LineX { get; set; }


        /// <summary>
        ///     纵向线条
        /// </summary>
        public LineY LineY { get; set; }


        /// <summary>
        ///     垂直或水平方向
        /// </summary>
        public Direction VerticalDirection { get; set; }


        /// <summary>
        ///     水平方向时，向左或向右
        /// </summary>
        public Direction LeftOfRight { get; set; }
    }

    /// <summary>
    ///     方向
    /// </summary>
    public enum Direction
    {
        /// <summary>
        ///     水平
        /// </summary>
        Horizontal = 0,

        /// <summary>
        ///     垂直
        /// </summary>
        Vertical = 1,

        /// <summary>
        ///     左
        /// </summary>
        Left = 2,

        /// <summary>
        ///     右
        /// </summary>
        Right = 3
    }
}