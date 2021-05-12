using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Connections
{
    public abstract class Connections
    {
        protected readonly List<Point3d> TopNodes;
        protected readonly List<Point3d> BottomNodes;

        protected Connections(List<Point3d> topNodes, List<Point3d> bottomNodes)
        {
            TopNodes = topNodes;
            BottomNodes = bottomNodes;
        }

        public abstract List<Curve> ConstructConnections();
    }
}