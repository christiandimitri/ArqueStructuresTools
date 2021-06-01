using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Profiles;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public abstract class Strap
    {
        public Line Axis;

        public Plane ProfileOrientationPlane;
        public ProfileDescription Profile;
        protected Strap()
        {
        }

        protected abstract Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index,bool isBoundary);
    }
}