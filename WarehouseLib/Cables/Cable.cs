using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;

namespace WarehouseLib.Cables
{
    public class Cable
    {
        public Line Axis;

        public Cable(Line axis)
        {
            Axis = axis;
        }
    }
}