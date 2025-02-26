using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAutoClick
{
    public class ClickParameters
    {
        public string Point1 { get; }
        public string Point2 { get; }
        public int Count { get; }
        public int Interval { get; }

        public ClickParameters(string point1, string point2, int count, int interval)
        {
            Point1 = point1;
            Point2 = point2;
            Count = count;
            Interval = interval;
        }
    }
}
