using System.Collections.Generic;
using Rhino.Geometry;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib.Trusses
{
    public class DoublepichedTruss : PichedTruss
    {
        public double RightLength;
        public double LeftLength;
        public int BaseType;

        public DoublepichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
            int divisions, string trussType, string articulationType, double rightLength, double leftLength,
            int baseType, int columnsCount) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType, articulationType, columnsCount)
        {
            RightLength = rightLength;
            LeftLength = leftLength;
            BaseType = baseType;
            GenerateTopBars();
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
            GenerateVerticalBottomNodes(crv);
        }
    }
}