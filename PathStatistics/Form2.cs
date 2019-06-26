using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PathStatistics.Models;
using PathStatistics.Properties;
using PathStatistics.UserControl;
using YPY.Winform.Library.ControlExtend;
using YPY.Winform.Library.UserControls;
using YPY.Winform.Library.UserControls.DataGridViewColumn;

namespace PathStatistics
{
    public partial class Form2 : Form
    {
        /// <summary>
        /// 键值对，代表每个节点对应的中文描述
        /// </summary>
        private Dictionary<char, string> PathDescription = new Dictionary<char, string>
        {
            {'A', "围墙"},
            {'B', "电动滑门"},
            {'C', "穿过控制区"},
            {'D', "双层围网"},
            {'E', "车辆出入口"},
            {'F', "人员出入口"},
            {'G', "穿过保护区"},
            {'H', "建筑物外墙"},
            {'I', "带门禁控制的甲级安全防盗门"},
            {'J', "穿过建筑物内部"},
            {'K', "20cm厚钢筋混凝土"},
            {'L', "带门禁控制的甲级安全防盗门"},
            {'M', "目标封套"}
        };

        /// <summary>
        /// 接收从第一窗体传过来的节点参数
        /// </summary>
        public Dictionary<char, double> PathParameters;

        public Form2()
        {
            InitializeComponent();

            //隐藏小人图片框
            pictureBoxPerson.Visible = false;

            SetPathDataGridViewColumns();
        }

        private void SetPathDataGridViewColumns()
        {
            DataGridViewHelper.SetColumns<PathParameter>(dataGridView1,
                new List<string>
                {
                    nameof(PathParameter.ProbabilityValue),
                });

            var actionColumn = new DataGridViewPathShowButtonColumn
            {
                DataPropertyName = nameof(PathParameter.Index),
                HeaderText = "操作",
                Name = "ColAction",
                ReadOnly = true,
                Resizable = DataGridViewTriState.False,
                Width = 100
            };
            dataGridView1.Columns.Add(actionColumn);
        }

        private List<PathParameter> _pathParameters;
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Load(object sender, EventArgs e)
        {
            if (PathParameters == null)
                throw new Exception("计算参数为空！");

            #region 路径计算

            //初始化12条路径
            _pathParameters = new List<PathParameter>
            {
                new PathParameter {Index = 1, Route = "ADH"},
                new PathParameter {Index = 2, Route = "ADI"},
                new PathParameter {Index = 3, Route = "AEH"},
                new PathParameter {Index = 4, Route = "AEI"},
                new PathParameter {Index = 5, Route = "AFH"},
                new PathParameter {Index = 6, Route = "AFI"},
                new PathParameter {Index = 7, Route = "BDH"},
                new PathParameter {Index = 8, Route = "BDI"},
                new PathParameter {Index = 9, Route = "BEH"},
                new PathParameter {Index = 10, Route = "BEI"},
                new PathParameter {Index = 11, Route = "BFH"},
                new PathParameter {Index = 12, Route = "BFI"}
            };


            //计算每条路径的概率值
            for (var i = 0; i < _pathParameters.Count; i++)
            {
                var parameter = _pathParameters[i];
                var route = parameter.Route;
                var sum = 1 - (1 - PathParameters[route[0]]) * (1 - PathParameters[route[1]]) *
                          (1 - PathParameters[route[2]]);

                parameter.ProbabilityValue = sum;

                parameter.Probability =
                    $"1-{1 - PathParameters[route[0]]}×{1 - PathParameters[route[1]]}×{1 - PathParameters[route[2]]}={sum}";
            }

            //计算结果绑定到DataGridView,用于展示数据
            dataGridView1.DataSource = _pathParameters;


            #endregion

            #region 展示最小路径
            //取最小的那一条,按概率值升序排列，并取第一条
            var minPath = _pathParameters.OrderBy(p => p.ProbabilityValue).FirstOrDefault();
            if (minPath == null)
                throw new Exception("最小路径为空！");

            //路径再加上固定的LM节点，共5个节点
            var minRoutes = minPath.Route.ToList();
            minRoutes.Add('L');
            minRoutes.Add('M');

            //路径节点的中文描述列表
            var nodeDescriptions = new List<string>()
            {
                string.Join("-",minRoutes),
                PathDescription[minRoutes[0]],
                PathDescription[minRoutes[1]],
                PathDescription[minRoutes[2]],
                PathDescription[minRoutes[3]],
                PathDescription[minRoutes[4]]
            };
            //A-F-H-L-M 是截住概率最小的路径，实物保护系统最薄弱的路径是:围墙-人员出入口-建筑物外墙-甲级安全防盗门-目标封套。
            var minPathDescription = $"{nodeDescriptions[0]} 是截住概率最小的路径，实物保护系统最薄弱的路径是:{string.Join("-", nodeDescriptions.GetRange(1, 5))}。";
            richTextBox1.Text = minPathDescription;

            ChangeKeyColor(nodeDescriptions, Color.Red);

            #endregion

        }


        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //dataGridView1.Columns[0].HeaderText = "序号";
            //dataGridView1.Columns[1].HeaderText = "路径";
            //dataGridView1.Columns[2].HeaderText = "截住概率";
            dataGridView1.Columns[0].Width = 70;
            dataGridView1.Columns[1].Width = 70;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Visible = false;
        }

