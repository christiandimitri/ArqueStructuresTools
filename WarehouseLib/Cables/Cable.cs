using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;

namespace WarehouseLib.Cables
{
    public abstract class Cable
    {
        public Line Axis;

        protected Cable(Line axis)
        {
            Axis = axis;
        }

        public abstract List<Cable> ConstructCables(List<Truss> trusses);
    }
}