using System.Collections.Generic;
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

        public List<Column> GenerateBoundaryColumns(List<Point3d> nodes, Plane plane)
        {
            var boundaryColumns = new List<Column>();
            var positions = nodes;
            var columns = new List<Column>();
            for (int i = 0; i < positions.Count; i++)
            {
                var column = new Column(Line.Unset).ConstructColumn(positions[i], plane);
                columns.Add(column);
            }

            boundaryColumns.AddRange(columns);
            return boundaryColumns;
        }
    }
}