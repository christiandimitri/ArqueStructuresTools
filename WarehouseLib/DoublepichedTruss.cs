﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public class DoublepichedTruss : PichedTruss
    {
        public DoublepichedTruss(Plane plane, double length, double height, double maxHeight,double clearHeight, int divisions) : base(plane, length, height, maxHeight, clearHeight, divisions)
        {
            GenerateTopBars();
            GenerateColumns();
            GenerateBottomBars();
            GenerateNodes(divisions);
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length, Height, MaxHeight, Height);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[2], StartingNodes[1]);
            TopBars = new List<Curve> { barA.ToNurbsCurve(), barB.ToNurbsCurve() };
        }
    }
}
