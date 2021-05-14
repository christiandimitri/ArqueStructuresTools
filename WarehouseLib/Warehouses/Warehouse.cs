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
        public Plane Plane;

        public TrussOptions _trussOptions;

        // public double Width;
        // public double Height;
        // public double MaxHeight;
        // public double ClearHeight;
        public int ColumnsCount;
        public string TrussType;
        public List<Truss> Trusses;
        public List<Point3d> Nodes;
        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;
        public List<Strap> RoofStraps;
        public List<Strap> FacadeStrapsX;
        public List<Strap> FacadeStrapsY;
        public List<Bracing> RoofBracings;
        public List<Cable> RoofCables;
        public WarehouseOptions _warehouseOptions;

        public Warehouse(Plane plane, TrussOptions trussOptions, WarehouseOptions warehouseOptions)
        {
            if (trussOptions.Width <= 0) throw new Exception("Warehouse cannot have 0 width!!");
            if (trussOptions.Height <= 0) throw new Exception("Warehouse cannot have 0 height!!");
            if (trussOptions.MaxHeight <= 0) throw new Exception("Warehouse cannot have 0 max height!!");
            if (!Enum.IsDefined(typeof(GeometricalTypology), warehouseOptions.Typology))
                throw new Exception("Warehouse roof typology should be either: Flat, Arch, Monopich, Doublepich");
            if (warehouseOptions.Length <= 0) throw new Exception("Warehouse cannot have 0 length!!");
            if (warehouseOptions.PorticoCount <= 1) throw new Exception("Warehouse cannot have portico count < 2");
            Plane = plane;
            _trussOptions = trussOptions;
            _warehouseOptions = warehouseOptions;
            ConstructTrusses(trussOptions);
            GetColumns();
            GenerateRoofStraps();
            GenerateFacadeStraps();
            // GenerateRoofBracings();
        }

        private void ConstructTrusses(TrussOptions trussOptions)
        {
            var trusses = new List<Truss>();
            for (int i = 0; i < _warehouseOptions.PorticoCount+1; i++)
            {
                var span = (_warehouseOptions.Length / _warehouseOptions.PorticoCount * i);
                var tempPlane = new Plane(Plane.PointAt(0, span, 0), Plane.ZAxis);
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
                    new FacadeStrap().ConstructStrapsAxisOnStaticColumns(Trusses, 0.5));
            foreach (var strap in strapsX)
            {
                tempStraps.Add(strap);
            }

            FacadeStrapsX = tempStraps;
            // tempStraps = new List<Strap>();
            // var portics = new List<Truss> {Trusses[0], Trusses[Trusses.Count - 1]};
            // var strapsY =
            //     new FacadeStrap(Line.Unset).ConstructStraps(
            //         new FacadeStrap(Line.Unset).ConstructStrapsAxisOnBoundaryColumns(portics, 0.5));
            //
            // foreach (var strap in strapsY)
            // {
            //     tempStraps.Add(new FacadeStrap(strap.Axis));
            // }
            //
            // FacadeStrapsY = tempStraps;
        }

        private void GenerateRoofBracings()
        {
            if (_warehouseOptions.PorticoCount <= 2) throw new Exception("Portics count has to be >2");
            RoofBracings = new List<Bracing>();
            RoofCables = new List<Cable>();
            if (_warehouseOptions.RoofBracingType == "Bracing")
            {
                var roofBracingsStart =
                    new RoofBracing().ConstructWarrenStudsBracings(Trusses, ColumnsCount, 0);
                RoofBracings.AddRange(roofBracingsStart);
                var roofBracingsEnd =
                    new RoofBracing().ConstructWarrenStudsBracings(Trusses, ColumnsCount, Trusses.Count - 1);
                RoofBracings.AddRange(roofBracingsEnd);
            }
            else if (_warehouseOptions.RoofBracingType == "Cable")
            {
                var roofBracingsStart =
                    new RoofBracing().ConstructBracings(Trusses, ColumnsCount, 0);
                var roofCablesStart =
                    new RoofCable(Line.Unset, 0, ColumnsCount).ConstructCables(Trusses);
                RoofBracings.AddRange(roofBracingsStart);
                RoofCables.AddRange(roofCablesStart);
                var roofBracingsEnd =
                    new RoofBracing().ConstructBracings(Trusses, ColumnsCount, Trusses.Count - 1);
                var roofCablesEnd =
                    new RoofCable(Line.Unset, Trusses.Count - 1, ColumnsCount).ConstructCables(Trusses);
                RoofBracings.AddRange(roofBracingsEnd);
                RoofCables.AddRange(roofCablesEnd);
            }
        }

        private void GetColumns()
        {
            var boundaryList = new List<Column>();
            var staticList = new List<Column>();
            foreach (var truss in Trusses)
            {
                if (truss.BoundaryColumns != null && ColumnsCount >= 1)
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

            StaticColumns = staticList;
            BoundaryColumns = boundaryList;
        }
    }
}