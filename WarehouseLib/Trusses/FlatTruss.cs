using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Columns;

namespace WarehouseLib.Trusses
{
    public class FlatTruss : PichedTruss
    {
        public FlatTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions,
            string trussType, string articulationType, int columnsCount) : base(plane, length, height, maxHeight,
            clearHeight, divisions, trussType, articulationType, columnsCount)
        {
            GenerateTopBars();
            StaticColumns=new List<Column>(new StaticColumn(Line.Unset).GenerateStaticColumns(StartingNodes, Plane));
            GenerateThickBottomBars();
            ConstructTruss(divisions);
            ChangeArticulationAtColumnsByType(articulationType);
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