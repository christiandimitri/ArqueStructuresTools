using System;
using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;
using WarehouseLib.Columns;
using WarehouseLib.Options;
using WarehouseLib.Straps;
using WarehouseLib.Trusses;

namespace WarehouseLib.Warehouses
{
    public class Warehouse
    {
        private Plane _plane;
        private readonly TrussOptions _trussOptions;
        public List<Truss> Trusses;
        public List<Point3d> Nodes;
        private List<Column> _staticColumns;
        private List<Column> _boundaryColumns;
        public List<Strap> RoofStraps;
        public List<Strap> FacadeStrapsX;
        public List<Strap> FacadeStrapsY;
        public List<Bracing> RoofBracings;
        public List<Bracing> ColumnsBracings;
        public List<Cable> RoofCables;
        public List<Cable> FacadeCables;
        private readonly WarehouseOptions _warehouseOptions;

        public Warehouse(Plane plane, TrussOptions trussOptions, WarehouseOptions warehouseOptions)
        {
            if (trussOptions.Width <= 0) throw new Exception("Warehouse cannot have 0 width!!");
            if (trussOptions.Height <= 0) throw new Exception("Warehouse cannot have 0 height!!");
            if (trussOptions.MaxHeight <= 0) throw new Exception("Warehouse cannot have 0 max height!!");
            _plane = plane;
            _trussOptions = trussOptions;
            _warehouseOptions = warehouseOptions;
            ConstructTrusses(trussOptions);
            GetColumns();
            GenerateRoofStraps();
            GenerateFacadeStraps();
            GenerateRoofBracing();
            GenerateColumnsBracing();
            GenerateFacadeBracing();
        }

        private void ConstructTrusses(TrussOptions trussOptions)
        {
            var trusses = new List<Truss>();
            for (int i = 0; i < _warehouseOptions.PorticoCount + 1; i++)
            {
                var span = (_warehouseOptions.Length / _warehouseOptions.PorticoCount * i);
                var tempPlane = new Plane(_plane.PointAt(0, span, 0), _plane.ZAxis);
                if (_warehouseOptions.Typology == GeometricalTypology.Flat.ToString())
                {
                    var trussA = new FlatTruss(tempPlane, trussOptions);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Arch.ToString())
                {
                    var trussA = new ArchTruss(tempPlane, trussOptions);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Monopich.ToString())
                {
                    var trussA = new MonopichTruss(tempPlane, trussOptions);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Doublepich.ToString())
                {
                    var trussA = new DoublepichTruss(tempPlane, trussOptions);
                    trusses.Add(trussA);
                }
            }

            if (_warehouseOptions.HasBoundary == true)
            {
                trusses = new List<Truss>(WarehouseHasPorticoAtBoundaries(trusses));
            }

            Trusses = trusses;
        }

        private List<Truss> WarehouseHasPorticoAtBoundaries(List<Truss> trusses)
        {
            var trussA = trusses[0];
            trussA.ConstructPorticoFromTruss(trussA);
            var trussB = trusses[trusses.Count - 1];
            trussB.ConstructPorticoFromTruss(trussB);

            trusses.RemoveAt(0);
            trusses.Insert(0, trussA);
            trusses.RemoveAt(trusses.Count - 1);
            trusses.Add(trussB);

            return trusses;
        }

        private void GenerateRoofStraps()
        {
            RoofStraps = new List<Strap>();
            var tempStraps = new List<Strap>();
            var straps =
                new RoofStrap().ConstructStraps(new RoofStrap().ConstructRoofStraps(Trusses, 0));
            foreach (var strap in straps)
            {
                tempStraps.Add(strap);
            }

            RoofStraps = tempStraps;
        }

        private void GenerateFacadeStraps()
        {
            FacadeStrapsX = new List<Strap>();
            FacadeStrapsY = new List<Strap>();
            var tempStraps = new List<Strap>();
            var strapsX =
                new FacadeStrap().ConstructStraps(
                    new FacadeStrap().ConstructStrapsAxisOnStaticColumns(Trusses,
                        _warehouseOptions.FacadeStrapsDistance));
            foreach (var strap in strapsX)
            {
                tempStraps.Add(strap);
            }

            FacadeStrapsX = tempStraps;
            tempStraps = new List<Strap>();
            var boundary = new List<Truss> {Trusses[0], Trusses[Trusses.Count - 1]};
            var strapsY =
                new FacadeStrap().ConstructStraps(
                    new FacadeStrap().ConstructStrapsAxisOnBoundaryColumns(boundary,
                        _warehouseOptions.FacadeStrapsDistance,
                        _warehouseOptions.HasBoundary));

            foreach (var strap in strapsY)
            {
                tempStraps.Add(strap);
            }

            FacadeStrapsY = tempStraps;
        }

