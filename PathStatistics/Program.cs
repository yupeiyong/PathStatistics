using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsharpHttpHelper;

namespace PathStatistics
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var httpItem = new HttpItem
            {
                URL = "http://www.inloveu.cn/yz/DoctorQuery.txt"
            };

            var httpHelper = new HttpHelper();
            var result = httpHelper.GetHtml(httpItem);
            if (result.StatusCode == 0)
            {
                MessageBox.Show("网络无法连接！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            var content = result.Html;
            if (content != "1")
            {
                MessageBox.Show("系统问题，请联系作者！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
