using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Monopich.Heights
{
    class ClearHeight
    {
        public static int ComputeDifference(ref int clearHeight, int columnMinimumHeight)
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
