﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry.Intersect;
using WarehouseLib.Columns;
using WarehouseLib.Connections;
using WarehouseLib.Options;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class ArchTruss : CurvedTruss
    {
        private TrussInputs _inputs;

        public ArchTruss(Plane plane, TrussInputs inputs) : base(plane, inputs)
        {
            _inputs = inputs;
            GenerateTopBars();
            StaticColumns = new List<Column>(new StaticColumn().GenerateColumns(StartingPoints, plane));
            ChangeBaseByType(_inputs.BaseType);
            ConstructTruss(inputs.Divisions);
            ChangeArticulationAtColumnsByType(inputs._articulationType);
            ConstructBeams(true, true);
        }

        public override List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index)
        {
            throw new NotImplementedException();
        }

        protected override void IsArticulatedToColumns()
        {
            var ptA = new Point3d();
            List<Curve> splitCurves = new List<Curve>();
            for (int i = 0; i < BottomBeamBaseCurves.Count; i++)
            {
                var bar = BottomBeamBaseCurves[i];
                ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
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

        protected override void GenerateThickBottomBars()
        {
            if (_height == _maxHeight)
            {
                var lineA = new Line(StartingPoints[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingPoints[1] - Vector3d.ZAxis * ComputeDifference());
                var lineB = new Line(StartingPoints[1] - Vector3d.ZAxis * ComputeDifference(),
                    StartingPoints[2] - Vector3d.ZAxis * ComputeDifference());
                BottomBeamBaseCurves = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                List<Curve> finalList;

                if (_inputs._articulationType == "Articulated" && _inputs.BaseType == 0)
                {
                    finalList = ComputeBottomBarsArticulatedToColumns(null);
                }
                else
                {
                    finalList = ComputeRigidBottomBarsToColumns();
                }

                BottomBeamBaseCurves = finalList;
            }
        }

        private List<Curve> ComputeRigidBottomBarsToColumns()
        {
            var startPoint = StartingPoints[0] - ComputeNormalAtStartEnd(0) * ComputeOffsetFromDot(0);
            var centerPoint = StartingPoints[1] - Vector3d.ZAxis * ComputeOffsetFromTrigo(0);
            var endPoint = StartingPoints[2] - ComputeNormalAtStartEnd(1) * ComputeOffsetFromDot(1);
            var arch = new Arc(startPoint, centerPoint, endPoint);
            arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
            Curve[] tempCurves = arch.ToNurbsCurve().Split(t);
            Line lineA = new Line(StartingPoints[0] - Vector3d.ZAxis * ComputeDifference(),
                StartingPoints[0] - ComputeNormalAtStartEnd(0) * ComputeOffsetFromDot(0));
            Line lineB = new Line(StartingPoints[2] - ComputeNormalAtStartEnd(1) * ComputeOffsetFromDot(1),
                StartingPoints[2] - Vector3d.ZAxis * ComputeDifference()
            );
            List<Curve> leftCurves = new List<Curve> {lineA.ToNurbsCurve(), tempCurves[0]};
            List<Curve> rightCurves = new List<Curve> {tempCurves[1], lineB.ToNurbsCurve()};
            Curve[] joinedRight = Curve.JoinCurves(rightCurves, 0.001);
            Curve[] joinedLeft = Curve.JoinCurves(leftCurves, 0.001);
            leftCurves.AddRange(rightCurves);
            var finalList = new List<Curve> {joinedLeft.ToList()[0], joinedRight.ToList()[0]};
            // tempCrvs[1].Reverse();
            return finalList;
        }

        protected override List<Curve> ComputeBottomBarsArticulatedToColumns(List<Curve> bars)
        {
            var tempParamsA = TopBeamBaseCurves[0].DivideByCount(_divisions, true);
            var tempParamsB = TopBeamBaseCurves[1].DivideByCount(_divisions, true);
            var t1 = tempParamsA[1];
            var t2 = tempParamsB[0];
            var t3 = tempParamsB[tempParamsB.Length - 2];
            var startPoint = TopBeamBaseCurves[0].PointAt(t1) -
                             Vector3d.ZAxis * (ComputeDifference() +
                                               (TopBeamBaseCurves[0].PointAt(t1).Z - StartingPoints[0].Z));
            var centerPoint = TopBeamBaseCurves[1].PointAt(t2) -
                              Vector3d.ZAxis * startPoint.DistanceTo(StartingPoints[0]);
            var endPoint = TopBeamBaseCurves[1].PointAt(t3) -
                           Vector3d.ZAxis * (ComputeDifference() +
                                             (TopBeamBaseCurves[0].PointAt(t1).Z - StartingPoints[0].Z));
            var arch = new Arc(startPoint, centerPoint, endPoint);
            arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
            Curve[] tempCurves = arch.ToNurbsCurve().Split(t);
            Line lineA = new Line(StartingPoints[0] - Vector3d.ZAxis * ComputeDifference(), startPoint);
            Line lineB = new Line(endPoint, StartingPoints[2] - Vector3d.ZAxis * ComputeDifference());
            List<Curve> leftCurves = new List<Curve> {lineA.ToNurbsCurve(), tempCurves[0]};
            List<Curve> rightCurves = new List<Curve> {tempCurves[1], lineB.ToNurbsCurve()};
            Curve[] joinedRight = Curve.JoinCurves(rightCurves, 0.001);
            Curve[] joinedLeft = Curve.JoinCurves(leftCurves, 0.001);
            leftCurves.AddRange(rightCurves);
            var finalList = new List<Curve> {joinedLeft.ToList()[0], joinedRight.ToList()[0]};
            return finalList;
        }

        public override void GenerateTopBars()
        {
            StartingPoints = GetStartingPoints(_plane, _length / 2, _length / 2, _height,
                _maxHeight, _height);

            if (_height == _maxHeight)
            {
                Line lineA = new Line(StartingPoints[0], StartingPoints[1]);
                Line lineB = new Line(StartingPoints[1], StartingPoints[2]);
                TopBeamBaseCurves = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                Arc arch = new Arc(StartingPoints[0], StartingPoints[1], StartingPoints[2]);
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                // tempCrvs[1].Reverse();
                TopBeamBaseCurves = tempCrvs.ToList();
            }
        }

        public override double ComputeArticulatedOffsetFromTrigo(int index, double difference)
        {
            throw new NotImplementedException();
        }
    }
}