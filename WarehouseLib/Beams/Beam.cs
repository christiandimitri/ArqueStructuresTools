using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.BucklingLengths;
using WarehouseLib.Profiles;

namespace WarehouseLib.Beams
{
    public abstract class Beam
    {
        public List<Curve> Axis;

        public Plane ProfileOrientationPlane;

        public ProfileDescription Profile;

        public BucklingLengths.BucklingLengths BucklingLengths;
        protected Beam()
        {
        }
        public abstract Plane GetTeklaProfileOrientationPlane();
    }
}