using System;
using System.Collections.Generic;
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
    public abstract class Truss
    {
        public List<Point3d> BottomNodes;
        public readonly double _clearHeight;
        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;
        public int _divisions;
        public int _columnsCount;
        public double _height;
        public double _length;
        public double _maxHeight;
        public Plane _plane;
        public List<Point3d> StartingNodes;
        public List<Curve> TopBeamBaseCurves;
        public List<BeamAxis> TopBeamAxis;
        public Beam TopBeam;
        public List<Curve> BottomBeamBaseCurves;
        public Beam BottomBeam;
        public List<BeamAxis> BottomBeamAxis;
        public List<Curve> IntermediateBeamsBaseCurves;
        public Beam IntermediateBeams;
        public List<BeamAxis> IntermediateBeamAxis;
        public List<Point3d> TopNodes;
        public List<Point3d> BoundaryTopNodes;
        public string _connectionType;
        public string _articulationType;
        public double _facadeStrapsDistance;
        public string _porticoType;
        public List<Point3d> StAndresBottomNodes;
        public List<int> StAndresBottomNodesIndices;

        protected Truss(Plane plane, TrussInputs inputs)
        {
            _plane = plane;
            _length = inputs.Width;
            _height = inputs.Height;
            _maxHeight = inputs.MaxHeight;
            _clearHeight = inputs.ClearHeight;
            _divisions = inputs.Divisions;
            _connectionType = inputs.TrussType;
            _articulationType = inputs._articulationType;
            _columnsCount = inputs.ColumnsCount;
            _divisions = RecomputeDivisions(_divisions);
            _facadeStrapsDistance = inputs.FacadeStrapsDistance;
            _porticoType = inputs.PorticoType;
        }

        public void UpdatePorticoType(Truss truss)
        {
            truss._porticoType =
                BottomBeam.Axis == null ? PorticoType.Portico.ToString() : PorticoType.Truss.ToString();
        }

        protected void ConstructBeams(bool joinTopBeamsAxis, bool joinBottomBeamsAxis)
        {
            var tempTopBeamAxis = (joinTopBeamsAxis == true)
                ? Curve.JoinCurves(TopBeamBaseCurves, 0.1).ToList()
                : TopBeamBaseCurves;

            var tempAxis = new List<BeamAxis>();
            for (int i = 0; i < tempTopBeamAxis.Count; i++)
            {
                var axis = new BeamAxis(tempTopBeamAxis[i]);
                tempAxis.Add(axis);
            }

            var topBeam = new Beam
            {
                Axis = tempAxis,
                ProfileOrientationPlane = Plane.WorldXY,
                Position = "Top",
            };


            TopBeam = topBeam;

            var tempBottomBeamAxis = (joinBottomBeamsAxis == true)
                ? Curve.JoinCurves(BottomBeamBaseCurves, 0.1).ToList()
                : BottomBeamBaseCurves;
            tempAxis = new List<BeamAxis>();
            for (int i = 0; i < tempBottomBeamAxis.Count; i++)
            {
                var axis = new BeamAxis(tempBottomBeamAxis[i]);
                tempAxis.Add(axis);
            }

            var bottomBeam = new Beam
            {
                Axis = tempAxis,
                ProfileOrientationPlane = Plane.WorldXY,
                Position = "Bottom"
            };

            BottomBeam = bottomBeam;

            tempAxis = new List<BeamAxis>();

            for (int i = 0; i < IntermediateBeamsBaseCurves.Count; i++)
            {
                var axis = new BeamAxis(IntermediateBeamsBaseCurves[i]);
                tempAxis.Add(axis);
            }

            var interBeams = new Beam
            {
                Axis = tempAxis,
                ProfileOrientationPlane = Plane.WorldYZ,
                Position = "Intermediate"
            };

            IntermediateBeams = interBeams;
            SetNodesToBeams(TopBeam, BottomBeam, IntermediateBeams);
        }

        public void SetNodesToBeams(Beam top, Beam bottom, Beam intermediate)
        {
            top.Nodes = new List<Node>();
            foreach (var t in TopNodes)
            {
                top.Nodes.Add(new Node(t));
            }

            IntermediateBeams.Nodes = new List<Node>();

            bottom.Nodes = new List<Node>();
            foreach (var t in BottomNodes)
            {
                bottom.Nodes.Add(new Node(t));
            }
        }

        private int RecomputeDivisions(int divisions)
        {
            if (divisions <= 1) throw new Exception("truss division has to be >=2");
            var recomputedDivisions = divisions;
            if (_connectionType != ConnectionType.Warren.ToString())
            {
                recomputedDivisions /= 2;
            }

            return recomputedDivisions;
        }

        protected static List<Point3d> GetStartingPoints(Plane plane, double leftLength, double rightLength,
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

        protected void ChangeBaseByType(int index)
        {
            if (index == 0)
            {
                BaseIsThickened();
            }
            else
            {
                BaseIsStraight();
            }
        }

        private void BaseIsThickened()
        {
            GenerateThickBottomBars();
        }

        private void BaseIsStraight()
        {
            GenerateStraightBottomBars();
        }

        protected void GenerateTopNodes(Curve curve, int divisions, int index)
        {
            var nodes = new List<Point3d>();
            var parameters =
                curve.DivideByCount(divisions, true);

            for (var i = 0; i < parameters.Length; i++) nodes.Add(curve.PointAt(parameters[i]));

            if (index == 0) nodes.RemoveAt(nodes.Count - 1);

            TopNodes.AddRange(nodes);
        }

        protected double ComputeDifference()
        {
            var difference = _height - _clearHeight;
            return difference;
        }

        public abstract void GenerateTopBars();
        protected abstract void GenerateThickBottomBars();

        protected abstract List<Curve> ComputeBottomBarsArticulatedToColumns(List<Curve> bars);

        public void GenerateStraightBottomBars()
        {
            Point3d ptA = StartingNodes[0] - Vector3d.ZAxis * ComputeDifference();
            Point3d ptC = new Point3d(StartingNodes[2].X, StartingNodes[2].Y, ptA.Z);
            Point3d ptB = new Point3d(StartingNodes[1].X, StartingNodes[1].Y, ptA.Z);
            Line lineA = new Line(ptA, ptB);
            Line lineB = new Line(ptB, ptC);
            var bars = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            BottomBeamBaseCurves = bars;
        }

        protected abstract void GenerateBottomNodes(Curve crv);

        protected void GenerateVerticalBottomNodes(Curve crv)
        {
            List<Point3d> nodes = new List<Point3d>();
            var points = new List<Point3d>(TopNodes);

            var intersectingLines = new List<Line>();
            for (int i = 0;
                i < points.Count;
                i++)
            {
                var tempPt = _plane.Origin - Vector3d.ZAxis * _maxHeight;
                var lineA = new Line(points[i], new Point3d(points[i].X, points[i].Y, tempPt.Z));
                intersectingLines.Add(lineA);
            }

            foreach (var line in intersectingLines)
            {
                var intersectionEvents = Intersection.CurveCurve(crv, line.ToNurbsCurve(), 0.01, 0.0);
                if (intersectionEvents == null) continue;
                for (int i = 0;
                    i < intersectionEvents.Count;
                    i++)
                {
                    var intEv = intersectionEvents[0];
                    nodes.Add(intEv.PointA);
                }
            }

            BottomNodes.AddRange(nodes);
        }

        public abstract void ConstructTruss(int divisions);

        protected void GenerateIntermediateBars(string trussType, int index)
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (trussType == ConnectionType.Warren.ToString())
            {
                connections = new WarrenConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }

            else if (trussType == ConnectionType.WarrenStuds.ToString())
            {
                connections = new WarrenStudsConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Pratt.ToString())
            {
                connections = new PrattConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Howe.ToString())
            {
                connections = new HoweConnection(TopNodes, BottomNodes, _articulationType);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }

            IntermediateBeamsBaseCurves = bars;
            RecomputeNodes(index);
        }

        public abstract List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index);

        protected abstract void RecomputeNodes(int index);

        protected void ChangeArticulationAtColumnsByType(string type)
        {
            if (type == ArticulationType.Articulated.ToString())
            {
                IsArticulatedToColumns();
            }
            else if (type == ArticulationType.Rigid.ToString())
            {
                IsRigidToColumns();
            }
        }

        private void IsRigidToColumns()
        {
            BottomBeamBaseCurves = new List<Curve>(BottomBeamBaseCurves);
        }

        protected abstract void IsArticulatedToColumns();

        private void GenerateBoundaryColumnsNodes(List<Curve> topBars, int divisions)
        {
            BoundaryTopNodes = new List<Point3d>();
            var nodes = new List<Point3d>();

            var joinedBars = Curve.JoinCurves(topBars)[0];
            var ptA = joinedBars.PointAtStart;
            var ptB = joinedBars.PointAtEnd;
            var columnsBase = new LineCurve(ptA, ptB);
            var parameters =
                columnsBase.DivideByCount(divisions - 1, true);

            foreach (var t1 in parameters)
            {
                var node = columnsBase.PointAt(t1);
                var tempPlane = new Plane(node, _plane.XAxis);
                var intersectionEvents = Intersection.CurvePlane(joinedBars, tempPlane, 0.001);
                if (intersectionEvents == null) continue;
                foreach (var t in intersectionEvents)
                {
                    var intEv = intersectionEvents[0];
                    nodes.Add(intEv.PointA);
                }
            }

            BoundaryTopNodes.AddRange(nodes);
        }

        public void ConstructPorticoFromTruss(Truss truss, int index)
        {
            TopBeamBaseCurves = new List<Curve>(TopBeamBaseCurves);
            TopNodes = new List<Point3d>(TopNodes);
            BottomBeam = new Beam();
            IntermediateBeams = new Beam();
            BottomBeamBaseCurves = new List<Curve>();
            BottomNodes = new List<Point3d>();
            IntermediateBeamsBaseCurves = new List<Curve>();
            truss.GenerateBoundaryColumnsNodes(truss.TopBeamBaseCurves, _columnsCount);
            BoundaryColumns =
                new List<Column>(new BoundaryColumn().GenerateColumns(truss.BoundaryTopNodes, _plane, index));
        }

        // <summary>
        // split beam axis curves private between nodes
        // </summary>
        private List<Curve> SplitBeamBetweenNodes(List<Point3d> nodes, Beam beam)
        {
            var axis = new List<Curve>();
            var tempBaseAxisList = new List<Curve>();

            for (int i = 0; i < beam.Axis.Count; i++)
            {
                var tempAxis = beam.Axis[i].AxisCurve;
                tempBaseAxisList.Add(tempAxis);
            }

            tempBaseAxisList = Curve.JoinCurves(tempBaseAxisList).ToList();

            var baseAxis = tempBaseAxisList[0];
            for (var i = 1; i < nodes.Count - 1; i++)
            {
                var node = nodes[i];
                double t;
                baseAxis.ClosestPoint(node, out t);
                var tempList = baseAxis.ToNurbsCurve().Split(t).ToList();
                axis.Add(tempList[0]);
                baseAxis = tempList[1];
                if (i == nodes.Count - 1)
                {
                    axis.Add(baseAxis);
                }
            }

            axis.Add(baseAxis);


            return axis;
        }

        // <summary>
        // gets the splitted axis in there corresponding list
        // </summary>

        public void GetAxisBetweenAxis()
        {
            var tempList = new List<Curve>(SplitBeamBetweenNodes(TopNodes, TopBeam));
            TopBeamAxis = new List<BeamAxis>();
            for (int i = 0; i < tempList.Count; i++)
            {
                var axis = new BeamAxis(tempList[i]);
                TopBeamAxis.Add(axis);
            }

            tempList = new List<Curve>(SplitBeamBetweenNodes(BottomNodes, BottomBeam));
            BottomBeamAxis = new List<BeamAxis>();
            for (int i = 0; i < tempList.Count; i++)
            {
                var axis = new BeamAxis(tempList[i]);
                BottomBeamAxis.Add(axis);
            }

            IntermediateBeamAxis = new List<BeamAxis>();
            for (int i = 0; i < IntermediateBeamsBaseCurves.Count; i++)
            {
                var axis = new BeamAxis(IntermediateBeamsBaseCurves[i]);
                IntermediateBeamAxis.Add(axis);
            }
        }

        // <summary>
        // sets the half-edge on each beam axis
        //</summary>
        private void SetBeamsAxisHalfEdge()
        {
            //<summary>
            // sets the half-edge
            //</summary>

            //<summary>
            // sets the Twin half-edge
            //</summary>

            // <summary>
            // sets the previous half-edge
            //</summary>

            //<summary>
            // sets the next half-edge
            //</summary>
        }
    }
}