using System;
using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public class PichedTruss : Truss
    {
        private TrussOptions _options;
        protected PichedTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            _options = options;
        }

        protected override void GenerateThickBottomBars()
        {
            var bars = new List<Curve>();
            for (var i = 0; i < StartingNodes.Count; i++)
            {
                if (i >= StartingNodes.Count - 1) continue;
                var ptA = StartingNodes[i] - Vector3d.ZAxis * ComputeDifference();
                var ptB = StartingNodes[i + 1] - Vector3d.ZAxis * ComputeDifference();
                var tempLine = new Line(ptA, ptB);
                bars.Add(tempLine.ToNurbsCurve());
            }

            BottomBars = bars;
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            throw new NotImplementedException();
        }

        public override void ConstructTruss(int divisions)
        {
            var recomputedDivisions = RecomputeDivisions(divisions);
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            // var angle = Vector3d.VectorAngle(TopBars[0].TangentAtStart, Plane.XAxis);
            for (var i = 0; i < TopBars.Count; i++)
            {
                GenerateTopNodes(TopBars[i], recomputedDivisions, i);
                GenerateBottomNodes(BottomBars[i]);
            }
            var cloud = new PointCloud(TopNodes);
            var index = cloud.ClosestPoint(StartingNodes[1]);
            GenerateIntermediateBars(TrussType, index);
        }

        public override List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index)
        {
            var normals = new List<Vector3d>();
            var lines = new List<Curve>();
            foreach (var t in points)
            {
                var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
                var perpendicularVector = Vector3d.CrossProduct(vectorA, Plane.ZAxis);
                perpendicularVector.Unitize();
                var normal = Vector3d.CrossProduct(vectorA, perpendicularVector);
                normals.Add(normal);
                var line = new Line(t, normal * 100);
                lines.Add(line.ToNurbsCurve());
            }

            BottomBars.AddRange(lines);
            return normals;
        }

        protected override void IsArticulatedToColumns()
        {
            var splitCurves = new List<Curve>();
            for (var i = 0; i < BottomBars.Count; i++)
            {
                var bar = BottomBars[i];
                var ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
                double t;
                bar.ClosestPoint(ptA, out t);
                splitCurves.Add(bar.Split(t)[i == 0 ? 1 : 0]);
            }

            BottomNodes.RemoveAt(0);
            BottomNodes.RemoveAt(BottomNodes.Count - 1);
            BottomBars = splitCurves;
        }

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }
    }
}