        public void ChangeKeyColor(string key, Color color)
        {
            Regex regex = new Regex(key);
            //找出内容中所有的要替换的关键字
            MatchCollection collection = regex.Matches(richTextBox1.Text);
            //对所有的要替换颜色的关键字逐个替换颜色
            foreach (Match match in collection)
            {
                //开始位置、长度、颜色缺一不可
                richTextBox1.SelectionStart = match.Index;
                richTextBox1.SelectionLength = key.Length;
                richTextBox1.SelectionColor = color;
                richTextBox1.SelectionFont = new Font(new FontFamily("宋体"), 14);
            }
        }
        public void ChangeKeyColor(List<string> list, Color color)
        {
            foreach (string str in list)
            {
                ChangeKeyColor(str, color);
            }
        }

        #region 显示路径相关

        private PathParameter _currentPath;

        private Dictionary<char, double> _walkPathValues { get; set; } = new Dictionary<char, double>();
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //用户单击DataGridView“操作”列中的“查看”按钮。
            if (DataGridViewOneButtonCell.IsButtonClick(sender, e))
            {
                var pathIndex = Convert.ToInt32(dataGridView1["ColAction", e.RowIndex].Value); // 获取所要删除关联对象的主键。
                if (_currentPath != null)
                {
                    //恢复为黑色
                    dataGridView1.Rows[_currentPath.Index - 1].Cells[1].Style.ForeColor = Color.Black;
                    dataGridView1.Rows[_currentPath.Index - 1].Cells[2].Style.ForeColor = Color.Black;
                }
                _currentPath = _pathParameters.FirstOrDefault(p => p.Index == pathIndex);
                //路径演示开始
                PathShowStart();
            }
        }

        /// <summary>
        /// 路径演示开始
        /// </summary>
        private void PathShowStart()
        {
            //当前路径的路径字体颜色为红色
            dataGridView1.Rows[_currentPath.Index - 1].Cells[1].Style.ForeColor = Color.Red;

            //路径再加上固定的LM节点，共5个节点
            var routes = _currentPath.Route.ToList();
            routes.Add('L');
            routes.Add('M');

            //按路径节点初始化展示路径
            PathInit(routes);

            //指定小人开始位置
            pictureBoxPerson.Location = _showers[0].StartPoint;

            //背景透明
            pictureBoxPerson.BackgroundToTransparent();
            pictureBoxPerson.BringToFront();
            pictureBoxPerson.Visible = true;

            timer1.Enabled = true;
        }

        private List<PathShower> _showers = new List<PathShower>();

        private PathShower _currentShower;


        /// <summary>
        /// 路径清除
        /// </summary>
        private void PathClear()
        {
            pictureBoxPerson.Visible = false;
            for (var i = _showers.Count - 1; i >= 0; i--)
            {
                var pathShower = _showers[i];
                if (pathShower.LineX != null)
                {
                    pathShower.LineX.Dispose();
                    pathShower.LineX = null;
                }

                if (pathShower.LineY != null)
                {
                    pathShower.LineY.Dispose();
                    pathShower.LineY = null;
                }
            }

            //清空所有元素
            _showers.Clear();
        }

        /// <summary>
        /// 显示路径初始化
        /// </summary>
        private void PathInit(List<char> routes)
        {
            PathClear();

            //清除走过路径的值
            _walkPathValues.Clear();

            var index = 0;
            for (var i = 0; i < routes.Count; i++)
            {
                var controls = groupBox1.Controls.Find("panel" + routes[i].ToString(), false);
                if (controls.Length == 0)
                    throw new Exception("panel" + routes[i].ToString() + "的控件不存在！");

                var panel = controls[0] as Panel;
                if (panel == null)
                    throw new Exception("panel" + routes[i].ToString() + "的控件不存在！");

                //panel的中心坐标
                var panelCenterPoint = new Point(panel.Left + panel.Width / 2, panel.Top + panel.Height / 2);

                //设置上一路径结束坐标
                if (i > 0)
                {
                    var previousShower = _showers[index - 1];
                    //x轴=当前节点的x轴
                    var endPoint = new Point(panel.Left + panel.Width / 2, previousShower.StartPoint.Y);
                    if (endPoint.X > previousShower.StartPoint.X)
                    {
                        //向右移动
                        previousShower.LeftOfRight = Direction.Right;
                    }
                    else
                    {
                        //向左移动
                        previousShower.LeftOfRight = Direction.Left;
                    }

                    previousShower.EndPoint = endPoint;
                }
                //不是最后节点
                if (i < routes.Count - 1)
                {
                    //竖线
                    var lineY = new LineY { LineColor = Color.Red, LineWidth = 3, Width = 0, Height = 0 };
                    groupBox1.Controls.Add(lineY);
                    lineY.BringToFront();
                    var shower = new PathShower
                    {
                        PathName = routes[i],
                        Index = index++,
                        LineY = lineY,
                        VerticalDirection = Direction.Vertical
                    };
                    if (i == 0)
                    {
                        shower.StartPoint = panelCenterPoint;
                    }
                    else
                    {
                        shower.StartPoint = _showers[index - 2].EndPoint;
                    }
                    lineY.Location = shower.StartPoint;
                    shower.CurrentPoint = shower.StartPoint;
                    _showers.Add(shower);
                    //寻找节点框图底部的panel
                    Panel bottomPanel = null;
                    foreach (var ct in groupBox1.Controls)
                    {
                        var pnl = ct as Panel;
                        if (pnl != null && pnl.Name != "panel" + routes[i].ToString())
                        {
                            var pName = pnl.Name;
                            pName = pName.Replace("panel", "");
                            if (pName.Contains(routes[i]))
                            {
                                bottomPanel = pnl;
                            }
                        }
                    }

                    if (bottomPanel == null)
                        throw new Exception("未找到底部Panel!");

                    var bottomMiddle = bottomPanel.Top + bottomPanel.Height / 2;
                    //节点框图的结束坐标
                    shower.EndPoint = new Point(shower.StartPoint.X, bottomMiddle);

                    //添加横线
                    var lineX = new LineX { LineColor = Color.Red, LineWidth = 3, Width = 0, Height = 3 };
                    groupBox1.Controls.Add(lineX);
                    lineX.BringToFront();

                    var bottomShower = new PathShower
                    {
                        PathName = ' ',//路径名称为空字符
                        Index = index++,
                        StartPoint = shower.EndPoint,//开始坐标等于上一路径的结束坐标
                        VerticalDirection = Direction.Horizontal,//水平方向移动
                        LineX = lineX
                    };
                    lineX.Location = bottomShower.StartPoint;
                    bottomShower.CurrentPoint = bottomShower.StartPoint;
                    _showers.Add(bottomShower);
                }
                else//最后一个节点
                {
                    //竖线
                    var lineY = new LineY { LineColor = Color.Red, LineWidth = 3, Width = 0, Height = 0 };

                    groupBox1.Controls.Add(lineY);
                    lineY.BringToFront();

                    var shower = new PathShower
                    {
                        PathName = routes[i],
                        Index = index++,
                        LineY = lineY,
                        VerticalDirection = Direction.Vertical,
                        StartPoint = _showers[index - 2].EndPoint,
                        EndPoint = panelCenterPoint
                    };
                    shower.CurrentPoint = shower.StartPoint;
                    lineY.Location = shower.StartPoint;
                    _showers.Add(shower);
                }
            }

            _currentShower = _showers[0];
        }
        /// <summary>
        /// 小人每次移动像素大小
        /// </summary>
        private int MoveStep = 1;

        /// <summary>
        /// 定时器，实现小人和线条按路径移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_showers.Count == 0)
            {
                timer1.Enabled = false;
            }

            var isPathEnd = false;

            //垂直向下移动
            if (_currentShower.VerticalDirection == Direction.Vertical)
            {
                _currentShower.CurrentPoint = new Point(_currentShower.CurrentPoint.X, _currentShower.CurrentPoint.Y + MoveStep);
                //小人重新定位
                pictureBoxPerson.Location = new Point(_currentShower.CurrentPoint.X - pictureBoxPerson.Width / 2, _currentShower.CurrentPoint.Y + MoveStep - pictureBoxPerson.Height / 2);
                //线条高度加1
                _currentShower.LineY.Height += MoveStep;
                //已经移动到终点
                if (_currentShower.CurrentPoint.Y >= _currentShower.EndPoint.Y)
                {
                    isPathEnd = true;
                }
            }
            else//水平移动
            {
                //向左移动
                if (_currentShower.LeftOfRight == Direction.Left)
                {
                    _currentShower.CurrentPoint = new Point(_currentShower.CurrentPoint.X - MoveStep, _currentShower.CurrentPoint.Y);
                    _currentShower.LineX.Location = _currentShower.CurrentPoint;
                    //线条宽度加
                    _currentShower.LineX.Width += MoveStep;
                    //已经移动到终点
                    if (_currentShower.CurrentPoint.X <= _currentShower.EndPoint.X)
                    {
                        isPathEnd = true;
                    }
                }
                else//向右移动
                {
                    _currentShower.CurrentPoint = new Point(_currentShower.CurrentPoint.X + MoveStep, _currentShower.CurrentPoint.Y);
                    //线条宽度加
                    _currentShower.LineX.Width += MoveStep;
                    //已经移动到终点
                    if (_currentShower.CurrentPoint.X >= _currentShower.EndPoint.X)
                    {
                        isPathEnd = true;
                    }
                }
                //小人重新定位
                pictureBoxPerson.Location = new Point(_currentShower.CurrentPoint.X - pictureBoxPerson.Width / 2, _currentShower.CurrentPoint.Y + MoveStep - pictureBoxPerson.Height / 2);// _currentShower.CurrentPoint;
            }

            //是否已经走到当前路径终点
            if (isPathEnd)
            {
                //已经是最后一个展示则结束本次路径演示，否则取下一条路径，继续展示
                if (_currentShower.Index + 1 >= _showers.Count)
                {
                    timer1.Enabled = false;
                    return;
                }

                if (_currentShower.PathName != ' ')
                {
                    //记录走过路径的概率值
                    _walkPathValues[_currentShower.PathName] = PathParameters[_currentShower.PathName];
                    CalculatePath();
                }
                _currentShower = _showers[_currentShower.Index + 1]; 
            }
        }

        /// <summary>
        /// 更新路径计算结果
        /// </summary>
        private void CalculatePath()
        {
            //当前演示的路径
            var path = _currentPath;

            //路径节点
            var route = path.Route;

            //记录前三个节点的计算结果
            var values = new double[3];
            for (var i = 0; i < values.Length; i++)
            {
                var n = route[i];
                if (_walkPathValues.ContainsKey(n))
                {
                    values[i] = _walkPathValues[n];
                }
            }
            //概率结果
            var sum = 1 - (1 - values[0]) * (1 - values[1]) * (1 - values[2]);

            //显示路径计算详情
            var probability = $"1-{1 - values[0]}×{1 - values[1]}×{1 - values[2]}={sum}";

            dataGridView1.Rows[path.Index - 1].Cells[2].Value = probability;
            dataGridView1.Rows[path.Index - 1].Cells[2].Style.ForeColor = Color.Red;
        }
        #endregion
    }



}