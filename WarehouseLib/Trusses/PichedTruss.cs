using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using WarehouseLib.Beams;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public class PichedTruss : Truss
    {
        private TrussInputs _inputs;

        protected PichedTruss(Plane plane, TrussInputs inputs) : base(plane, inputs)
        {
            _inputs = inputs;
        }

        protected override void GenerateThickBottomBars()
        {
            var bars = new List<Curve>();
            var ptA = new Point3d();
            var ptB = new Point3d();
            for (var i = 0; i < StartingPoints.Count; i++)
            {
                if (i >= StartingPoints.Count - 1) continue;
                ptA = StartingPoints[i] - Vector3d.ZAxis * ComputeDifference();
                ptB = StartingPoints[i + 1] - Vector3d.ZAxis * ComputeDifference();
                var tempLine = new Line(ptA, ptB);
                bars.Add(tempLine.ToNurbsCurve());
            }


            if (_inputs._articulationType == "Articulated" && _inputs.BaseType == 0)
            {
                bars = ComputeBottomBarsArticulatedToColumns(bars);
            }

            BottomBeamBaseCurves = new List<Curve>(bars);
        }

        protected override List<Curve> ComputeBottomBarsArticulatedToColumns(List<Curve> bars)
        {
            var startingPoint = StartingPoints[0] - Vector3d.ZAxis * ComputeDifference();
            var tempParams = TopBeamBaseCurves[0].DivideByCount(_divisions, true);
            var t1 = tempParams[1];
            var tempPt = TopBeamBaseCurves[0].PointAt(t1);
            var tempPlane = new Plane(tempPt, _plane.XAxis);
            var interPt = new Point3d();
            var intersectionEvents = Intersection.CurvePlane(bars[0], tempPlane, 0.01);
            if (intersectionEvents != null)
            {
                for (int i = 0; i < intersectionEvents.Count; i++)
                {
                    var intEv = intersectionEvents[0];
                    interPt = intEv.PointA;
                }
            }

            bars = new List<Curve>();
            double difference = interPt.Z - startingPoint.Z;
            for (var i = 0; i < StartingPoints.Count; i++)
            {
                if (i >= StartingPoints.Count - 1) continue;
                var ptA = new Point3d(StartingPoints[i] -
                                      (Vector3d.ZAxis * difference + Vector3d.ZAxis * ComputeDifference()));
                var ptB = new Point3d(StartingPoints[i + 1] -
                                      (Vector3d.ZAxis * difference + Vector3d.ZAxis * ComputeDifference()));
                var tempLine = new Line(ptA, ptB);
                bars.Add(tempLine.ToNurbsCurve());
            }

            return bars;
        }

        public override List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index)
        {
            var normals = new List<Vector3d>();
            var lines = new List<Curve>();
            foreach (var t in points)
            {
                var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
                var perpendicularVector = Vector3d.CrossProduct(vectorA, _plane.ZAxis);
                perpendicularVector.Unitize();
                var normal = Vector3d.CrossProduct(vectorA, perpendicularVector);
                normals.Add(normal);
                var line = new Line(t, normal * 100);
                lines.Add(line.ToNurbsCurve());
            }

            BottomBeamBaseCurves.AddRange(lines);
            return normals;
        }
        

        protected override void IsArticulatedToColumns()
        {
            var splitCurves = new List<Curve>();
            for (var i = 0; i < BottomBeamBaseCurves.Count; i++)
            {
                var bar = BottomBeamBaseCurves[i];
                var ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
                double t;
                bar.ClosestPoint(ptA, out t);
                splitCurves.Add(bar.Split(t)[i == 0 ? 1 : 0]);
            }

            BottomNodes.RemoveAt(0);
            BottomNodes.RemoveAt(BottomNodes.Count - 1);
            BottomNodes.Insert(0, TopNodes[0]);
            BottomNodes.Add(TopNodes[TopNodes.Count - 1]);
            var tempList = new List<Curve> {IntermediateBeamsBaseCurves[0], splitCurves[0]};
            var axisA = Curve.JoinCurves(tempList)[0];
            tempList = new List<Curve>
                {IntermediateBeamsBaseCurves[IntermediateBeamsBaseCurves.Count - 1], splitCurves[1]};
            var axisB = Curve.JoinCurves(tempList)[0];
            splitCurves = new List<Curve> {axisA, axisB};
            IntermediateBeamsBaseCurves.RemoveAt(0);
            IntermediateBeamsBaseCurves.RemoveAt(IntermediateBeamsBaseCurves.Count - 1);
            BottomBeamBaseCurves = splitCurves;
        }

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }
    }
}