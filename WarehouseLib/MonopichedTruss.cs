using Rhino.Geometry;
using System.Collections.Generic;
// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib
{
    public class MonopichedTruss : PichedTruss
    {
        public int BaseType;
        public MonopichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
            int divisions, string trussType, string articulationType, int baseType) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType, articulationType)
        {
            BaseType = baseType;
            GenerateTopBars();
            GenerateColumns();
            ChangeBaseByType(baseType);
            ConstructTruss(divisions);
            // ChangeArticulationAtColumnsByType(articulationType);
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length/2, Length/2,Height, Height + ((MaxHeight - Height) / 2), MaxHeight);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[1], StartingNodes[2]);
            TopBars = new List<Curve> {barA.ToNurbsCurve(), barB.ToNurbsCurve()};
        }
    }
}