        private void GenerateFacadeBracing()
        {
            var cables = new List<Cable>();
            for (int i = 0; i < Trusses[0].StaticColumns.Count; i++)
            {
                var trussA = Trusses[0];
                var trussB = Trusses[1];
                var nodes = new List<Point3d>
                {
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtStart,
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtEnd
                };
                var beamB = trussB.StaticColumns[i].Axis.ToNurbsCurve();
                var cable = new FacadeCable {Threshold = _warehouseOptions.FacadeCablesThreshold};
                cables.AddRange(cable.ConstructCables(nodes, beamB));

                trussA = Trusses[Trusses.Count - 2];
                trussB = Trusses[Trusses.Count - 1];
                nodes = new List<Point3d>
                {
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtStart,
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtEnd
                };
                beamB = trussB.StaticColumns[i].Axis.ToNurbsCurve();
                cable=new FacadeCable();
                cable.Threshold = _warehouseOptions.FacadeCablesThreshold;
                cables.AddRange(cable.ConstructCables(nodes, beamB));
            }

            FacadeCables = cables;
        }

        private void GenerateRoofBracing()
        {
            var startBracingPoints = ExtractRoofBracingPoints(Trusses[0]);
            var startTopBeam = Curve.JoinCurves(Trusses[1].TopBars)[0];
            var endBracingPoints = ExtractRoofBracingPoints(Trusses[Trusses.Count - 1]);
            var endTopBeam = Curve.JoinCurves(Trusses[Trusses.Count - 2].TopBars)[0];

            if (_warehouseOptions.PorticoCount <= 2) throw new Exception("Portico count has to be >2");
            RoofBracings = new List<Bracing>();
            RoofCables = new List<Cable>();
            if (_warehouseOptions.RoofBracingType == RoofBracingType.Bracing.ToString())
            {
                var roofBracingsStart =
                    new RoofBracing().ConstructWarrenStudsBracings(startBracingPoints, startTopBeam);
                RoofBracings.AddRange(roofBracingsStart);
                var roofBracingsEnd =
                    new RoofBracing().ConstructWarrenStudsBracings(endBracingPoints, endTopBeam);
                RoofBracings.AddRange(roofBracingsEnd);
            }
            else if (_warehouseOptions.RoofBracingType == RoofBracingType.Cable.ToString())
            {
                var roofBracingsStart =
                    new RoofBracing().ConstructBracings(startBracingPoints, startTopBeam);

                var roofCablesStart =
                    new RoofCable().ConstructCables(startBracingPoints, startTopBeam);
                RoofBracings.AddRange(roofBracingsStart);
                RoofCables.AddRange(roofCablesStart);

                var roofBracingsEnd =
                    new RoofBracing().ConstructBracings(endBracingPoints, endTopBeam);
                var roofCablesEnd =
                    new RoofCable().ConstructCables(endBracingPoints, endTopBeam);
                RoofBracings.AddRange(roofBracingsEnd);
                RoofCables.AddRange(roofCablesEnd);
            }
            
            foreach (var truss in Trusses)
            {
                if (truss.BoundaryColumns != null)
                {
                    truss.BoundaryColumns.RemoveAt(0);
                    truss.BoundaryColumns.RemoveAt(truss.BoundaryColumns.Count - 1);
                }
                
            }
        }

        private void GenerateColumnsBracing()
        {
            var tempBracings = new List<Bracing>();

            for (int i = 0; i < Trusses.Count - 1; i++)
            {
                var trussA = Trusses[i];
                var trussB = Trusses[i + 1];
                var tempList = trussA.StartingNodes;
                if (i == 0 || i == Trusses.Count - 2)
                {
                    tempList.RemoveAt(1);
                }

                var nodes = tempList;
                var beam = Curve.JoinCurves(trussB.TopBars)[0];
                tempBracings.AddRange(new ColumnsBracing().ConstructBracings(nodes, beam));
            }

            ColumnsBracings = tempBracings;
        }

        private List<Point3d> ExtractRoofBracingPoints(Truss truss)
        {
            var trussA = truss;
            var columns = (_trussOptions.ColumnsCount <= 2 || _warehouseOptions.HasBoundary == false)
                ? trussA.StaticColumns
                : trussA.BoundaryColumns;
            var tempPointList = new List<Point3d>();
            foreach (var column in columns)
            {
                tempPointList.Add(column.Axis.ToNurbsCurve().PointAtEnd);
            }

            if (_warehouseOptions.HasBoundary == false || _trussOptions.ColumnsCount % 2 == 0)
            {
                tempPointList.Insert(tempPointList.Count / 2, trussA.TopBars[0].PointAtEnd);
            }

            return tempPointList;
        }

        private void GetColumns()
        {
            var boundaryList = new List<Column>();
            var staticList = new List<Column>();
            foreach (var truss in Trusses)
            {
                if (truss.BoundaryColumns != null && _trussOptions.ColumnsCount >= 1)
                {
                    foreach (var bc in truss.BoundaryColumns)
                    {
                        boundaryList.Add(bc);
                    }
                }
                else if (truss.StaticColumns != null)
                {
                    foreach (var sc in truss.StaticColumns)
                    {
                        staticList.Add(sc);
                    }
                }
            }

            _staticColumns = staticList;
            _boundaryColumns = boundaryList;
        }
    }
}