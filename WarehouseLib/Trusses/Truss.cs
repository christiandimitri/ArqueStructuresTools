using System;
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
        public ConnectionType _connectionType;
        public string _articulationType;
        public double _facadeStrapsDistance;
        public PorticoType _porticoType;
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

        private List<BeamAxis> AllAxisList()
        {
            var allAxisList = new List<BeamAxis>();

            allAxisList.AddRange(TopBeamAxis);
            allAxisList.AddRange(BottomBeamAxis);

            return allAxisList;
        }

        public void UpdatePorticoType(Truss truss)
        {
            truss._porticoType =
                BottomBeam.Axis == null ? PorticoType.Portico : PorticoType.Truss;
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
            // SetNodesToBeams(TopBeam, BottomBeam, IntermediateBeams);
            // GetAxisBetweenNodes();
            // SetBeamsAxisHalfEdges();
        }

        // public void SetNodesToBeams(Beam top, Beam bottom, Beam intermediate)
        // {
        //     top.Nodes = new List<Node>();
        //     foreach (var t in TopNodes)
        //     {
        //         top.Nodes.Add(new Node(t));
        //     }
        //
        //     IntermediateBeams.Nodes = new List<Node>();
        //
        //     bottom.Nodes = new List<Node>();
        //     foreach (var t in BottomNodes)
        //     {
        //         bottom.Nodes.Add(new Node(t));
        //     }
        // }

        private int RecomputeDivisions(int divisions)
        {
            if (divisions <= 1) throw new Exception("truss division has to be >=2");
            var recomputedDivisions = divisions;
            if (_connectionType != ConnectionType.Warren)
            {
                recomputedDivisions /= (int) 2;
            }
            // else
            //     recomputedDivisions *= 2;

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

        public struct TempBeam
        {
            public List<Point3d> nodes;
            public List<BeamAxis> axis;
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

        protected TempBeam GenerateTopBeamDivisions(Curve curve, double[] parameters, int index)
        {
            var nodes = new List<Point3d>();
            var topBeamAxisTrimmed = new List<BeamAxis>();
            for (var i = 0; i < parameters.Length; i++)
            {
                nodes.Add(curve.PointAt(parameters[i]));
                if (i <= 0) continue;
                var axisCurve = curve.Trim(parameters[i - 1], parameters[i]);
                var beamAxis = new BeamAxis(axisCurve);
                topBeamAxisTrimmed.Add(beamAxis);
            }

            return new TempBeam()
            {
                nodes = nodes,
                axis = topBeamAxisTrimmed
            };
        }

        protected TempBeam GenerateBottomBeamDivisions(Curve curve, List<Point3d> points, int index)
        {
            var nodes = new List<Point3d>();
            var bottomBeamAxisTrimmed = new List<BeamAxis>();
            var intersectingLines = new List<Line>();
            var tempPoints = new List<Point3d>(points);
            if (index == 0) tempPoints.Add(curve.PointAtEnd);
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
                var intersectionEvents = Intersection.CurveCurve(curve, line.ToNurbsCurve(), 0.01, 0.0);
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
                var axisCurve = curve.Trim(parameters[i - 1], parameters[i]);
                var beamAxis = new BeamAxis(axisCurve);
                bottomBeamAxisTrimmed.Add(beamAxis);
            }

            return new TempBeam()
            {
                nodes = nodes,
                axis = bottomBeamAxisTrimmed
            };
        }

        public virtual void ConstructTruss(int divisions)
        {
            divisions = _divisions;
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            TopBeamAxis = new List<BeamAxis>();
            BottomBeamAxis = new List<BeamAxis>();

            for (var i = 0; i < TopBeamBaseCurves.Count; i++)
            {
                var topBeamBaseCurve = TopBeamBaseCurves[i];
                var bottomBeambaseCurve = BottomBeamBaseCurves[i];
                var divideByCount = topBeamBaseCurve.DivideByCount(divisions, true);


                var recomputedTopParams = RecomputeParametersByConnectionType(divideByCount);
                var recomputedBottomParams = ReturnNotContainedParams(recomputedTopParams, divideByCount.ToList());


                var resultTop =
                    GenerateTopBeamDivisions(topBeamBaseCurve, divideByCount.ToArray(), i);

                foreach (var node in resultTop.nodes)
                {
                    if (!TopNodes.Contains(node))
                    {
                        TopNodes.Add(node);
                    }
                }

                foreach (var axis in resultTop.axis)
                {
                    if (!TopBeamAxis.Contains(axis))
                    {
                        TopBeamAxis.Add(axis);
                    }
                }


                var tempBottomNodes = GenerateTopBeamDivisions(topBeamBaseCurve, recomputedBottomParams.ToArray(), i).nodes;
                var resultBottom = GenerateBottomBeamDivisions(bottomBeambaseCurve, tempBottomNodes, i);


                foreach (var node in resultBottom.nodes)
                {
                    if (!BottomNodes.Contains(node))
                    {
                        BottomNodes.Add(node);
                    }
                }

                foreach (var axis in resultBottom.axis)
                {
                    if (!BottomBeamAxis.Contains(axis))
                    {
                        BottomBeamAxis.Add(axis);
                    }
                }
            }

            var cloud = new PointCloud(TopNodes);
            var index = cloud.ClosestPoint(StartingNodes[1]);
            GenerateIntermediateBars(_connectionType, index);
        }

        private List<double> ReturnNotContainedParams(List<double> topParams, List<double> divideByCount)
        {
            var outParams = new List<double>();
            if (_connectionType == ConnectionType.Warren)
            {
                for (int i = 0; i < divideByCount.Count; i++)
                {
                    if (!topParams.Contains(divideByCount[i]))
                    {
                        outParams.Add(divideByCount[i]);
                    }
                }
            }
            else outParams = divideByCount;

            return outParams;
        }

        private List<double> RecomputeParametersByConnectionType(double[] parameters)
        {
            var outParams = new List<double>();
            if (_connectionType == ConnectionType.Warren)
            {
                for (int i = 0; i < parameters.ToList().Count; i += 2)
                {
                    outParams.Add(parameters[i]);
                }
            }

            else outParams = parameters.ToList();

            return outParams;
        }

        protected void GenerateIntermediateBars(ConnectionType trussType, int index)
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (trussType == ConnectionType.Warren)
            {
                connections = new WarrenConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }

            else if (trussType == ConnectionType.WarrenStuds)
            {
                connections = new WarrenStudsConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Pratt)
            {
                connections = new PrattConnection(TopNodes, BottomNodes);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Howe)
            {
                connections = new HoweConnection(TopNodes, BottomNodes, _articulationType);
                connections.MidPointIndex = index;
                bars = connections.ConstructConnections();
            }

            IntermediateBeamsBaseCurves = bars;
            // RecomputeNodes(index);
        }

        public abstract List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index);

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
        // sets the half-edge on each beam axis
        //</summary>
        private void SetBeamsAxisHalfEdges()
        {
            var beamAxisHalfEdgeList = new List<BeamAxisHalfEdge>();
            var ptNodes = new Dictionary<Point3d, Node>();
            var beamAxisEnumerable = AllAxisList();

            TopNodes.ForEach(pt => ptNodes.Add(pt, new Node(pt)));
            BottomNodes.ForEach(pt => ptNodes.Add(pt, new Node(pt)));

            foreach (var tempAxisA in beamAxisEnumerable)
            {
                var beamAxisHalfEdgeA = new BeamAxisHalfEdge();
                var beamAxisHalfEdgeB = new BeamAxisHalfEdge();

                beamAxisHalfEdgeA.Axis = tempAxisA;
                beamAxisHalfEdgeA.Origin = ptNodes[tempAxisA.AxisCurve.PointAtStart];
                ptNodes[tempAxisA.AxisCurve.PointAtStart].AdjacentAxisHalfEdges.Add(beamAxisHalfEdgeA);

                beamAxisHalfEdgeB.Axis = tempAxisA;
                beamAxisHalfEdgeB.Origin = ptNodes[tempAxisA.AxisCurve.PointAtEnd];
                ptNodes[tempAxisA.AxisCurve.PointAtEnd].AdjacentAxisHalfEdges.Add(beamAxisHalfEdgeB);


                beamAxisHalfEdgeB.Twin = beamAxisHalfEdgeA;
                beamAxisHalfEdgeA.Twin = beamAxisHalfEdgeB;

                beamAxisHalfEdgeList.Add(beamAxisHalfEdgeA);
                beamAxisHalfEdgeList.Add(beamAxisHalfEdgeB);
            }
        }
    }
}