using System.Collections.Generic;
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