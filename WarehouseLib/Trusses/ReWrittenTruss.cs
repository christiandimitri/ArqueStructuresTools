using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
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

        public Curve TopBeamSkeleton;
        public Curve BottomBeamSkeleton;
        public List<Curve> IntermediateBeamSkeleton;

        // store the beams axis

        public List<BeamAxis> TopBeamAxis;
        public List<BeamAxis> BottomBeamAxis;
        public List<BeamAxis> IntermediateBeamAxis;

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

        protected class TempBeam
        {
            public List<Point3d> nodes;
            public List<BeamAxis> axis;
        }

        // top beam nodes and axis
        protected TempBeam GenerateTopBeamDivisions(Curve skeleton, double[] divisionParams)
        {
            var nodes = new List<Point3d>();
            var topBeamAxisTrimmed = new List<BeamAxis>();
            var tempParams = new List<double>(divisionParams.ToList());
            for (var i = 0; i < tempParams.Count; i++)
            {
                nodes.Add(skeleton.PointAt(tempParams[i]));
                if (i <= 0) continue;
                var axisCurve = skeleton.Trim(tempParams[i - 1], tempParams[i]);
                var beamAxis = new BeamAxis(axisCurve);
                topBeamAxisTrimmed.Add(beamAxis);
            }

            return new TempBeam()
            {
                nodes = nodes,
                axis = topBeamAxisTrimmed
            };
        }

        // bottom beam nodes and axis
        protected TempBeam GenerateBottomBeamDivisions(Curve skeleton, List<Point3d> points)
        {
            var nodes = new List<Point3d>();

            var bottomBeamAxisTrimmed = new List<BeamAxis>();
            var intersectingLines = new List<Line>();
            var tempPoints = new List<Point3d>(points);

            for (int i = 0;
                i < tempPoints.Count;
                i++)
            {
                var tempPt = _plane.Origin - Vector3d.ZAxis * _maxHeight;
                var lineA = new Line(tempPoints[i], new Point3d(tempPoints[i].X, tempPoints[i].Y, tempPt.Z));
                intersectingLines.Add(lineA);
            }

            var parameters = new List<double>();
            foreach (var line in intersectingLines)
            {
                var intersectionEvents = Intersection.CurveCurve(skeleton, line.ToNurbsCurve(), 0.01, 0.0);
                if (intersectionEvents == null) continue;
                for (int i = 0;
                    i < intersectionEvents.Count;
                    i++)
                {
                    var intEv = intersectionEvents[0];
                    nodes.Add(intEv.PointA);
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

            return new TempBeam()
            {
                nodes = nodes,
                axis = bottomBeamAxisTrimmed
            };
        }

        // construct intermediate beam axis
        protected void GenerateIntermediateBeamAxis()
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (_inputs.TrussType == ConnectionType.Warren)
            {
                connections = new WarrenConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }

            else if (_inputs.TrussType == ConnectionType.WarrenStuds)
            {
                connections = new WarrenStudsConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Pratt)
            {
                connections = new PrattConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Howe)
            {
                connections = new HoweConnection(TopPoints, BottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            IntermediateBeamSkeleton = bars;
        }

        // construct truss method
        protected abstract void ConstructTruss();
    }
}