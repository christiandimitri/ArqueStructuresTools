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

        public List<BucklingLengths.BucklingLengths> BucklingLengths;

        protected Beam()
        {
        }

        public abstract Plane GetTeklaProfileOrientationPlane();


        public List<BucklingLengths.BucklingLengths> ComputeBucklingLengths(Beam beam, bool stAndreCross,
            double stAndreCrossDistance)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var bucklings = new List<BucklingLengths.BucklingLengths>();
            foreach (var axis in beam.Axis)
            {
                buckling.BucklingY = axis.GetLength();
                buckling.BucklingZ = axis.GetLength();
                bucklings.Add(buckling);
            }

            return bucklings;
        }
    }
}