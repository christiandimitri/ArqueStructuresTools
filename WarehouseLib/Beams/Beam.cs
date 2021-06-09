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


        public BucklingLengths.BucklingLengths ComputeBucklingLengths(Beam beam, bool stAndreCross,
            double stAndreCrossDistance)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var variable = 1;
            foreach (var axis in beam.Axis)
            {
                buckling.BucklingY = axis.GetLength() * variable;
                buckling.BucklingZ = axis.GetLength() * variable;
            }

            return buckling;
        }
    }
}