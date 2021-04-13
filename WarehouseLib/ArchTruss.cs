﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public class ArchTruss : CurvedTruss
    {
        public ArchTruss(Plane plane, double length, double height, double maxHeight,double clearHeight, int divisions) : base(plane, length, height, maxHeight,clearHeight, divisions)
        {
            GenerateUpperBars();
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            foreach (var bar in UpperBars)
            {
                TopNodes.AddRange(GetNodesOnCurve(bar, divisions));
            }
        }

        public override void GenerateUpperBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length,  Height, MaxHeight, Height);

            if (Height == MaxHeight) {
                Line lineA = new Line(StartingNodes[0], StartingNodes[1]);
                Line lineB = new Line(StartingNodes[2], StartingNodes[1]);
                UpperBars = new List<Curve> { lineA.ToNurbsCurve(), lineB.ToNurbsCurve() };
            }
            else {
                Arc arch = new Arc(StartingNodes[0], StartingNodes[1], StartingNodes[2]);
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                tempCrvs[1].Reverse();
                UpperBars = new List<Curve> { tempCrvs[0], tempCrvs[1] };
            }
        }
        public override void GenerateLowerBars()
        {
            throw new NotImplementedException();
        }
        public override void GenerateBeams()
        {
            throw new NotImplementedException();
        }

        public override void GenerateLowerNodes(List<Point3d> points, double difference)
        {
            throw new NotImplementedException();
        }
    }
}
