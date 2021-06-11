using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Connections;
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
            for (int i = 1; i < outerTopNodes.Count - 1; i++)
            {
                var cross = new StAndre
                {
                    Axis = ConstructAxis(outerTopNodes[i], innerBottomNodes[i], outerBottomNodes[i], innerTopNodes[i])
                };
                cross.AddTeklaProfileOrientationPlane(cross);
                crosses.Add(cross);
            }

            return crosses;
        }

        public override List<Point3d> ComputeCrossTopNodes(Truss truss, int count)
        {
            count += 1;
            var topNodes = new List<Point3d>();
            var tempList = new List<Point3d>();
            if (truss._connectionType == ConnectionType.WarrenStuds.ToString())
            {
                for (var i = 1; i < truss.TopNodes.Count; i += 2)
                {
                    tempList.Add(truss.TopNodes[i]);
                }
            }
            else
            {
                tempList = truss.TopNodes;
            }

            var tempCloud = new PointCloud(tempList);
            var topBeam = Curve.JoinCurves(truss.TopBeamAxisCurves)[0];
            var parameters = topBeam.DivideByCount(count, true);
            foreach (var t in parameters)
            {
                var tempPt = topBeam.PointAt(t);
                var index = tempCloud.ClosestPoint(tempPt);
                topNodes.Add(tempList[index]);
            }


            return topNodes;
        }

        public override List<Point3d> ComputeCrossBottomNodes(Truss truss, List<Point3d> topNodes)
        {
            var bottomNodes = new List<Point3d>();
            var tempCloud = new PointCloud(truss.BottomNodes);
            foreach (var node in topNodes)
            {
                var index = tempCloud.ClosestPoint(node);
                bottomNodes.Add(truss.BottomNodes[index]);
            }

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

        public override void AddTeklaProfileOrientationPlane(StAndre cross)
        {
            cross.ProfileOrientationPlanes = new List<Plane>();
            for (int i = 0; i < cross.Axis.Count; i++)
            {
                var normal = Vector3d.CrossProduct(Axis[i].Direction, Vector3d.ZAxis);
                var plane = new Plane(cross.Axis[i].PointAt(0), (i == 0) ? -Vector3d.ZAxis : Vector3d.ZAxis);
                cross.ProfileOrientationPlanes.Add(plane);
            }
        }
    }
}