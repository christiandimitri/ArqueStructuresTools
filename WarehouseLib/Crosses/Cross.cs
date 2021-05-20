using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Crosses
{
    public abstract class Cross
    {
        public List<Line> Axis;

        protected Cross()
        {
        }

        public abstract Cross ConstructCross(List<Point3d> outsideNodes, List<Point3d> insideNodes);
    }
}