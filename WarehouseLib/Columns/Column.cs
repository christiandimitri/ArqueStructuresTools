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
        public List<Column> GenerateStaticColumns(List<Point3d> startingNodes, Plane plane)
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            var axisA = new Line(new Point3d(startingNodes[0].X, startingNodes[0].Y, plane.Origin.Z),
                startingNodes[0]);

            var axisB = new Line(new Point3d(startingNodes[2].X, startingNodes[2].Y, plane.Origin.Z),
                startingNodes[2]);

            columns.Add(new Column(axisA));
            columns.Add(new Column(axisB));
            var staticColumns = new List<Column>(columns);
            return staticColumns;
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