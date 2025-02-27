using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MouseAutoClick
{
    public partial class Form1 : Form
    {
        private int clickCount = 0;
        private MouseHook mouseHook;
        private MouseClicker clicker; // 声明点击器实例

        private bool isMonitoringSideButtons = false; // 新增：标记是否正在监听侧键

        public Form1()
        {
            InitializeComponent();
            mouseHook = new MouseHook();
            // 订阅事件
            mouseHook.MousePositionCaptured += MouseHook_MousePositionCaptured;
            clicker = new MouseClicker(); // 在构造函数中初始化 clicker

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)//选择点击的两个坐标
        {
            clickCount = 0;
            comboBox1.Text = "";
            comboBox2.Text = "";
            button1.Enabled = false;
            mouseHook.IsCapturing = true;
            mouseHook.StartHook();
            infoBox.Text += "点击屏幕选择两个坐标！\r\n";
        }

        private void button2_Click(object sender, EventArgs e)//监听按钮
        {
            try
            {
                if (!isMonitoringSideButtons)
                {
                    // 从文本框获取参数
                    string point1 = comboBox1.Text;
                    string point2 = comboBox2.Text;
                    int count = int.Parse(textBox3.Text);
                    int interval = int.Parse(textBox4.Text);

                    // 开始监听
                    isMonitoringSideButtons = true;
                    mouseHook.MouseSideButtonPressed += MouseHook_MouseSideButtonPressed;
                    mouseHook.StartHook();
                    button2.Text = "停止监听"; // 更改按钮文字
                    button1.Enabled=false;//监听过程中静止选择坐标

                    infoBox.Text += "已开始监听鼠标侧键，按下侧键触发点击\r\n";

                    // 存储参数到字段
                    this.Tag = new ClickParameters(point1, point2, count, interval);
                }
                else
                {
                    // 停止监听
                    isMonitoringSideButtons = false;
                    mouseHook.MouseSideButtonPressed -= MouseHook_MouseSideButtonPressed;
                    mouseHook.StopHook();
                    button2.Text = "开始点击"; // 恢复按钮文字
                    button1.Enabled = true;//恢复坐标选择
                    infoBox.Text += "已停止监听鼠标侧键\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入参数错误: " + ex.Message);
            }
        }

        private void MouseHook_MouseSideButtonPressed(object sender, MouseSideButtonEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MouseHook_MouseSideButtonPressed(sender, e)));
                return;
            }

            if (isMonitoringSideButtons && this.Tag is ClickParameters parameters)
            {
                // 执行点击
                clicker.ClickTwoPoints(
                    parameters.Point1,
                    parameters.Point2,
                    parameters.Count,
                    parameters.Interval
                );
                infoBox.Text += $"侧键{e.ButtonId}触发点击\r\n";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mouseHook.StopHook(); // 确保程序关闭时移除钩子
        }

        private void MouseHook_MousePositionCaptured(object sender, MousePositionEventArgs e)
        {
            if (InvokeRequired)
            {
                // 如果不在UI线程，切换到UI线程
                Invoke(new Action(() => MouseHook_MousePositionCaptured(sender, e)));
                return;
            }

            if (clickCount == 0)
            {
                //textBox1.Text = $"{e.X},{e.Y}";
                comboBox1.Text = $"{e.X},{e.Y}";
                clickCount++;
            }
            else if (clickCount == 1)
            {
                //textBox2.Text = $"{e.X},{e.Y}";
                comboBox2.Text = $"{e.X},{e.Y}";
                //infoBox.Text += $"已添加两次点击坐标（{textBox1.Text}）;({textBox2.Text})\r\n";
                infoBox.Text += $"已添加两次点击坐标（{comboBox1.Text}）;({comboBox2.Text})\r\n";//修改为下拉框

                clickCount++;
                mouseHook.IsCapturing = false;
                button1.Enabled = true;
                this.Cursor = Cursors.Default;
                mouseHook.StopHook();
            }
        }
    }
}