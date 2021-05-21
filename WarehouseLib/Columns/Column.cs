using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Columns
{
    public abstract class Column
    {
        public Line Axis;
        public Plane ProfileOrientitionPlane { get; set; }

        protected Column()
        {
        }

        protected Line ConstructAxis(Point3d node, Plane plane)
        {
            var axis = new Line(plane.ClosestPoint(node), node);
            return axis;
        }

        public List<Column> SplitColumnByArticulation(Column column, Point3d articulationPoint)
        {
            column.Axis.ToNurbsCurve().ClosestPoint(articulationPoint, out double t);
            var axisList = column.Axis.ToNurbsCurve().Split(t).ToList();
            var columns = new List<Column>();
            foreach (var axis in axisList)
            {
                var tempColumn = new StaticColumn();
                tempColumn.Axis = new Line(axis.PointAtStart, axis.PointAtEnd);
                columns.Add(tempColumn);
            }

            return columns;
        }

        public abstract Plane GetColumnsOrientationPlane(Point3d node, Plane plane, int index);
    }
}