using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Connections
{
    public abstract class Connections
    {
        protected readonly List<Point3d> TopNodes;
        protected readonly List<Point3d> BottomNodes;
        protected readonly string _articulationType;

        protected Connections(List<Point3d> topNodes, List<Point3d> bottomNodes, string articulationType)
        {
            TopNodes = topNodes;
            BottomNodes = bottomNodes;
            _articulationType = articulationType;
        }

        public int MidPointIndex;

        public abstract List<Curve> ConstructConnections();
    }
}