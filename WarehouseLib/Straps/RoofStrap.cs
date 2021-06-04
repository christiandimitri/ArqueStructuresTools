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
            if (truss is FlatTruss || truss is MonopichTruss || truss is ArchTruss)
            {
                for (var i = 0; i < trusses.Count - 1; i++)
                {
                    var trussA = trusses[i];
                    var trussB = trusses[i + 1];
                    for (var j = 0; j < trussA.TopNodes.Count; j++)
                    {
                        var nodeA = trussA.TopNodes[j];
                        var nodeB = trussB.TopNodes[j];
                        var strap = ConstructStrap(trussA, nodeA, nodeB);
                        roofStraps.Add(strap);
                    }
                }
            }
            else
            {
                roofStraps = new List<Strap>();
                var referencePoint = truss.TopBeamAxisCurves[0].PointAtEnd;
                var tempCloud = new PointCloud(truss.TopNodes);
                var index = tempCloud.ClosestPoint(referencePoint);
                for (var i = 0; i < trusses.Count - 1; i++)
                {
                    var trussA = trusses[i];
                    var trussB = trusses[i + 1];

                    var offset = 0.95;
                    var newNodesA =
                        new List<Point3d>(new RoofStrap().ModifyNodesAtIndex(trussA, index, offset));
                    var newNodesB =
                        new List<Point3d>(new RoofStrap().ModifyNodesAtIndex(trussB, index, offset));

                    for (var j = 0; j <= index; j++)
                    {
                        var nodeA = newNodesA[j];
                        var nodeB = newNodesB[j];
                        var strap = ConstructStrap(trussA, nodeA, nodeB);
                        roofStraps.Add(strap);
                    }

                    for (var j = index + 1; j < newNodesA.Count; j++)
                    {
                        var nodeA = newNodesA[j];
                        var nodeB = newNodesB[j];
                        var strap = ConstructStrap(trussB, nodeB, nodeA);
                        roofStraps.Add(strap);
                    }
                }
            }

            return roofStraps;
        }

        private List<Point3d> ModifyNodesAtIndex(Truss truss, int index, double offset)
        {
            var outNodes = truss.TopNodes;

            var ptA = truss.TopBeamAxisCurves[0].PointAtNormalizedLength(offset);
            var ptB = truss.TopBeamAxisCurves[1].PointAtNormalizedLength(1 - offset);
            outNodes.Insert(index, ptA);
            outNodes.RemoveAt(index + 1);
            outNodes.Insert(index + 1, ptB);
            return outNodes;
        }

        private RoofStrap ConstructStrap(Truss truss, Point3d nodeA, Point3d nodeB)
        {
            var ptA = nodeA;
            var ptB = nodeB;
            var axis = new Line(ptA, ptB);
            var strap = new RoofStrap
            {
                Axis = axis,
                ProfileOrientationPlane = GetTeklaProfileOrientationPlane(truss, ptA, 0, false)
            };
            return strap;
        }

        protected override Plane GetTeklaProfileOrientationPlane(Truss truss, Point3d strapPosition, int index,
            bool isBoundary)
        {
            var beam = Curve.JoinCurves(truss.TopBeam.Axis)[0];
            double t;
            beam.ClosestPoint(strapPosition, out t);
            var tangent = beam.TangentAt(t);
            var strapVector = truss._plane.YAxis;
            var normal = Vector3d.CrossProduct(tangent, strapVector);
            var profilePlane = new Plane(strapPosition, normal);
            return profilePlane;
        }
    }
}