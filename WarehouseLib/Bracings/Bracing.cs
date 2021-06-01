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

        public abstract List<Bracing> ConstructBracings(List<Point3d> nodes, Curve beam);
    }
}