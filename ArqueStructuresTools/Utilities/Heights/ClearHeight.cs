using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Utilities.Heights
{
    class ClearHeight
    {
        public static int ComputeDifference(int maximumHeight, ref int clearHeight, int columnMinimumHeight)
        {
            int difference;
            if (clearHeight >= columnMinimumHeight - 200)
            {
                difference = 200;
            }
            else
            {
                difference = columnMinimumHeight - clearHeight;
            }
            return difference;
        }
    }
}
