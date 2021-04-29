using System;
using Rhino.Geometry;
using System.Collections.Generic;
using Rhino.Geometry.Intersect;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib
{
    public class DoublepichedTruss : PichedTruss
    {
        private double RightLength;
        private double LeftLength;
        private int BaseType;
        public DoublepichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
            int divisions, string trussType, string articulationType, double rightLength, double leftLength,
            int baseType) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType, articulationType)
        {
            RightLength = rightLength;
            LeftLength = leftLength;
            BaseType = baseType;
            GenerateTopBars();
            GenerateColumns();
            ChangeBaseByType(baseType);
            ConstructTruss(divisions);
            ChangeArticulationAtColumnsByType(articulationType);
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, LeftLength, RightLength, Height, MaxHeight, Height);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[1], StartingNodes[2]);
            TopBars = new List<Curve> {barA.ToNurbsCurve(), barB.ToNurbsCurve()};
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            if ((TrussType == "Warren" || TrussType == "Warren_Studs") && BaseType == 0 )
            {
                GeneratePerpendicularBottomNodes(crv);
            }
            else
            {
                GenerateVerticalBottomNodes(crv);
            }
        }
        public override void GeneratePerpendicularBottomNodes(Curve crv)
        {
            var nodes = new List<Point3d>();
            var points = new List<Point3d>(TopNodes);
            BottomNodes = new List<Point3d>();
            foreach (var pt in points)
            {
                double t;
                crv.ClosestPoint(pt, out t);
                nodes.Add(crv.PointAt(t));
            }

            BottomNodes.AddRange(nodes);
        }
    }
}