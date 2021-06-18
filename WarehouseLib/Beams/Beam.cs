﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<BucklingLengths.BucklingLengths> ComputePorticoBeamBucklingLengths(Beam beam, bool bucklingActive)
        {
            var bucklings = new List<BucklingLengths.BucklingLengths>();
            var buckling = new BucklingLengths.BucklingLengths();

            if (bucklingActive)
            {
                var lengths = new List<double>();
                var beamLength = Curve.JoinCurves(beam.Axis,0.01)[0].GetLength();
                for (int i = 0; i < beam.Axis.Count; i++)
                {
                    var axis = beam.Axis[i];
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = beamLength;
                    bucklings.Add(buckling);
                }
            }

            return bucklings;
        }

        public List<BucklingLengths.BucklingLengths> SetTrussBeamBucklingLengthsBetweenStAndresCrosses(double distance,
            Beam beam)
        {
            return null;
        }
        public List<BucklingLengths.BucklingLengths> ComputeTrussBeamBucklingLengthsBetweenNodes(Beam beam, bool bucklingActive)
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
                    var axis = beam.Axis[i];
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = axis.GetLength();
                    bucklings.Add(buckling);
                }
            }

            return bucklings;
        }
    }
}