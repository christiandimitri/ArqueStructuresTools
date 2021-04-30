using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib
{
    public abstract class Strap
    {
        public Line Axis;
        protected Strap(Line axis)
        {
            Axis = axis;
        }

        public abstract List<Strap> ConstructStrapsAxis(List<Truss> trusses, double distance);

    }
}