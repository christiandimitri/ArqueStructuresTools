using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Columns;
using WarehouseLib.Options;

// ReSharper disable VirtualMemberCallInConstructor

namespace WarehouseLib.Trusses
{
    public class DoublepichTruss : PichedTruss
    {
        public TrussOptions _options;

        public DoublepichTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            _options = options;
            GenerateTopBars();
            StaticColumns=
                new List<Column>(new StaticColumn(Line.Unset).GenerateColumns(StartingNodes, plane));
            ChangeBaseByType(options.BaseType);
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options.ArticulationType);
            
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, _options.Width, _options.Width, Height, MaxHeight, Height);
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