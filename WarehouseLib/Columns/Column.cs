using Rhino.Geometry;

namespace WarehouseLib
{
    public class Column
    {
        public Line Axis;

        public Column(Line axis)
        {
            Axis = axis;
        }
        public Column ConstructColumn(Point3d node, Plane plane)
        {
            var axis = new Line(plane.ClosestPoint(node), node);
            var column = new Column(axis);
            return column;
        }
    }
}