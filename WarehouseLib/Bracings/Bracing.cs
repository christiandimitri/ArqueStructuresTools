using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Bracings
{
    public abstract class Bracing
    {
        public Line Axis;
        public string Type;
        public Bracing(Line axis, string type)
        {
            Axis = axis;
            Type = type;
        }
    }
}