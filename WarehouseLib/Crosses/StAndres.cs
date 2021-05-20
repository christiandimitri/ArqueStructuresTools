using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib.Crosses
{
    public class StAndres : Cross
    {
        public StAndres()
        {
        }

        public override Cross ConstructCross(List<Point3d> outsideNodes, List<Point3d> insideNodes)
        {
            var cross = new StAndres();

            cross.Axis = ConstructAxis(outsideNodes, insideNodes);
            return cross;
        }

        private List<Line> ConstructAxis(List<Point3d> outsideNodes, List<Point3d> insideNodes)
        {
            var cross = new List<Line>();

            var axisA = new Line(outsideNodes[0], insideNodes[1]);
            var axisB = new Line(outsideNodes[1], insideNodes[0]);
            cross = new List<Line> {axisA, axisB};

            return cross;
        }
    }
}