﻿using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WarehouseLib
{
    public abstract class Truss
    {
        public Plane Plane;
        public double Length;
        public double Height;
        public double MaxHeight;
        public double ClearHeight;
        public int Divisions;
        public string TrussType;
        public List<Point3d> StartingNodes;
        public List<Point3d> TopNodes;
        public List<Point3d> BottomNodes;
        public List<Curve> TopBars;
        public List<Curve> BottomBars;
        public List<Curve> IntermediateBars;
        public List<Line> Beams;
        public List<Column> Columns;

        public Truss(Plane plane, double length, double height, double maxHeight,double clearHeight, int divisions, string trussType)
        {
            Plane = plane;
            Length = length;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            Divisions = divisions;
            TrussType = trussType;
        }

        public void RecomputeDivisions()
        {
            if(TrussType== "Warren" && Divisions%2 ==1)
            {
                Divisions++;
            }
        }
        public List<Point3d> GetStartingPoints(Plane plane, double length, double leftHeight, double centerHeight, double rightHeight)
        {
            Point3d ptA = plane.PointAt(0, 0, leftHeight);
            Point3d ptB = plane.PointAt(length / 2, 0, centerHeight);
            Point3d ptC = plane.PointAt(length, 0, rightHeight);
            List<Point3d> startingPoints = new List<Point3d> { ptA, ptB, ptC };
            return startingPoints;
        }
        public List<Point3d> GenerateTopNodes(Curve curve, int divisions)
        {
            List<Point3d> nodes = new List<Point3d>();
            double[]parameters = 
            curve.DivideByCount(divisions, true);

            for (int i = 0; i < parameters.Length; i++)
            {
                nodes.Add(curve.PointAt(parameters[i]));
            }
            return nodes;
        }
        public double ComputeDifference()
        {
            double difference = Height - ClearHeight;
            return difference;
        }
        public void GenerateColumns()
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            Line axisA = new Line(new Point3d(StartingNodes[0].X, StartingNodes[0].Y, Plane.Origin.Z), StartingNodes[0]);
            Line axisB = new Line(new Point3d(StartingNodes[2].X, StartingNodes[2].Y, Plane.Origin.Z), StartingNodes[2]);
            columns.Add(new Column(axisA));
            columns.Add(new Column(axisB));
            
            Columns = columns;
        }
        public abstract void GenerateTopBars();
        public abstract void GenerateBottomNodes(List<Point3d> points, double difference);
        public abstract void ConstructTruss(int divisions);
        public abstract void GenerateBottomBars();
        public void GenerateIntermediateBars(string trussType, Point3d pt)
        {

            if(trussType == "Warren")
            {
                ConstructWarrenTruss(pt);
            }
            else if (trussType == "Warren_Studs")
            {
                ConstructWarrenStudsTruss(pt);
            }
            else if (trussType == "Pratt")
            {
                ConstructPrattTruss(pt);
            }
            else if (trussType == "Howe")
            {
                ConstructHoweTruss(pt);
            }
        }
        private void ConstructWarrenTruss(Point3d pt)
        {
            List<Curve> bars = new List<Curve>();
            int index = TopNodes.IndexOf(pt);
            for (int i = 0; i < TopNodes.Count; i += 2)
            {
                Line lineA = new Line();
                Line lineB = new Line();
                
                if (i < index)
                {
                    lineA = new Line(TopNodes[i], BottomNodes[i + 1]);
                }
                else if (i > 0 && i<index)
                {
                    lineB = new Line(TopNodes[i], BottomNodes[i - 1]);
                }
                if(lineA.IsValid || lineB.IsValid)
                {
                    bars.Add(lineA.ToNurbsCurve());
                    bars.Add(lineB.ToNurbsCurve());

                }
            }
            IntermediateBars = bars;
        }
        private void ConstructHoweTruss(Point3d pt)
        {
            throw new NotImplementedException();
        }

        private void ConstructPrattTruss(Point3d pt)
        {
            throw new NotImplementedException();
        }

        private void ConstructWarrenStudsTruss(Point3d pt)
        {
            throw new NotImplementedException();
        }
        public abstract void GenerateBeams();

    }
}