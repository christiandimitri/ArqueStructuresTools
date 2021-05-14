﻿using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Articulations;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public abstract class Truss
    {
        public List<Curve> BottomBars;
        public List<Point3d> BottomNodes;
        private double _clearHeight;
        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;
        public int _divisions;
        public int _columnsCount;
        public double _height;
        public List<Curve> IntermediateBars;
        public double _length;
        public double _maxHeight;
        public Plane _plane;
        public List<Point3d> StartingNodes;
        public List<Curve> TopBars;
        public List<Point3d> TopNodes;
        public List<Point3d> BoundaryTopNodes;
        public string _trussType;
        public string _articulationType;

        protected Truss(Plane plane, TrussOptions options)
        {
            _plane = plane;
            _length = options.Width;
            _height = options.Height;
            _maxHeight = options.MaxHeight;
            _clearHeight = options.ClearHeight;
            _divisions = options.Divisions;
            _trussType = options.TrussType;
            _articulationType = options._articulationType;
            _columnsCount = options.ColumnsCount;
        }

        protected int RecomputeDivisions(int divisions)
        {
            if (divisions <= 1) throw new Exception("truss division has to be >=2");
            var recomputedDivisions = divisions;
            if (_trussType != "Warren")
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
            BottomBars = bars;
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
                bars = connections.ConstructConnections();
            }

            else if (trussType == ConnectionType.WarrenStuds.ToString())
            {
                connections = new WarrenStudsConnection(TopNodes, BottomNodes, index);
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Pratt.ToString())
            {
                connections = new PrattConnection(TopNodes, BottomNodes, index);
                bars = connections.ConstructConnections();
            }
            else if (trussType == ConnectionType.Howe.ToString())
            {
                connections = new HoweConnection(TopNodes, BottomNodes, index, _articulationType);
                bars = connections.ConstructConnections();
            }

            IntermediateBars = bars;
            RecomputeNodes();
        }

        public abstract List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index);

        private void RecomputeNodes()
        {
            List<Point3d> tempTopList = new List<Point3d>();
            List<Point3d> tempBottomList = new List<Point3d>();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                if (_trussType == ConnectionType.Warren.ToString())
                {
                    if (i % 2 == 0)
                    {
                        tempTopList.Add(TopNodes[i]);
                    }
                    else if (i % 2 == 1)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_trussType == ConnectionType.WarrenStuds.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    if (i % 2 == 1 || i == TopNodes.Count - 1 || i == 0)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_trussType == ConnectionType.Howe.ToString() || _trussType == ConnectionType.Pratt.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    tempBottomList.Add(BottomNodes[i]);
                }
            }

            if (_trussType == ConnectionType.Warren.ToString())
            {
                tempBottomList.Insert(0, BottomNodes[0]);
                tempBottomList.Add(BottomNodes[BottomNodes.Count - 1]);
            }

            TopNodes = new List<Point3d>(tempTopList);
            BottomNodes = new List<Point3d>(tempBottomList);
        }

        protected void ChangeArticulationAtColumnsByType(string type)
        {
            if (type == "Articulated")
            {
                IsArticulatedToColumns();
            }
            else if (type == "Rigid")
            {
                IsRigidToColumns();
            }
        }

        private void IsRigidToColumns()
        {
            BottomBars = new List<Curve>(BottomBars);
        }

        protected abstract void IsArticulatedToColumns();

        private void GenerateBoundaryColumnsNodes(List<Curve> topBars, int divisions)
        {
            BoundaryTopNodes = new List<Point3d>();
            var nodes = new List<Point3d>();
            var joinedBar = Curve.JoinCurves(topBars)[0];

            var parameters =
                joinedBar.DivideByCount(divisions - 1, true);

            for (var i = 0; i < parameters.Length; i++) nodes.Add(joinedBar.PointAt(parameters[i]));

            BoundaryTopNodes.AddRange(nodes);
        }

        public void ConstructPorticoFromTruss(Truss truss)
        {
            TopBars = new List<Curve>(TopBars);
            TopNodes = new List<Point3d>(TopNodes);
            BottomBars = null;
            BottomNodes = null;
            IntermediateBars = null;

            // if (ColumnsCount >= 1)
            // {
            truss.GenerateBoundaryColumnsNodes(truss.TopBars, _columnsCount);
            BoundaryColumns =
                new List<Column>(
                    new BoundaryColumn().GenerateColumns(truss.BoundaryTopNodes, _plane));
            // }
            // else throw new Exception("the columns count should be >=2");
        }
    }
}