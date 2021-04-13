using Grasshopper.Kernel.Data;
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

        public List<Point3d> StartingNodes;
        public List<Point3d> TopNodes;
        public List<Point3d> BottomNodes;
        public List<Curve> UpperBars;
        public List<Curve> LowerBars;
        public List<Line> Beams;

        public Truss(Plane plane, double length, double height, double maxHeight,double clearHeight, int divisions)
        {
            Plane = plane;
            Length = length;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            Divisions = divisions;
        }
        public List<Point3d> GetStartingPoints(Plane plane, double length, double leftHeight, double centerHeight, double rightHeight)
        {
            Point3d ptA = plane.PointAt(0, 0, leftHeight);
            Point3d ptB = plane.PointAt(length / 2, 0, centerHeight);
            Point3d ptC = plane.PointAt(length, 0, rightHeight);
            List<Point3d> startingPoints = new List<Point3d> { ptA, ptB, ptC };
            return startingPoints;
        }
        public List<Point3d> GetNodesOnCurve(Curve curve, int divisions)
        {
            List<Point3d> nodes = new List<Point3d>();
            double[]parameters=curve.DivideByCount(divisions, true);

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

        public abstract void GenerateUpperBars();
        public abstract void GenerateLowerNodes(List<Point3d> points, double difference);
        public abstract void GenerateLowerBars();
        public abstract void GenerateBeams();



    }
}