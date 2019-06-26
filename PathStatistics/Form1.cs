using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PathStatistics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            //键值对，键为每个节点代表的字典，值为每个节点的值
            var pathParameters = new Dictionary<char, double>();

            //节点字母从A到M
            for (var i = 'A'; i <= 'M'; i++)
            {
                //通过名称获取文本框，每个文本框有输入的值
                var textbox = groupBox1.Controls["txt" + i] as TextBox;
                if (textbox == null)
                    throw new Exception("控件错！");

                var text = textbox.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    pathParameters[i] = 0;
                }
                else
                {
                    double v = 0;
                    //文本框的字符串转为双精度小数
                    if (!double.TryParse(text, out v)) throw new Exception("必须输入小数或整数！");

                    pathParameters[i] = v;
                }
            }

            //实例化第二个窗体，并将节点值传递给窗体
            var frm = new Form2();
            frm.PathParameters = pathParameters;

            //显示窗体
            frm.ShowDialog(this);
        }
    }
}