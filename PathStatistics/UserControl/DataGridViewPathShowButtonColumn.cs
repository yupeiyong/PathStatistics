using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathStatistics.Properties;
using YPY.Winform.Library.UserControls.DataGridViewColumn;

namespace PathStatistics.UserControl
{
    class DataGridViewPathShowButtonColumn : DataGridViewOneButtonColumn
    {
        public DataGridViewPathShowButtonColumn() : base(Resources.BtnView, Resources.BtnView02)
        {
            HeaderText = "操作";
        }
    }
}