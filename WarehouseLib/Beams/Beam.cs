using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Articulations;
using WarehouseLib.BucklingLengths;
using WarehouseLib.Connections;
using WarehouseLib.Nodes;
using WarehouseLib.Options;
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
        // gets or sets the beam's skeleton axis list
        // </summary>
        public List<BeamAxis> SkeletonAxis { get; set; }

        // <summary>
        // gets or sets the beam's axis list between nodes
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

        public List<Point3d> GetNodesPoints()
        {
            var points = new List<Point3d>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                points.Add(Nodes[i].Position);
            }

            return points;
        }

        // constructs tekla beam axis, nodes and properties
        public Beam SetBeamAxisNodes(List<BeamAxis> axis, List<Node> nodes, List<BeamAxis> skeletonAxis)
        {
            var beam = new Beam();
            beam.SkeletonAxis = new List<BeamAxis>(axis);
            beam.Nodes = nodes;
            beam.SkeletonAxis = skeletonAxis;
            return beam;
        }


        // <summary>
        // Initializes a new instance of the beam's class with a another half-edge beam
        // </summary>
        public Beam(Beam halfEdgeBeam)
        {
            Nodes = halfEdgeBeam.Nodes != null ? new List<Node>(halfEdgeBeam.Nodes) : new List<Node>();
            SkeletonAxis = halfEdgeBeam.SkeletonAxis != null
                ? new List<BeamAxis>(halfEdgeBeam.SkeletonAxis)
                : new List<BeamAxis>();
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

                for (int i = 0; i < beam.SkeletonAxis.Count; i++)
                {
                    beamAxis.Add(beam.SkeletonAxis[i].AxisCurve);
                }

                var beamLength = Curve.JoinCurves(beamAxis, 0.01)[0].GetLength();
                for (int i = 0; i < beam.SkeletonAxis.Count; i++)
                {
                    var axis = beam.SkeletonAxis[i].AxisCurve;
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

            for (int i = 0; i < beam.SkeletonAxis.Count; i++)
            {
                buckling.BucklingY = distances != null ? distances[i] : double.NaN;
                buckling.BucklingZ = beam.SkeletonAxis[i].AxisCurve.GetLength();
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
                for (var i = 0; i < beam.SkeletonAxis.Count; i++)
                {
                    var axis = beam.SkeletonAxis[i];
                    buckling.BucklingY = 0.0;
                    buckling.BucklingZ = 0.0;
                    bucklings.Add(buckling);
                }
            }
            else
            {
                for (var i = 0; i < beam.SkeletonAxis.Count; i++)
                {
                    var axis = beam.SkeletonAxis[i].AxisCurve;
                    buckling.BucklingY = axis.GetLength();
                    buckling.BucklingZ = axis.GetLength();
                    bucklings.Add(buckling);
                }
            }

            return bucklings;
        }


        // <summary>
        // bottom beam nodes and axis
        // </summary>
        public Beam GenerateBottomBeamDivisions(Curve skeleton, List<Point3d> topNodes, Plane _plane,
            double _maxHeight)
        {
            var points = new List<Point3d>();
            var nodes = new List<Node>();
            var bottomBeamAxisTrimmed = new List<BeamAxis>();
            var intersectingLines = new List<Line>();
            var tempPoints = new List<Point3d>(topNodes);

            for (int i = 0;
                i < tempPoints.Count;
                i++)
            {
                var tempPt = _plane.Origin - Vector3d.ZAxis * _maxHeight;
                var lineA = new Line(tempPoints[i], new Point3d(tempPoints[i].X, tempPoints[i].Y, tempPt.Z));
                intersectingLines.Add(lineA);
            }

            var parameters = new List<double>();
            var point = new Point3d();
            foreach (var line in intersectingLines)
            {
                var intersectionEvents = Intersection.CurveCurve(skeleton, line.ToNurbsCurve(), 0.01, 0.0);
                if (intersectionEvents == null) continue;
                for (int i = 0;
                    i < intersectionEvents.Count;
                    i++)
                {
                    var intEv = intersectionEvents[0];
                    point = intEv.PointA;
                    points.Add(point);
                    nodes.Add(new Node(point));
                    parameters.Add(intEv.ParameterA);
                }
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                if (i <= 0) continue;
                var axisCurve = skeleton.Trim(parameters[i - 1], parameters[i]);
                var beamAxis = new BeamAxis(axisCurve);
                bottomBeamAxisTrimmed.Add(beamAxis);
            }

            var beamSkeleton = new List<BeamAxis> {new BeamAxis(skeleton)};

            return new Beam()
            {
                Nodes = nodes,
                SkeletonAxis = beamSkeleton,
                Axis = bottomBeamAxisTrimmed
            };
        }

        // <summary>
        // generates the top beam nodes and axis
        // </summary>
        public Beam GenerateTopBeamDivisions(Curve skeleton, double[] divisionParams)
        {
            var points = new List<Point3d>();
            var nodes = new List<Node>();
            var topBeamAxisTrimmed = new List<BeamAxis>();
            var tempParams = new List<double>(divisionParams.ToList());
            for (var i = 0; i < tempParams.Count; i++)
            {
                var point = skeleton.PointAt(tempParams[i]);
                nodes.Add(new Node(point));
                points.Add(point);
                if (i <= 0) continue;
                var axisCurve = skeleton.Trim(tempParams[i - 1], tempParams[i]);
                var beamAxis = new BeamAxis(axisCurve);
                topBeamAxisTrimmed.Add(beamAxis);
            }

            var beamSkeleton = new List<BeamAxis> {new BeamAxis(skeleton)};
            return new Beam()
            {
                Nodes = nodes,
                SkeletonAxis = beamSkeleton,
                Axis = topBeamAxisTrimmed
            };
        }

        // construct intermediate beam axis
        public Beam GenerateIntermediateBeamAxis(TrussInputs _inputs, List<Point3d> _topPoints,
            List<Point3d> _bottomPoints, string _articulationType)
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (_inputs.TrussType == ConnectionType.Warren)
            {
                connections = new WarrenConnection(_topPoints, _bottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            else if (_inputs.TrussType == ConnectionType.WarrenStuds)
            {
                connections = new WarrenStudsConnection(_topPoints, _bottomPoints, _articulationType.ToString());
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Pratt)
            {
                connections = new PrattConnection(_topPoints, _bottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Howe)
            {
                connections = new HoweConnection(_topPoints, _bottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            var intermediateBeamAxis = new List<BeamAxis>();
            bars.ForEach(axis => intermediateBeamAxis.Add(new BeamAxis(axis)));

            return new Beam()
            {
                SkeletonAxis = intermediateBeamAxis,
                Axis = intermediateBeamAxis
            };
        }
    }
}