﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using WarehouseLib.Beams;
using WarehouseLib.Connections;
using WarehouseLib.Trusses;

namespace WarehouseLib.Crosses
{
    public class StAndre : Cross
    {
        public StAndre()
        {
        }

        public override List<Cross> ConstructCrossesBetweenTwoTrusses(List<Point3d> outerTopNodes,
            List<Point3d> innerBottomNodes,
            List<Point3d> outerBottomNodes, List<Point3d> innerTopNodes)
        {
            var crosses = new List<Cross>();

            for (int i = 0; i < outerTopNodes.Count; i++)
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

        private List<Point3d> GetTopNodesFromStuds(Truss truss)
        {
            var intermediateBeams = truss.IntermediateBeams;
            var topStudsNodes = new List<Point3d>();
            for (int i = 0; i < intermediateBeams.SkeletonAxis.Count; i++)
            {
                var beam = intermediateBeams.SkeletonAxis[i];
                var angle = Vector3d.VectorAngle(beam.AxisCurve.PointAtEnd - beam.AxisCurve.PointAtStart, Vector3d.ZAxis);
                if (angle == Math.PI || angle == 0)
                {
                    // Debug.WriteLine("its a stud");
                    topStudsNodes.Add(beam.AxisCurve.PointAtStart);
                }
            }

            return topStudsNodes;
        }

        private List<Point3d> GetBottomNodesFromStuds(Truss truss)
        {
            var intermediateBeams = truss.IntermediateBeams;
            var bottomStudsNodes = new List<Point3d>();
            for (int i = 0; i < intermediateBeams.SkeletonAxis.Count; i++)
            {
                var beam = intermediateBeams.SkeletonAxis[i];
                var angle = Vector3d.VectorAngle(beam.AxisCurve.PointAtEnd - beam.AxisCurve.PointAtStart, Vector3d.ZAxis);
                if (angle == Math.PI || angle == 0)
                {
                    // Debug.WriteLine("its a stud");
                    bottomStudsNodes.Add(beam.AxisCurve.PointAtEnd);
                }
            }

            return bottomStudsNodes;
        }

        public override List<Point3d> ComputeCrossTopNodes(Truss truss, int count)
        {
            count += 1;
            var topNodes = new List<Point3d>();
            var tempList = GetTopNodesFromStuds(truss);

            var tempCloud = new PointCloud(tempList);
            var topBeam = Curve.JoinCurves(truss.TopBeamBaseCurves)[0];
            var parameters = topBeam.DivideByCount(count, false);
            foreach (var t in parameters)
            {
                var tempPt = topBeam.PointAt(t);
                var index = tempCloud.ClosestPoint(tempPt);
                if (!topNodes.Contains(tempList[index]))
                {
                    topNodes.Add(tempList[index]);
                }
            }

            // topNodes = new List<Point3d>(tempList);
            return topNodes;
        }

        public override List<Point3d> ComputeCrossBottomNodes(Truss truss, List<Point3d> topNodes)
        {
            var bottomNodes = new List<Point3d>();
            var tempTopNodes = GetTopNodesFromStuds(truss);
            var tempBottomNodes = GetBottomNodesFromStuds(truss);

            for (int i = 0; i < tempBottomNodes.Count; i++)
            {
                var topNode = tempBottomNodes[i];
                for (int j = 0; j < topNodes.Count; j++)
                {
                    var bottomNode = topNodes[j];

                    if (Math.Abs(topNode.X - bottomNode.X) <= 0.0001 && Math.Abs(topNode.Y - bottomNode.Y) <= 0.0001 &&
                        topNode.IsValid && bottomNode.IsValid)
                    {
                        var index = j;
                        bottomNodes.Add(tempBottomNodes[i]);
                    }
                }
            }

            SetCrossesBottomNodesToTruss(truss, bottomNodes);
            return bottomNodes;
        }

        private void SetCrossesBottomNodesToTruss(Truss truss, List<Point3d> bottomNodes)
        {
            var tempNodes = new List<Point3d>(bottomNodes);
            tempNodes.Insert(0, truss.BottomNodes[0]);
            tempNodes.Add(truss.BottomNodes[truss.BottomNodes.Count - 1]);
            truss.StAndresBottomNodes = new List<Point3d>(tempNodes);
            var tempCloud = new PointCloud(truss.BottomNodes);
            truss.StAndresBottomNodesIndices = new List<int>();
            for (var i=0;i<tempNodes.Count;i++)
            {
                var index = tempCloud.ClosestPoint(tempNodes[i]);
                truss.StAndresBottomNodesIndices.Add(index>0?index-1:index);
            }
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