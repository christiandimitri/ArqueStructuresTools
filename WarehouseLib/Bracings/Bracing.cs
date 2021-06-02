using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public abstract class Bracing
    {
        public Line Axis;
        public ProfileDescription Profile;
        public Plane ProfileOrientationPlane;

        protected Bracing()
        {
        }

        protected abstract Plane GetTeklaProfileOrientationPlane(Curve beam, Point3d position, Plane plane,
            int index);

        public abstract List<Bracing> ConstructBracings(List<Point3d> nodes, Curve beam, Plane plane, int index);
    }
}