using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Cables
{
    public abstract class Cable
    {
        public Line Axis;
        public ProfileDescription Profile;
        public Plane ProfileOrientationPlane;

        protected Cable()
        {
        }

        public abstract List<Cable> ConstructCables(List<Point3d> nodes, Curve beam, Plane plane, int index);

        protected abstract Plane GetTeklaProfileOrientationPlane(Curve beam, Point3d position, Plane plane,
            int index);
    }
}