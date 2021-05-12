using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Columns;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public class FlatTruss : PichedTruss
    {
        public FlatTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            GenerateTopBars();
            StaticColumns = new List<Column>(new StaticColumn(Line.Unset).GenerateColumns(StartingNodes, plane));
            GenerateThickBottomBars();
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options.ArticulationType);
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length / 2, Length / 2, Height, Height, Height);
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