using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Eto.Drawing;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;

namespace WarehouseLib
{
    public class Warehouse
    {
        public Plane Plane;
        public double Length;
        public double Width;
        public double Height;
        public double MaxHeight;
        public double ClearHeight;
        public int Typology;
        public int PoticsCount;
        public int ColumnsCount;
        public string TrussType;
        public string RoofBracingType;
        public List<Truss> Trusses;
        public List<Point3d> Nodes;
        public List<Column> StaticColumns;
        public List<Column> BoundaryColumns;
        public List<Strap> RoofStraps;
        public List<Strap> FacadeStrapsX;
        public List<Strap> FacadeStrapsY;
        public List<Bracing> RoofBracings;
        public List<Cable> RoofCables;

        public Warehouse(Plane plane, double length, double width, double height, double maxHeight, double clearHeight,
            int typology, int poticsCount, string trussType, int columnsCount, string roofBracingType)
        {
            if (width <= 0) throw new Exception("Warehouse cannot have 0 width!!");
            if (length <= 0) throw new Exception("Warehouse cannot have 0 length!!");
            if (height <= 0) throw new Exception("Warehouse cannot have 0 height!!");
            if (maxHeight <= 0) throw new Exception("Warehouse cannot have 0 max height!!");
            if (typology >= 4) throw new Exception("Warehouse root typology is between 0 to 3!!");
            Plane = plane;
            Length = length;
            Width = width;
            Height = height;
            MaxHeight = maxHeight;
            ClearHeight = clearHeight;
            Typology = typology;
            PoticsCount = poticsCount;
            TrussType = trussType;
            ColumnsCount = columnsCount;
            RoofBracingType = roofBracingType;
            GenerateTrusses();
            GenerateRoofStraps();
            GetColumns();
            GenerateFacadeStraps();
            GenerateBracings();
        }

        public void GenerateRoofStraps()
        {
            RoofStraps = new List<Strap>();
            var tempStraps = new List<Strap>();
            var straps = new RoofStrap(Line.Unset).ConstructRoofStraps(Trusses, 0);
            foreach (var strap in straps)
            {
                tempStraps.Add(new RoofStrap(strap.Axis));
            }

            RoofStraps = tempStraps;
        }

        public void GenerateFacadeStraps()
        {
            FacadeStrapsX = new List<Strap>();
            FacadeStrapsY = new List<Strap>();
            var tempStraps = new List<Strap>();
            var strapsX = new FacadeStrap(Line.Unset).ConstructStrapsAxisOnStaticColumns(Trusses, 0.5);
            foreach (var strap in strapsX)
            {
                tempStraps.Add(new FacadeStrap(strap.Axis));
            }

            FacadeStrapsX = tempStraps;
            tempStraps = new List<Strap>();
            var portics = new List<Truss> {Trusses[0], Trusses[Trusses.Count - 1]};
            var strapsY = new FacadeStrap(Line.Unset).ConstructStrapsAxisOnBoundaryColumns(portics, 0.5);

            foreach (var strap in strapsY)
            {
                tempStraps.Add(new FacadeStrap(strap.Axis));
            }

            FacadeStrapsY = tempStraps;
        }

        private void GenerateTrusses()
        {
            var trusses = new List<Truss>();
            for (int i = 0; i < PoticsCount; i++)
            {
                var span = (Width / PoticsCount) * i;
                var tempPlane = new Plane(Plane.PointAt(0, span, 0), Plane.ZAxis);
                if (Typology == 0)
                {
                    var trussA = new FlatTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 6, TrussType,
                        "Articulated", ColumnsCount);
                    trusses.Add(trussA);
                }
                else if (Typology == 1)
                {
                    var trussA = new ArchTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 6, TrussType,
                        "Articulated", 0, ColumnsCount);
                    trusses.Add(trussA);
                }
                else if (Typology == 2)
                {
                    var trussA = new MonopichedTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 6, TrussType,
                        "Articulated", 0, ColumnsCount);
                    trusses.Add(trussA);
                }
                else if (Typology == 3)
                {
                    var trussA = new DoublepichedTruss(tempPlane, 0, Height, MaxHeight, ClearHeight, 6, TrussType,
                        "Articulated", Length, Length * 0.8, 0, ColumnsCount);
                    trusses.Add(trussA);
                }
            }

            trusses = new List<Truss>(WarehouseHasPorticAtBoundaries(trusses));

            Trusses = trusses;
        }

        public List<Truss> WarehouseHasPorticAtBoundaries(List<Truss> trusses)
        {
            var trussA = trusses[0];
            trussA.ConstructPorticFromTruss(trussA);
            var trussB = trusses[trusses.Count - 1];
            trussB.ConstructPorticFromTruss(trussB);

            trusses.RemoveAt(0);
            trusses.Insert(0, trussA);
            trusses.RemoveAt(trusses.Count - 1);
            trusses.Add(trussB);

            return trusses;
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

        private void GenerateBracings()
        {
            if (PoticsCount < 2) throw new Exception("Portics count has to be >=2");
            RoofBracings = new List<Bracing>();
            RoofCables = new List<Cable>();
            if (RoofBracingType == "Bracing")
            {
                var roofBracingsStart =
                    new RoofBracing(Line.Unset).ConstructWarrenStudsBracings(Trusses, 0, ColumnsCount);
                RoofBracings.AddRange(roofBracingsStart);
                // var roofBracingsEnd =
                //     new RoofBracing(Line.Unset).ConstructWarrenStudsBracings(Trusses, Trusses.Count - 1, ColumnsCount);
                // RoofBracings.AddRange(roofBracingsEnd);
            }
            else if (RoofBracingType == "Cable")
            {
                var roofBracingsStart =
                    new RoofBracing(Line.Unset).ConstructBracings(Trusses, 0, ColumnsCount);
                var roofCablesStart =
                    new RoofCable(Line.Unset).ConstructCables(Trusses, 0, ColumnsCount);
                RoofBracings.AddRange(roofBracingsStart);
                RoofCables.AddRange(roofCablesStart);
            }
        }
    }
}