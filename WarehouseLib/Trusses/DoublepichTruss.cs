using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Columns;
using WarehouseLib.Options;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib.Trusses
{
    public class DoublepichedTruss : PichedTruss
    {
        public double RightLength;
        public double LeftLength;
        public int BaseType;

        public DoublepichedTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            RightLength = options.Width;
            LeftLength = options.Width;
            BaseType = options.BaseType;
            GenerateTopBars();
            StaticColumns=
                new List<Column>(new StaticColumn(Line.Unset).GenerateStaticColumns(StartingNodes, Plane));
            ChangeBaseByType(BaseType);
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options.ArticulationType);
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