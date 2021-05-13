using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Columns
{
    public class StaticColumn : Column
    {
        public StaticColumn()
        {
        }

        public override List<Column> GenerateColumns(List<Point3d> nodes, Plane plane)
        {
            // TODO: Create columns here using trusses!
            var axisA = ConstructAxis(nodes[0], plane);
            var axisB = ConstructAxis(nodes[2], plane);
            var columnA = new StaticColumn();
            columnA.Axis = axisA;
            var columnB = new StaticColumn();
            columnB.Axis = axisB;
            var columns = new List<Column> {columnA, columnB};
            return columns;
        }
    }
}