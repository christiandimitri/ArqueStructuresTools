using System.Collections.Generic;
using System.Web.Configuration;
using Rhino.Geometry;

namespace WarehouseLib.Beams
{
    public abstract class Beam
    {
        public List<Curve> Axis { get; set; }

        public Plane ProfileOrientationPlane { get; set; }
        
        protected Beam()
        {
        }
        public abstract Plane GetTeklaProfileOrientationPlane();
    }
}