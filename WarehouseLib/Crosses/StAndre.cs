using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Crosses
{
    public class StAndre : Cross
    {
        public StAndre()
        {
        }

        public override List<Cross> ConstructCrosses(List<Point3d> outerTopNodes, List<Point3d> innerBottomNodes,
            List<Point3d> outerBottomNodes, List<Point3d> innerTopNodes)
        {
            var crosses = new List<Cross>();
            for (int i = 0; i < outerTopNodes.Count; i++)
            {
                var cross = new StAndre
                {
                    Axis = ConstructAxis(outerTopNodes[i], innerBottomNodes[i], outerBottomNodes[i], innerTopNodes[i])
                };
                crosses.Add(cross);
            }

            return crosses;
        }

        public override List<Point3d> ComputeCrossTopNodes(Truss truss, int count)
        {
            var topNodes = new List<Point3d>(truss.TopNodes);

            return topNodes;
        }

        public override List<Point3d> ComputeCrossBottomNodes(Truss truss)
        {
            var bottomNodes = new List<Point3d>(truss.BottomNodes);

            return bottomNodes;
        }

        private List<Line> ConstructAxis(Point3d outerTopNode, Point3d innerBottomNode,
            Point3d outerBottomNode, Point3d innerTopNode)
        {
            var cross = new List<Line>();

            var axisA = new Line(outerTopNode, innerBottomNode);
            var axisB = new Line(outerBottomNode, innerTopNode);
            cross = new List<Line> {axisA, axisB};


            return cross;
        }
    }
}