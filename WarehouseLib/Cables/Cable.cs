using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public abstract class Cable
    {
        public Line Axis;

        protected Cable()
        {
        }

        public abstract List<Cable> ConstructCables(List<Point3d> nodes, Curve beam);
    }
}