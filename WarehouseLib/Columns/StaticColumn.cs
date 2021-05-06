using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class StaticColumn : Column
    {
        public Line Axis;
        public StaticColumn(Line axis)
        {
            Axis = axis;
        }

        public List<Column> GenerateStaticColumns(List<Point3d> startingNodes, Plane plane)
        {
            var columns = new List<Column>();

            // TODO: Create columns here using trusses!
            var axisA = new Line(new Point3d(startingNodes[0].X, startingNodes[0].Y, plane.Origin.Z),
                startingNodes[0]);

            var axisB = new Line(new Point3d(startingNodes[2].X, startingNodes[2].Y, plane.Origin.Z),
                startingNodes[2]);

            columns.Add(new StaticColumn(axisA));
            columns.Add(new StaticColumn(axisB));
            var staticColumns = new List<Column>(columns);
            return staticColumns;
        }
    }
}