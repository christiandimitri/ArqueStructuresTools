using System.Collections.Generic;
using System.Web.Configuration;
using Rhino.Geometry;
using WarehouseLib.Profiles;

namespace WarehouseLib.Beams
{
    public abstract class Beam
    {
        public List<Curve> Axis { get; set; }

        public Plane ProfileOrientationPlane { get; set; }

        public ProfileDescription Profile;
        protected Beam()
        {
        }
        public abstract Plane GetTeklaProfileOrientationPlane();
    }
}