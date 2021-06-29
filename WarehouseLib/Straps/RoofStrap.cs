using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class RoofStrap : Strap
    {
        public RoofStrap()
        {
        }

        public List<Strap> ConstructRoofStraps(List<Truss> trusses)
        {
            var roofStraps = new List<Strap>();
            var truss = trusses[1];
            // if (truss is FlatTruss || truss is MonopichTruss || truss is ArchTruss)
            // {
                for (var i = 0; i < trusses.Count - 1; i++)
                {
                    var trussA = trusses[i];
                    var trussB = trusses[i + 1];
                    // var offset = 0.9;
                    var newNodesA = trussA.TopNodes;
                        // new List<Point3d>(
                        //     new RoofStrap().ModifyNodesAtStartEndAndAtIndexByOffset(trussA, 0, offset));
                        var newNodesB = trussB.TopNodes;
                        // new List<Point3d>(
                        //     new RoofStrap().ModifyNodesAtStartEndAndAtIndexByOffset(trussB, 0, offset));
                    for (var j = 0; j < newNodesA.Count; j++)
                    {
                        var nodeA = newNodesA[j];
                        var nodeB = newNodesB[j];
                        var strap = ConstructStrap(trussA, nodeA, nodeB, 0);
                        roofStraps.Add(strap);
                    }
                }
            // }
            // else
            // {
            //     roofStraps = new List<Strap>();
            //     var referencePoint = truss.TopBeamAxisCurves[0].PointAtEnd;
            //     var tempCloud = new PointCloud(truss.TopNodes);
            //     var index = tempCloud.ClosestPoint(referencePoint);
            //     for (var i = 0; i < trusses.Count - 1; i++)
            //     {
            //         var trussA = trusses[i];
            //         var trussB = trusses[i + 1];
            //
            //         var offset = 0.9;
            //         var newNodesA =
            //             new List<Point3d>(
            //                 new RoofStrap().ModifyNodesAtStartEndAndAtIndexByOffset(trussA, index, offset));
            //         var newNodesB =
            //             new List<Point3d>(
            //                 new RoofStrap().ModifyNodesAtStartEndAndAtIndexByOffset(trussB, index, offset));
            //
            //         for (var j = 0; j < newNodesA.Count; j++)
            //         {
            //             var nodeA = newNodesA[j];
            //             var nodeB = newNodesB[j];
            //             var strap = ConstructStrap(trussA, (j <= index) ? nodeA : nodeB, (j <= index) ? nodeB : nodeA,
            //                 0);
            //             roofStraps.Add(strap);
            //         }
            //     }
            // }

            return roofStraps;
        }

        private List<Point3d> ModifyNodesAtStartEndAndAtIndexByOffset(Truss truss, int index, double offset)
        {
            var outNodes = new List<Point3d>(truss.TopNodes);
            var pt = truss.TopBeamBaseCurves[0].PointAtNormalizedLength(1 - offset);
            outNodes.Insert(0, pt);
            outNodes.RemoveAt(1);
            if (truss is DoublepichTruss)
            {
                pt = new Point3d(truss.TopBeamBaseCurves[0].PointAtNormalizedLength(offset));
                outNodes.Insert(index, pt);
                outNodes.RemoveAt(index + 1);
                pt = new Point3d(truss.TopBeamBaseCurves[1].PointAtNormalizedLength(1 - offset));
                outNodes.Insert(index + 1, pt);
                // outNodes.RemoveAt(index + 2);
            }

            pt = new Point3d(truss.TopBeamBaseCurves[1].PointAtNormalizedLength(offset));
            outNodes.Insert(outNodes.Count - 1, pt);
            outNodes.RemoveAt(outNodes.Count - 1);
            return outNodes;
        }

        private RoofStrap ConstructStrap(Truss truss, Point3d nodeA, Point3d nodeB, int index)
        {
            var ptA = nodeA;
            var ptB = nodeB;
            var axis = new Line(ptA, ptB);
            var strap = new RoofStrap
            {
                Axis = axis,
                ProfileOrientationPlane = GetTeklaProfileOrientationPlane(truss, ptA, index, false)
            };
            return strap;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index,
            bool isBoundary)
        {
            var tempBeamAxis = new List<Curve>();
            for (int i = 0; i < truss.TopBeam.Axis.Count; i++)
            {
                tempBeamAxis.Add(truss.TopBeam.Axis[i].AxisCurve);
            }
            var beam = Curve.JoinCurves(tempBeamAxis)[0];
            double t;
            beam.ClosestPoint(strapPosition, out t);
            var tangent = beam.TangentAt(t);
            var strapVector = index == 0 ? truss._plane.YAxis : -truss._plane.YAxis;
            var normal = Vector3d.CrossProduct(tangent, strapVector);
            var profilePlane = new Plane(strapPosition, normal);
            return profilePlane;
        }
    }
}