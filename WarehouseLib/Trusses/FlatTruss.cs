using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
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
            ConstructBeams(true, true);
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
        protected override void RecomputeNodes(int index)
        {
            List<Point3d> tempTopList = new List<Point3d>();
            List<Point3d> tempBottomList = new List<Point3d>();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                if (_trussType == ConnectionType.Warren.ToString())
                {
                    if (i % 2 == 0)
                    {
                        tempTopList.Add(TopNodes[i]);
                    }
                    else if (i % 2 == 1)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_trussType == ConnectionType.WarrenStuds.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    if (i % 2 == 1 || i == TopNodes.Count - 1 || i == 0)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_trussType == ConnectionType.Howe.ToString() || _trussType == ConnectionType.Pratt.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    tempBottomList.Add(BottomNodes[i]);
                }
            }

            if (_trussType == ConnectionType.Warren.ToString())
            {
                tempBottomList.Insert(0, BottomNodes[0]);
                tempBottomList.Add(BottomNodes[BottomNodes.Count - 1]);
            }

            TopNodes = new List<Point3d>(tempTopList);
            BottomNodes = new List<Point3d>(tempBottomList);
            if (ConnectionType.Warren.ToString() == _trussType)
            {
                IntermediateBeamsAxisCurves.RemoveAt(index);
            }

        }
    }
}