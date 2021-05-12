using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public abstract class Column
    {
        public Line Axis;
        protected Column()
        {
        }

        public abstract List<Column> GenerateColumns(List<Point3d> nodes, Plane plane);

        protected Line ConstructAxis(Point3d node, Plane plane)
        {
            var axis = new Line(plane.ClosestPoint(node), node);
            return axis;
        }
    }
}