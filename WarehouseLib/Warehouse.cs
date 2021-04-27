using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<Plane> TempPlanes;
        public List<Column> Columns;

        public Warehouse(Plane plane,double length, double width, double height, double maxHeight,double clearHeight, int typology, int count, string trussType)
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
        }

        private void GenerateColumns()
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            foreach (var truss in Trusses)
            {
                Line axisA = new Line(new Point3d(truss.StartingNodes[0].X, truss.StartingNodes[0].Y,Plane.Origin.Z), truss.StartingNodes[0]);
                Line axisB = new Line(new Point3d(truss.StartingNodes[2].X, truss.StartingNodes[2].Y, Plane.Origin.Z), truss.StartingNodes[2]);
                columns.Add(new Column(axisA));
                columns.Add(new Column(axisB));
            }
            Columns = columns;
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
                var tempPlane = new Plane(Plane.PointAt(0,span, 0), Plane.ZAxis);
                if (Typology ==0)
                {
                    var trussA = new FlatTruss(tempPlane, Length, Height, MaxHeight,ClearHeight, 4, TrussType, "Articulated");
                    trusses.Add(trussA);
                }
                else if(Typology == 1)
                {
                    var trussA = new ArchTruss(tempPlane, Length, Height, MaxHeight,ClearHeight, 4, TrussType, "Articulated");
                    trusses.Add(trussA);
                }
                else if(Typology == 2)
                {
                    var trussA = new MonopichedTruss(tempPlane, Length, Height, MaxHeight,ClearHeight, 4, TrussType,"Articulated");
                    trusses.Add(trussA);
                }
                else if (Typology == 3)
                {
                    var trussA = new DoublepichedTruss(tempPlane, 0, Height, MaxHeight,ClearHeight, 4, TrussType, "Articulated", Length,Length*0.8);
                    trusses.Add(trussA);
                }
            }
            Trusses = trusses;
        }
    }
}
