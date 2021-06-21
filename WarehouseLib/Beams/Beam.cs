using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.BucklingLengths;
using WarehouseLib.Nodes;
using WarehouseLib.Profiles;

namespace WarehouseLib.Beams
{
    public abstract class Beam
    {
        public List<Axis> Axis;

        public Plane ProfileOrientationPlane;

        public ProfileDescription Profile;

        public List<BucklingLengths.BucklingLengths> BucklingLengths;
        public List<Node> Nodes;

        protected Beam()
        {
        }

        public abstract Plane GetTeklaProfileOrientationPlane();

        public List<BucklingLengths.BucklingLengths> ComputePorticoBeamBucklingLengths(Beam beam, bool bucklingActive)
        {
            var bucklings = new List<BucklingLengths.BucklingLengths>();
            var buckling = new BucklingLengths.BucklingLengths();

            if (bucklingActive)
            {
                var lengths = new List<double>();
                var beamAxis = new List<Curve>();

                for (int i = 0; i < beam.Axis.Count; i++)
                {
                    beamAxis.Add(beam.Axis[i].AxisCurve);
                }

                var beamLength = Curve.JoinCurves(beamAxis, 0.01)[0].GetLength();
                for (int i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i].AxisCurve;
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = beamLength;
                    bucklings.Add(buckling);
                }
            }

            return bucklings;
        }

        public List<BucklingLengths.BucklingLengths> SetTrussBeamBucklingLengthsBetweenStAndresCrosses(Beam beam,
            List<double> distances)
        {
            var buckling = new BucklingLengths.BucklingLengths();
            var bucklings = new List<BucklingLengths.BucklingLengths>();

            for (int i = 0; i < beam.Axis.Count; i++)
            {
                buckling.BucklingY = distances != null ? distances[i] : double.NaN;
                buckling.BucklingZ = beam.Axis[i].AxisCurve.GetLength();
                bucklings.Add(buckling);
            }

            return bucklings;
        }

        public List<BucklingLengths.BucklingLengths> ComputeTrussBeamBucklingLengthsBetweenNodes(Beam beam,
            bool bucklingActive)
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
            else
            {
                for (var i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i].AxisCurve;
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = axis.GetLength();
                    bucklings.Add(buckling);
                }
            }

            return bucklings;
        }
    }
}