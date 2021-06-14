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
            double stAndreCrossDistance, bool bucklingActive)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var bucklings = new List<BucklingLengths.BucklingLengths>();
            if (!bucklingActive)
            {
                for (var i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i];
                    buckling.BucklingY = 0.0;
                    buckling.BucklingZ = 0.0;
                    bucklings.Add(buckling);
                }
            }
            else if (!stAndreCross && bucklingActive)
            {
                for (var i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i];
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = axis.GetLength();
                    bucklings.Add(buckling);
                }
            }
            else if (stAndreCross && bucklingActive)
            {
                for (var i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i];
                    buckling.BucklingY = stAndreCrossDistance;
                    buckling.BucklingZ = axis.GetLength();
                    bucklings.Add(buckling);
                }
            }
            return bucklings;
        }
    }
}