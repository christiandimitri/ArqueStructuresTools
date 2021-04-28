﻿using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace WarehouseLib
{
    public abstract class Truss
    {
        public List<Line> Beams;
        public List<Curve> BottomBars;
        public List<Point3d> BottomNodes;
        public double ClearHeight;
        public List<Column> Columns;
        public int Divisions;
        public double Height;
        public List<Curve> IntermediateBars;
        public double Length;
        public double MaxHeight;
        public Plane Plane;
        public List<Point3d> StartingNodes;
        public List<Curve> TopBars;
        public List<Point3d> TopNodes;
        public string TrussType;
        public string ArticulationType;

        protected Truss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions,
            string trussType, string articulationType)
        {
            Plane = plane;
            Length = length;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            Divisions = divisions;
            TrussType = trussType;
            ArticulationType = articulationType;
        }

        protected int RecomputeDivisions(int divisions)
        {
            var recomputedDivisions = divisions;
            if ((TrussType == "Howe" || TrussType == "Pratt") && divisions % 2 == 1) recomputedDivisions++;

            return recomputedDivisions;
        }

        protected List<Point3d> GetStartingPoints(Plane plane, double leftLength, double rightLength, double leftHeight,
            double centerHeight,
            double rightHeight)
        {
            var ptA = plane.PointAt(-leftLength, 0, leftHeight);
            var ptB = plane.PointAt(0, 0, centerHeight);
            var ptC = plane.PointAt(rightLength, 0, rightHeight);
            var startingPoints = new List<Point3d> {ptA, ptB, ptC};
            return startingPoints;
        }

        public abstract void ChangeArticulationAtColumnsByType(string type);
        public abstract void ChangeBaseByType(int type);
        public abstract void IsRigidToColumns();
        public abstract void IsArticulatedToColumns();

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
            var difference = Height - ClearHeight;
            return difference;
        }

        protected void GenerateColumns()
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            var axisA = new Line(new Point3d(StartingNodes[0].X, StartingNodes[0].Y, Plane.Origin.Z),
                StartingNodes[0]);
            var axisB = new Line(new Point3d(StartingNodes[2].X, StartingNodes[2].Y, Plane.Origin.Z),
                StartingNodes[2]);
            columns.Add(new Column(axisA));
            columns.Add(new Column(axisB));

            Columns = columns;
        }

        public abstract void GenerateTopBars();
        public abstract void GenerateTickBottomBars();

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

        public void GenerateThickBottomNodes(Curve crv)
        {
            List<Point3d> nodes = new List<Point3d>();
            List<double> parameters = new List<double>();
            var difference = ComputeDifference();
            var points = new List<Point3d>(TopNodes);
            var intersectingLines = new List<Line>();
            for (int i = 0; i < points.Count; i++)
            {
                var tempPt = Plane.Origin - Vector3d.ZAxis * MaxHeight;
                var lineA = new Line(points[i], new Point3d(points[i].X, points[i].Y, tempPt.Z));
                intersectingLines.Add(lineA);
            }

            foreach (var line in intersectingLines)
            {
                var intersectionEvents = Intersection.CurveCurve(crv, line.ToNurbsCurve(), 0.01, 0.0);
                if (intersectionEvents != null)
                {
                    for (int i = 0; i < intersectionEvents.Count; i++)
                    {
                        var intEv = intersectionEvents[0];
                        nodes.Add(intEv.PointA);
                    }
                }
            }

            BottomNodes.AddRange(nodes);
        }

        public abstract void ConstructTruss(int divisions);

        protected void GenerateIntermediateBars(string trussType, int index)
        {
            if (trussType == "Warren")
                ConstructWarrenTruss();
            else if (trussType == "Warren_Studs")
                ConstructWarrenStudsTruss();
            else if (trussType == "Pratt")
                ConstructPrattTruss(index);
            else if (trussType == "Howe") ConstructHoweTruss(index);
            RecomputeNodes();
        }

        private void ConstructWarrenTruss()
        {
            var bars = new List<Curve>();
            for (var i = 0; i < TopNodes.Count; i += 2)
            {
                if (i < TopNodes.Count - 1)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i + 1]);
                    bars.Add(lineA.ToNurbsCurve());
                }

                if (i > 0)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i - 1]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            IntermediateBars = bars;
        }

        private void ConstructPrattTruss(int index)
        {
            var bars = new List<Curve>();
            for (var i = 0; i < TopNodes.Count; i += 2)
            {
                if (i < index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i + 2]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i - 2]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            bars.RemoveAt(0);
            bars.RemoveAt(bars.Count - 1);
            IntermediateBars = bars;
        }

        private void ConstructHoweTruss(int index)
        {
            var tempTopNodes = BottomNodes;
            var tempBottomNodes = TopNodes;
            var bars = new List<Curve>();
            for (var i = 0; i < tempTopNodes.Count; i += 2)
            {
                if (i < index)
                {
                    var lineA = new Line(tempBottomNodes[i], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i + 2], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i == index)
                {
                    var lineA = new Line(TopNodes[i], BottomNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    var lineA = new Line(tempBottomNodes[i - 2], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                    lineA = new Line(tempBottomNodes[i], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            bars.RemoveAt(0);
            bars.RemoveAt(bars.Count - 1);
            if (ArticulationType == "Articulated")
            {
                bars.RemoveAt(0);
                var lineA = new Line(TopNodes[0], BottomNodes[2]);
                bars.Insert(0, lineA.ToNurbsCurve());
                bars.RemoveAt(bars.Count - 1);
                lineA = new Line(TopNodes[TopNodes.Count - 1], BottomNodes[BottomNodes.Count - 3]);
                bars.Add(lineA.ToNurbsCurve());
            }

            IntermediateBars = bars;
        }

        private void ConstructWarrenStudsTruss()
        {
            throw new NotImplementedException();
        }

        private void RecomputeNodes()
        {
            List<Point3d> tempTopList = new List<Point3d>();
            List<Point3d> tempBottomList = new List<Point3d>();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                if (TrussType == "Warren")
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
                else if (TrussType == "Warren_Studs")
                {
                    tempTopList.Add(TopNodes[i]);
                    if (i % 2 == 1 || i == TopNodes.Count - 1 || i == 0)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (TrussType == "Howe" || TrussType == "Pratt")
                {
                    if (i % 2 == 0)
                    {
                        tempTopList.Add(TopNodes[i]);
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
            }

            if (TrussType == "Warren")
            {
                tempBottomList.Insert(0, BottomNodes[0]);
                tempBottomList.Add(BottomNodes[BottomNodes.Count - 1]);
            }

            TopNodes = new List<Point3d>(tempTopList);
            BottomNodes = new List<Point3d>(tempBottomList);
        }
    }
}