using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Columns;
using WarehouseLib.Options;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib.Trusses
{
    public class MonopichTruss : PichedTruss
    {
        private TrussOptions _options;

        public MonopichTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            _options = options;
            GenerateTopBars();
            StaticColumns = new List<Column>(new StaticColumn(Line.Unset).GenerateColumns(StartingNodes, plane));
            ChangeBaseByType(options.BaseType);
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options.ArticulationType);
            
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length / 2, Length / 2, Height,
                Height + ((MaxHeight - Height) / 2), MaxHeight);
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