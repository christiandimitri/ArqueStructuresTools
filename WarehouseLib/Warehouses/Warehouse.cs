using Rhino.Geometry;
using System;
using System.Collections.Generic;

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
        public int Count;
        public string TrussType;
        public List<Truss> Trusses;
        public List<Point3d> Nodes;
        public List<Column> Columns;
        public List<Strap> RoofStraps;
        public List<Strap> FacadeStraps;

        public Warehouse(Plane plane, double length, double width, double height, double maxHeight, double clearHeight,
            int typology, int count, string trussType)
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
            Count = count;
            TrussType = trussType;
            GenerateTrusses();
            GenerateNodes();
            GenerateColumns();
            GenerateRoofStraps();
        }

        private void GenerateColumns()
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            foreach (var truss in Trusses)
            {
                Line axisA = new Line(new Point3d(truss.StartingNodes[0].X, truss.StartingNodes[0].Y, Plane.Origin.Z),
                    truss.StartingNodes[0]);
                Line axisB = new Line(new Point3d(truss.StartingNodes[2].X, truss.StartingNodes[2].Y, Plane.Origin.Z),
                    truss.StartingNodes[2]);
                columns.Add(new Column(axisA));
                columns.Add(new Column(axisB));
            }

            Columns = columns;
        }

        public void GenerateRoofStraps()
        {
            RoofStraps = new List<Strap>();
            var straps = new RoofStrap(Line.Unset).ConstructStrapsAxis(Trusses, 0);
            foreach (var strap in straps)
            {
                RoofStraps.Add(new RoofStrap(strap.Axis));
            }
        }

        public void GenerateFacadeStraps()
        {
            FacadeStraps = new List<Strap>();
            var straps = new FacadeStrap(Line.Unset).ConstructStrapsAxis(Trusses, 0.5);
            foreach (var strap in straps)
            {
                FacadeStraps.Add(new FacadeStrap(strap.Axis));
            }
        }

        private void GenerateNodes()
        {
            var nodes = new List<Point3d>();
            foreach (var truss in Trusses)
            {
                //nodes = truss.StartingNodes;
                nodes = truss.TopNodes;
                nodes = truss.BottomNodes;
            }

            Nodes = nodes;
        }

        private void GenerateTrusses()
        {
            var trusses = new List<Truss>();
            for (int i = 0; i <= Count; i++)
            {
                var span = (Width / Count) * i;
                var tempPlane = new Plane(Plane.PointAt(0, span, 0), Plane.ZAxis);
                if (Typology == 0)
                {
                    var trussA = new FlatTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 4, TrussType,
                        "Articulated");
                    trusses.Add(trussA);
                }
                else if (Typology == 1)
                {
                    var trussA = new ArchTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 4, TrussType,
                        "Articulated", 0);
                    trusses.Add(trussA);
                }
                else if (Typology == 2)
                {
                    var trussA = new MonopichedTruss(tempPlane, Length, Height, MaxHeight, ClearHeight, 4, TrussType,
                        "Articulated", 0);
                    trusses.Add(trussA);
                }
                else if (Typology == 3)
                {
                    var trussA = new DoublepichedTruss(tempPlane, 0, Height, MaxHeight, ClearHeight, 4, TrussType,
                        "Articulated", Length, Length * 0.8, 0);
                    trusses.Add(trussA);
                }
            }

            trusses = new List<Truss>(WarehouseHasPorticAtBoundary(trusses));

            Trusses = trusses;
        }

        public List<Truss> WarehouseHasPorticAtBoundary(List<Truss> trusses)
        {
            var trussA = trusses[0];
            trussA.ConstructPorticFromTruss();
            var trussB = trusses[trusses.Count - 1];
            trussB.ConstructPorticFromTruss();
            trusses.RemoveAt(0);
            trusses.Insert(0, trussA);
            trusses.RemoveAt(trusses.Count - 1);
            trusses.Add(trussB);
            return trusses;
        }
    }
}