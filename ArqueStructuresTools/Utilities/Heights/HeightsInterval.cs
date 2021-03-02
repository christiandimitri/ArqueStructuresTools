using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Utilities.Heights
{
    class HeightsInterval
    {
        public static int[] Interval(int leftHeight, int rightHeight)
        {
            int[] heights = new int[] { leftHeight, rightHeight };
            return heights;
        }
        public static int Min(int[]heights)
        {
            return heights.Min();
        }

        public static int IndexOfMin(int[] heights, int min)
        {
            return Array.IndexOf(heights, min);
        }

        public static int Max(int[] heights)
        {
            return heights.Max();
        }

        public static int IndexOfMax(int[] heights, int max)
        {
            return Array.IndexOf(heights, max);
        }

    }
}
