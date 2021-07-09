using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Articulations;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Nodes;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public abstract class ReWrittenTruss
    {
        // store inputs as local variables
        public TrussInputs _inputs;
        public Plane _plane;
        public double _width;
        public double _height;
        public double _maxHeight;
        public double _clearHeight;
        public double _columnsCount;
        public int _divisions;
        public ConnectionType _connectionType;
        public string _articulationType;
        public double _facadeStrapsDistance;
        public PorticoType _porticoType;

        // initialize a truss with a plane and inputs
        public ReWrittenTruss(Plane plane, TrussInputs inputs)
        {
            _inputs = inputs;
            _plane = plane;
            _width = inputs.Width;
            _height = inputs.Height;
            _maxHeight = inputs.MaxHeight;
            _clearHeight = inputs.ClearHeight;
            _divisions = inputs.Divisions;
            _connectionType = inputs.TrussType;
            _articulationType = inputs._articulationType;
            _columnsCount = inputs.ColumnsCount;
            _divisions = inputs.Divisions;
            _facadeStrapsDistance = inputs.FacadeStrapsDistance;
            _porticoType = inputs.PorticoType;
            ConstructBeamsSkeleton();
            ConstructTruss();
        }

        // store truss properties

        // store the columns Skeleton

        public List<Curve> StaticColumnsSkeleton;
        public List<Curve> BoundaryColumnsSkeleton;

        // store the columns

        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;

        // store the beams

        public Beam TopBeam;
        public Beam BottomBeam;
        public Beam IntermediateBeam;

        // store the starting points

        public List<Point3d> StartingPoints;

        // store the subdivided points
        public List<Point3d> TopPoints;
        public List<Point3d> BottomPoints;

        // store the beams skeleton curves

        public List<Curve> TopBeamSkeleton;
        public List<Curve> BottomBeamSkeleton;
        public List<Curve> IntermediateBeamSkeleton;

        // store the beams axis

        public List<BeamAxis> TopBeamAxis;
        public List<BeamAxis> BottomBeamAxis;
        public List<BeamAxis> IntermediateBeamAxis;

        // store the truss nodes
        public List<Node> TopNodes;
        public List<Node> BottomNodes;

        // initialize a truss with another one

        public ReWrittenTruss(ReWrittenTruss truss)
        {
            _inputs = truss._inputs;
            StaticColumnsSkeleton = truss.StaticColumnsSkeleton;
            BoundaryColumnsSkeleton = truss.BoundaryColumnsSkeleton;
            StaticColumns = truss.StaticColumns;
            BoundaryColumns = truss.BoundaryColumns;
            TopBeam = truss.TopBeam;
            BottomBeam = truss.BottomBeam;
            IntermediateBeam = truss.IntermediateBeam;
            StartingPoints = truss.StartingPoints;
            TopBeamSkeleton = truss.TopBeamSkeleton;
            BottomBeamSkeleton = truss.BottomBeamSkeleton;
            IntermediateBeamSkeleton = truss.IntermediateBeamSkeleton;
        }

        // construct starting points
        protected List<Point3d> ConstructStartingPoints(Plane plane, double leftLength, double rightLength,
            double leftHeight,
            double centerHeight,
            double rightHeight)
        {
            var ptA = plane.PointAt(-leftLength, 0, leftHeight);
            var ptB = plane.PointAt(0, 0, centerHeight);
            var ptC = plane.PointAt(rightLength, 0, rightHeight);
            var startingPoints = new List<Point3d> {ptA, ptB, ptC};
            return startingPoints;
        }

        // construct beams skeleton
        protected abstract void ConstructBeamsSkeleton();

        // generate top beams nodes and axis

        // top beam nodes and axis
        protected Beam GenerateTopBeamDivisions(Curve skeleton, double[] divisionParams)
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

            return new Beam()
            {
                Nodes = nodes,
                SkeletonAxis = topBeamAxisTrimmed
            };
        }

        // bottom beam nodes and axis
        protected Beam GenerateBottomBeamDivisions(Curve skeleton, List<Point3d> topNodes)
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

            return new Beam()
            {
                Nodes = nodes,
                SkeletonAxis = bottomBeamAxisTrimmed
            };
        }

        // construct intermediate beam axis
        protected virtual List<BeamAxis> GenerateIntermediateBeamAxis()
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (_inputs.TrussType == ConnectionType.Warren)
            {
                connections = new WarrenConnection(TopPoints, BottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            else if (_inputs.TrussType == ConnectionType.WarrenStuds)
            {
                connections = new WarrenStudsConnection(TopPoints, BottomPoints, _articulationType.ToString());
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Pratt)
            {
                connections = new PrattConnection(TopPoints, BottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Howe)
            {
                connections = new HoweConnection(TopPoints, BottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            IntermediateBeamSkeleton = bars;
            var intermediateBeamAxis = new List<BeamAxis>();
            IntermediateBeamSkeleton.ForEach(axis => intermediateBeamAxis.Add(new BeamAxis(axis)));

            return intermediateBeamAxis;
        }

        // recompute divisions * 2 to make divisions 
        protected virtual int RecomputeDivisonsCount()
        {
            var divisions = _divisions;
            if (_connectionType == ConnectionType.Warren)
            {
                divisions *= 2;
            }
            else if (_divisions % 2 == 1 && _connectionType != ConnectionType.Warren) divisions += 1;

            return divisions;
        }

        // construct truss method
        protected virtual void ConstructTruss()
        {
            TopPoints = new List<Point3d>();
            BottomPoints = new List<Point3d>();
            TopBeamAxis = new List<BeamAxis>();
            BottomBeamAxis = new List<BeamAxis>();
            TopNodes = new List<Node>();
            BottomNodes = new List<Node>();

            var initParams =
                TopBeamSkeleton[0].DivideByCount(RecomputeDivisonsCount(), true);

            var newParams = RecomputeParams(initParams.ToList());

            var topElements = GenerateTopBeamDivisions(TopBeamSkeleton[0], newParams.TopParams.ToArray());
            TopPoints.AddRange(topElements.GetNodesPoints());
            TopPoints.ForEach(pt => TopNodes.Add(new Node(pt)));
            Debug.WriteLine("top points");
            TopBeamAxis.AddRange(topElements.SkeletonAxis);

            var tempTopElements = GenerateTopBeamDivisions(TopBeamSkeleton[0], newParams.BottomParams.ToArray());

            var bottomElements = GenerateBottomBeamDivisions(BottomBeamSkeleton[0], tempTopElements.GetNodesPoints());
            BottomPoints.AddRange(bottomElements.GetNodesPoints());
            BottomBeamAxis.AddRange(bottomElements.SkeletonAxis);
            BottomPoints.ForEach(pt => BottomNodes.Add(new Node(pt)));

            IntermediateBeamAxis = GenerateIntermediateBeamAxis();

            TopBeam = new Beam();
            BottomBeam = new Beam();
            IntermediateBeam = new Beam();

            TopBeam.SetBeamAxisNodes(TopBeamAxis, TopNodes, new List<BeamAxis> {new BeamAxis(TopBeamSkeleton[0])});
            BottomBeam.SetBeamAxisNodes(BottomBeamAxis, BottomNodes,
                new List<BeamAxis> {new BeamAxis(BottomBeamSkeleton[0])});
            IntermediateBeam.SetBeamAxisNodes(IntermediateBeamAxis, new List<Node>(),
                new List<BeamAxis>(IntermediateBeamAxis));
        }


        // initializes a curve params divisions struct
        public struct DivisionParamsOnBeam
        {
            public List<double> TopParams;
            public List<double> BottomParams;
        }

        // recompute params by connection type
        protected virtual DivisionParamsOnBeam RecomputeParams(List<double> initParams)
        {
            var recomputedParams = new DivisionParamsOnBeam();
            recomputedParams.TopParams = new List<double>();
            recomputedParams.BottomParams = new List<double>();
            if (_connectionType == ConnectionType.Warren)
            {
                for (int i = 0; i < initParams.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        recomputedParams.TopParams.Add(initParams[i]);
                    }
                    else
                    {
                        recomputedParams.BottomParams.Add(initParams[i]);
                    }
                }

                if (_articulationType != ArticulationType.Articulated.ToString())
                {
                    recomputedParams.BottomParams.Insert(0, initParams[0]);
                    recomputedParams.BottomParams.Add(initParams[initParams.Count - 1]);
                }
            }
            else if (_connectionType == ConnectionType.WarrenStuds)
            {
                for (int i = 0; i < initParams.Count; i++)
                {
                    recomputedParams.TopParams.Add(initParams[i]);

                    if (i % 2 == 1)
                    {
                        recomputedParams.BottomParams.Add(initParams[i]);
                    }
                }

                if (_articulationType != ArticulationType.Articulated.ToString())
                {
                    recomputedParams.BottomParams.Insert(0, initParams[0]);
                    recomputedParams.BottomParams.Add(initParams[initParams.Count - 1]);
                }
            }
            else
            {
                recomputedParams.TopParams = initParams.ToList();
                recomputedParams.BottomParams = initParams.ToList();
            }

            return recomputedParams;
        }

        protected virtual List<double> RecomputedTopParams()
        {
            var topParams = new List<double>();

            return topParams;
        }
    }
}