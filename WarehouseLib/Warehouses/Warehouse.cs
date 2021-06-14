using System;
using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Crosses;
using WarehouseLib.Options;
using WarehouseLib.Straps;
using WarehouseLib.Trusses;

namespace WarehouseLib.Warehouses
{
    public class Warehouse
    {
        public TrussInputs TrussInputs;
        public WarehouseOptions _warehouseOptions;
        private List<Column> _boundaryColumns;
        public Plane _plane;
        private List<Column> _staticColumns;
        public List<Bracing> ColumnsBracings;
        public List<Cross> Crosses;
        public List<Cable> FacadeCables;
        public List<Strap> FacadeStrapsX;
        public List<Strap> FacadeStrapsY;
        public List<Point3d> Nodes;
        public List<Bracing> RoofBracings;
        public List<Cable> RoofCables;
        public List<Strap> RoofStraps;
        public List<Truss> Trusses;

        public Warehouse(Plane plane, TrussInputs trussInputs, WarehouseOptions warehouseOptions)
        {
            if (trussInputs.Width <= 0) throw new Exception("Warehouse cannot have 0 width!!");
            if (trussInputs.Height <= 0) throw new Exception("Warehouse cannot have 0 height!!");
            if (trussInputs.MaxHeight <= 0) throw new Exception("Warehouse cannot have 0 max height!!");

            _plane = plane;
            TrussInputs = trussInputs;
            _warehouseOptions = warehouseOptions;
            ConstructTrusses(trussInputs);
            GetColumns();
            GenerateRoofStraps();
            GenerateFacadeStraps();
            GenerateRoofBracing();
            GenerateColumnsBracing();
            GenerateFacadeBracing();
            GenerateStAndresCross();
        }

        private void ConstructTrusses(TrussInputs trussInputs)
        {
            var trusses = new List<Truss>();
            for (var i = 0; i < _warehouseOptions.PorticoCount + 1; i++)
            {
                var span = _warehouseOptions.Length / _warehouseOptions.PorticoCount * i;
                var tempPlane = new Plane(_plane.PointAt(0, span, 0), _plane.ZAxis);
                if (_warehouseOptions.Typology == GeometricalTypology.Flat.ToString())
                {
                    var trussA = new FlatTruss(tempPlane, trussInputs);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Arch.ToString())
                {
                    var trussA = new ArchTruss(tempPlane, trussInputs);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Monopich.ToString())
                {
                    var trussA = new MonopichTruss(tempPlane, trussInputs);
                    trusses.Add(trussA);
                }
                else if (_warehouseOptions.Typology == GeometricalTypology.Doublepich.ToString())
                {
                    var trussA = new DoublepichTruss(tempPlane, trussInputs);
                    trusses.Add(trussA);
                }
            }

            if (_warehouseOptions.HasBoundary) trusses = new List<Truss>(WarehouseHasPorticoAtBoundaries(trusses));

            Trusses = trusses;
        }

        private List<Truss> WarehouseHasPorticoAtBoundaries(List<Truss> trusses)
        {
            var trussA = trusses[0];
            trussA.ConstructPorticoFromTruss(trussA, 0);
            var trussB = trusses[trusses.Count - 1];
            trussB.ConstructPorticoFromTruss(trussB, trusses.Count - 1);

            trusses.RemoveAt(0);
            trusses.Insert(0, trussA);
            trusses.RemoveAt(trusses.Count - 1);
            trusses.Add(trussB);
            for (int i = 0; i < trusses.Count; i++)
            {
                var truss = trusses[i];
                truss.UpdatePorticoType(truss);
            }

            return trusses;
        }

        private void GenerateRoofStraps()
        {
            RoofStraps = new List<Strap>();
            var tempStraps = new List<Strap>();
            var straps = new RoofStrap().ConstructRoofStraps(Trusses);
            foreach (var strap in straps) tempStraps.Add(strap);
            RoofStraps = tempStraps;
        }

        private void GenerateFacadeStraps()
        {
            FacadeStrapsX = new List<Strap>();
            FacadeStrapsY = new List<Strap>();
            FacadeStrapsX = new FacadeStrap().ConstructStrapsOnStaticColumns(
                Trusses, TrussInputs.FacadeStrapsDistance);

            var boundary = new List<Truss> {Trusses[0], Trusses[Trusses.Count - 1]};
            FacadeStrapsY = new FacadeStrap().ConstructStrapsOnBoundaryColumns(boundary,
                TrussInputs.FacadeStrapsDistance, _warehouseOptions.HasBoundary);
        }

        private void GenerateFacadeBracing()
        {
            var cables = new List<Cable>();
            for (var i = 0; i < Trusses[0].StaticColumns.Count; i++)
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
                cables.AddRange(cable.ConstructCables(nodes, beamB, _plane, i));

                trussA = Trusses[Trusses.Count - 2];
                trussB = Trusses[Trusses.Count - 1];
                nodes = new List<Point3d>
                {
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtStart,
                    trussA.StaticColumns[i].Axis.ToNurbsCurve().PointAtEnd
                };
                beamB = trussB.StaticColumns[i].Axis.ToNurbsCurve();
                cable = new FacadeCable();
                cable.Threshold = _warehouseOptions.FacadeCablesThreshold;
                cables.AddRange(cable.ConstructCables(nodes, beamB, _plane, i));
            }

            FacadeCables = cables;
        }

