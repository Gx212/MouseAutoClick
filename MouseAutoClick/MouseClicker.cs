using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseAutoClick
{
    internal class MouseClicker
    {
        // Windows API函数导入
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        // 点击两个点的核心方法
        public void ClickTwoPoints(string point1, string point2, int count, int interval)
        {
            try
            {
                // 解析坐标
                string[] pos1 = point1.Split(',');
                string[] pos2 = point2.Split(',');

                int x1 = int.Parse(pos1[0].Trim());
                int y1 = int.Parse(pos1[1].Trim());
                int x2 = int.Parse(pos2[0].Trim());
                int y2 = int.Parse(pos2[1].Trim());

                // 在新线程中执行点击
                Thread clickThread = new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        ClickAtPoint(x1, y1);
                        Thread.Sleep(interval); // 两次点击间的短暂间隔推荐100毫秒
                        ClickAtPoint(x2, y2);
                        Thread.Sleep(interval);
                    }
                });
                clickThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误: " + ex.Message + "\n坐标格式应为: X,Y (例如: 100,200)");
            }
        }

        // 私有辅助方法：点击指定点
        private void ClickAtPoint(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
