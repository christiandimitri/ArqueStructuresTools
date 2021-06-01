using System.Collections.Generic;
using System.Web.Configuration;
using Rhino.Geometry;
using WarehouseLib.Profiles;

namespace WarehouseLib.Beams
{
    public abstract class Beam
    {
        public List<Curve> Axis;

        public Plane ProfileOrientationPlane;

        public ProfileDescription Profile;
        protected Beam()
        {
        }
        public abstract Plane GetTeklaProfileOrientationPlane();
    }
}