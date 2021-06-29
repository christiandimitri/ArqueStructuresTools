using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.BucklingLengths;
using WarehouseLib.Nodes;
using WarehouseLib.Profiles;

namespace WarehouseLib.Beams
{
    public class Beam
    {
        // <summary>
        // initializes a new instance of the beam's class
        // </summary>
        public Beam()
        {
        }

        // <summary>
        // gets or sets the beam's axis list
        // </summary>
        public List<BeamAxis> Axis { get; set; }


        // <summary>
        // gets or sets the beam's profile orientation plane
        // </summary>
        public Plane ProfileOrientationPlane { get; set; }


        // <summary>
        // gets or sets the string value representing the position of the beam
        // </summary>
        public string Position { get; set; }


        // <summary>
        // gets or sets the beam's Profile description
        // </summary>
        public ProfileDescription Profile;

        // <summary>
        // gets or sets the beam's buckling length list 
        // </summary>
        public List<BucklingLengths.BucklingLengths> BucklingLengths;

        // <summary>
        // gets or sets the beam's nodes
        // </summary>
        public List<Node> Nodes;

        // <summary>
        // Initializes a new instance of the beam's class with a another half-edge beam
        // </summary>
        public Beam(Beam halfEdgeBeam)
        {
            Nodes = halfEdgeBeam.Nodes != null ? new List<Node>(halfEdgeBeam.Nodes) : new List<Node>();
            Axis = halfEdgeBeam.Axis != null ? new List<BeamAxis>(halfEdgeBeam.Axis) : new List<BeamAxis>();
            HalfEdgeAxis = halfEdgeBeam.HalfEdgeAxis != null
                ? new List<BeamAxisHalfEdge>(halfEdgeBeam.HalfEdgeAxis)
                : new List<BeamAxisHalfEdge>();
        }

        // <summary>
        // gets or sets the half-edge axis of the beam
        // </summary>
        public List<BeamAxisHalfEdge> HalfEdgeAxis { get; set; }


        // <summary>
        // returns the Beam's profile orientation plane
        // </summary>
        public Plane GetTeklaProfileOrientationPlane()
        {
            throw new System.NotImplementedException();
        }

        // <summary>
        // returns the buckling length list for each axis of the beam in case the beam is a Portico
        // </summary>
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


        // <summary>
        // returns the buckling length list for each axis of the beam in case the trusses are connected with st Andre crosses
        // </summary>
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

        // <summary>
        // returns the buckling length list between each node for each axis of the beam
        // </summary>
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