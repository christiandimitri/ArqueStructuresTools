using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Crosses
{
    public abstract class Cross
    {
        public List<Line> Axis;
        public ProfileDescription Profile;
        public List<Plane> ProfileOrientationPlanes;

        protected Cross()
        {
        }

        public abstract List<Cross> ConstructCrosses(List<Point3d> outerTopNodes, List<Point3d> innerBottomNodes,
            List<Point3d> outerBottomNodes, List<Point3d> innerTopNodes);

        public abstract List<Point3d> ComputeCrossTopNodes(Truss truss, int count);
        public abstract List<Point3d> ComputeCrossBottomNodes(Truss truss, List<Point3d> topNodes);
        public abstract void AddTeklaProfileOrientationPlane(StAndre cross);
    }
}