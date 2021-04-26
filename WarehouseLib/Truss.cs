using System;
using System.Collections.Generic;
using Rhino.Geometry;

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

        public Truss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions,
            string trussType)
        {
            Plane = plane;
            Length = length;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            Divisions = divisions;
            TrussType = trussType;
        }

        public int RecomputeDivisions(int divisions)
        {
            var recomputedDivisons = divisions;
            if ((TrussType == "Howe" || TrussType == "Pratt") && divisions % 2 == 1) recomputedDivisons++;

            return recomputedDivisons;
        }

        public List<Point3d> GetStartingPoints(Plane plane, double length, double leftHeight, double centerHeight,
            double rightHeight)
        {
            var ptA = plane.PointAt(0, 0, leftHeight);
            var ptB = plane.PointAt(length / 2, 0, centerHeight);
            var ptC = plane.PointAt(length, 0, rightHeight);
            var startingPoints = new List<Point3d> {ptA, ptB, ptC};
            return startingPoints;
        }

        public List<Point3d> GenerateTopNodes(Curve curve, int divisions, int index)
        {
            var nodes = new List<Point3d>();
            var parameters =
                curve.DivideByCount(divisions, true);

            for (var i = 0; i < parameters.Length; i++) nodes.Add(curve.PointAt(parameters[i]));

            if (index == 0) nodes.RemoveAt(nodes.Count - 1);
            return nodes;
        }

        public double ComputeDifference()
        {
            var difference = Height - ClearHeight;
            return difference;
        }

        public void GenerateColumns()
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
        public abstract void GenerateBottomBars();
        public abstract void GenerateBottomNodes(List<Point3d> points, double difference);

        public abstract void ConstructTruss(int divisions);
        public void GenerateIntermediateBars(string trussType, int index)
        {
            if (trussType == "Warren")
                ConstructWarrenTruss();
            else if (trussType == "Warren_Studs")
                ConstructWarrenStudsTruss();
            else if (trussType == "Pratt")
                ConstructPrattTruss(index);
            else if (trussType == "Howe") ConstructHoweTruss(index);
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
                var lineA = new Line(TopNodes[i], BottomNodes[i]);
                bars.Add(lineA.ToNurbsCurve());
                if (i < index)
                {
                    lineA = new Line(TopNodes[i], BottomNodes[i + 2]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    lineA = new Line(TopNodes[i], BottomNodes[i - 2]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            IntermediateBars = bars;
        }

        private void ConstructHoweTruss(int index)
        {
            var tempTopNodes = BottomNodes;
            var tempBottomNodes = TopNodes;
            var bars = new List<Curve>();
            for (var i = 0; i < tempTopNodes.Count; i += 2)
            {
                var lineA = new Line(tempBottomNodes[i],tempTopNodes[i]);
                bars.Add(lineA.ToNurbsCurve());
                if (i < index)
                {
                    lineA = new Line(tempBottomNodes[i + 2],tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
                else if (i > index)
                {
                    lineA = new Line(tempBottomNodes[i - 2], tempTopNodes[i]);
                    bars.Add(lineA.ToNurbsCurve());
                }
            }

            IntermediateBars = bars;
        }

        private void ConstructWarrenStudsTruss()
        {
            throw new NotImplementedException();
        }

        public abstract void GenerateBeams();
    }
}