        private void GenerateRoofBracing()
        {
            var startBracingPoints = ExtractRoofBracingPoints(Trusses[0]);
            var startTopBeam = Curve.JoinCurves(Trusses[1].TopBeamAxisCurves)[0];
            // var startTopBeam = Trusses[1].TopBeamAxisCurves[0];
            var endBracingPoints = ExtractRoofBracingPoints(Trusses[Trusses.Count - 1]);
            var endTopBeam = Curve.JoinCurves(Trusses[Trusses.Count - 2].TopBeamAxisCurves)[0];
            // var endTopBeam = Trusses[Trusses.Count - 2].TopBeamAxisCurves[0];

            if (_warehouseOptions.PorticoCount <= 2) throw new Exception("Portico count has to be >2");

            RoofBracings = new List<Bracing>();
            RoofCables = new List<Cable>();
            if (_warehouseOptions.RoofBracingType == RoofBracingType.Bracing.ToString())
            {
                var roofBracingStart =
                    new RoofBracing().ConstructWarrenStudsBracings(startBracingPoints, startTopBeam, _plane, 0);
                RoofBracings.AddRange(roofBracingStart);
                var roofBracingEnd =
                    new RoofBracing().ConstructWarrenStudsBracings(endBracingPoints, endTopBeam, _plane,
                        Trusses.Count - 1);
                RoofBracings.AddRange(roofBracingEnd);
            }
            else if (_warehouseOptions.RoofBracingType == RoofBracingType.Cable.ToString())
            {
                var roofBracingStart =
                    new RoofBracing().ConstructBracings(startBracingPoints, startTopBeam, _plane, 0);

                var roofCablesStart =
                    new RoofCable().ConstructCables(startBracingPoints, startTopBeam, _plane, 0);
                RoofBracings.AddRange(roofBracingStart);
                RoofCables.AddRange(roofCablesStart);

                var roofBracingEnd =
                    new RoofBracing().ConstructBracings(endBracingPoints, endTopBeam, _plane, Trusses.Count - 1);
                var roofCablesEnd =
                    new RoofCable().ConstructCables(endBracingPoints, endTopBeam, _plane, Trusses.Count - 1);
                RoofBracings.AddRange(roofBracingEnd);
                RoofCables.AddRange(roofCablesEnd);
            }

            foreach (var truss in Trusses)
                if (truss.BoundaryColumns != null)
                {
                    truss.BoundaryColumns.RemoveAt(0);
                    truss.BoundaryColumns.RemoveAt(truss.BoundaryColumns.Count - 1);
                }
        }

        private void GenerateColumnsBracing()
        {
            var tempBracings = new List<Bracing>();

            for (var i = 0; i < Trusses.Count - 1; i++)
            {
                var trussA = Trusses[i];
                var trussB = Trusses[i + 1];
                var tempList = trussA.StartingNodes;
                if (i == 0 || i == Trusses.Count - 2) tempList.RemoveAt(1);

                var nodes = tempList;
                var beam = Curve.JoinCurves(trussB.TopBeamAxisCurves)[0];
                tempBracings.AddRange(new ColumnsBracing().ConstructBracings(nodes, beam, _plane, 0));
            }

            ColumnsBracings = tempBracings;
        }

        private void GenerateStAndresCross()
        {
            Crosses = new List<Cross>();
            if (TrussInputs.TrussType != ConnectionType.Warren.ToString())
                for (var i = 1; i < Trusses.Count - 2; i++)
                {
                    var outerTopNodes =
                        new StAndre().ComputeCrossTopNodes(Trusses[i], _warehouseOptions.StAndreCrossCount);
                    var outerBottomNodes = new StAndre().ComputeCrossBottomNodes(Trusses[i], outerTopNodes);
                    var innerTopNodes =
                        new StAndre().ComputeCrossTopNodes(Trusses[i + 1], _warehouseOptions.StAndreCrossCount);
                    var innerBottomNodes = new StAndre().ComputeCrossBottomNodes(Trusses[i + 1], innerTopNodes);
                    var cross = new StAndre().ConstructCrosses(outerTopNodes, innerBottomNodes, outerBottomNodes,
                        innerTopNodes);
                    Crosses.AddRange(cross);
                }
        }

        private List<Point3d> ExtractRoofBracingPoints(Truss truss)
        {
            var trussA = truss;
            var columns = TrussInputs.ColumnsCount <= 2 || _warehouseOptions.HasBoundary == false
                ? trussA.StaticColumns
                : trussA.BoundaryColumns;
            var tempPointList = new List<Point3d>();
            foreach (var column in columns) tempPointList.Add(column.Axis.ToNurbsCurve().PointAtEnd);

            if (_warehouseOptions.HasBoundary == false || TrussInputs.ColumnsCount % 2 == 0)
                tempPointList.Insert(tempPointList.Count / 2, trussA.TopBeamAxisCurves[0].PointAtEnd);

            return tempPointList;
        }

        private void GetColumns()
        {
            var boundaryList = new List<Column>();
            var staticList = new List<Column>();
            foreach (var truss in Trusses)
                if (truss.BoundaryColumns != null && TrussInputs.ColumnsCount >= 1)
                    foreach (var bc in truss.BoundaryColumns)
                        boundaryList.Add(bc);
                else if (truss.StaticColumns != null)
                    foreach (var sc in truss.StaticColumns)
                        staticList.Add(sc);

            _staticColumns = staticList;
            _boundaryColumns = boundaryList;
        }
    }
}