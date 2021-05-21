using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public class FlatTruss : PichedTruss
    {
        public FlatTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            GenerateTopBars();
            StaticColumns = new List<Column>(new StaticColumn().GenerateColumns(StartingNodes, plane));
            GenerateThickBottomBars();
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options._articulationType);
            ConstructBeams();
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(_plane, _length / 2, _length / 2, _height, _height, _height);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[1], StartingNodes[2]);
            TopBeamAxisCurves = new List<Curve> {barA.ToNurbsCurve(), barB.ToNurbsCurve()};
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            GenerateVerticalBottomNodes(crv);
        }
    }
}