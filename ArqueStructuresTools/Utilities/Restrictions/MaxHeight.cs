using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools.Utilities.Restrictions
{
    class MaxHeight
    {
        public static int Recompute(ref int maxHeight, int maxColumnHeight, int threshold)
        {
            if(maxHeight<= maxColumnHeight)
            {
                return maxHeight = maxColumnHeight + threshold;
            }
            else return maxHeight;
        }

     
    }
}
