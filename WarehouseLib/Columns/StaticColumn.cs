using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class StaticColumn : Column
    {
        public StaticColumn(Line axis) : base(axis)
        {
        }

        public List<Column> GenerateStaticColumns(List<Point3d> startingNodes, Plane plane)
        {
            // TODO: Create columns here using trusses!
            var axisA = ConstructColumn(startingNodes[0], plane);
            var axisB = ConstructColumn(startingNodes[2], plane);
            var staticColumns = new List<Column> {axisA, axisB};
            return staticColumns;
        }
    }
}