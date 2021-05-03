using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Bracings
{
    public abstract class Bracing
    {
        public Line Axis;

        public Bracing(Line axis)
        {
            Axis = axis;
        }
    }
}