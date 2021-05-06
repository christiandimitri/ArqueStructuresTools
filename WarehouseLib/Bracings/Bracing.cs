using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public abstract class Bracing
    {
        public Line Axis;

        protected Bracing(Line axis)
        {
            Axis = axis;
        }

        public abstract List<Bracing> ConstructBracings(List<Truss> trusses);
    }
}