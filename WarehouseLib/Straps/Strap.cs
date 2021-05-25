using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public abstract class Strap
    {
        public Line Axis;

        public Plane ProfileOrientationPlane;
        protected Strap()
        {
        }

        protected abstract Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index,bool isBoundary);
    }
}