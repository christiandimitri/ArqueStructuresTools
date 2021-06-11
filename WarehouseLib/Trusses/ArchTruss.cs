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
            StaticColumns = new List<Column>(new StaticColumn().GenerateColumns(StartingNodes, plane));
            ChangeBaseByType(_inputs.BaseType);
            ConstructTruss(inputs.Divisions);
            ChangeArticulationAtColumnsByType(inputs._articulationType);
            ConstructBeams(true, true);
        }

        protected override void IsArticulatedToColumns()
        {
            var ptA = new Point3d();
            List<Curve> splitCurves = new List<Curve>();
            for (int i = 0; i < BottomBeamAxisCurves.Count; i++)
            {
                var bar = BottomBeamAxisCurves[i];
                ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
                double t;
                bar.ClosestPoint(ptA, out t);
                splitCurves.Add(bar.Split(t)[i == 0 ? 1 : 0]);
            }

            BottomNodes.RemoveAt(0);
            BottomNodes.RemoveAt(BottomNodes.Count - 1);
            var tempList = new List<Curve> {IntermediateBeamsAxisCurves[0], splitCurves[0]};
            var axisA = Curve.JoinCurves(tempList)[0];
            tempList = new List<Curve>
                {IntermediateBeamsAxisCurves[IntermediateBeamsAxisCurves.Count - 1], splitCurves[1]};
            var axisB = Curve.JoinCurves(tempList)[0];
            splitCurves = new List<Curve> {axisA, axisB};
            IntermediateBeamsAxisCurves.RemoveAt(0);
            IntermediateBeamsAxisCurves.RemoveAt(IntermediateBeamsAxisCurves.Count - 1);
            BottomBeamAxisCurves = splitCurves;
        }

        protected override void GenerateThickBottomBars()
        {
            if (_height == _maxHeight)
            {
                var lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[1] - Vector3d.ZAxis * ComputeDifference());
                var lineB = new Line(StartingNodes[1] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[2] - Vector3d.ZAxis * ComputeDifference());
                BottomBeamAxisCurves = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
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

                BottomBeamAxisCurves = finalList;
            }
        }

        private List<Curve> ComputeRigidBottomBarsToColumns()
        {
            var startPoint = StartingNodes[0] - ComputeNormalAtStartEnd(0) * ComputeOffsetFromDot(0);
            var centerPoint = StartingNodes[1] - Vector3d.ZAxis * ComputeOffsetFromTrigo(0);
            var endPoint = StartingNodes[2] - ComputeNormalAtStartEnd(1) * ComputeOffsetFromDot(1);
            var arch = new Arc(startPoint, centerPoint, endPoint);
            arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
            Curve[] tempCurves = arch.ToNurbsCurve().Split(t);
            Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                StartingNodes[0] - ComputeNormalAtStartEnd(0) * ComputeOffsetFromDot(0));
            Line lineB = new Line(StartingNodes[2] - ComputeNormalAtStartEnd(1) * ComputeOffsetFromDot(1),
                StartingNodes[2] - Vector3d.ZAxis * ComputeDifference()
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
            var tempParamsA = TopBeamAxisCurves[0].DivideByCount(_divisions, true);
            var tempParamsB = TopBeamAxisCurves[1].DivideByCount(_divisions, true);
            var t1 = tempParamsA[1];
            var t2 = tempParamsB[0];
            var t3 = tempParamsB[tempParamsB.Length - 2];
            var startPoint = TopBeamAxisCurves[0].PointAt(t1) -
                             Vector3d.ZAxis * (ComputeDifference() +
                                               (TopBeamAxisCurves[0].PointAt(t1).Z - StartingNodes[0].Z));
            var centerPoint = TopBeamAxisCurves[1].PointAt(t2) -
                              Vector3d.ZAxis * startPoint.DistanceTo(StartingNodes[0]);
            var endPoint = TopBeamAxisCurves[1].PointAt(t3) -
                           Vector3d.ZAxis * (ComputeDifference() +
                                             (TopBeamAxisCurves[0].PointAt(t1).Z - StartingNodes[0].Z));
            var arch = new Arc(startPoint, centerPoint, endPoint);
            arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
            Curve[] tempCurves = arch.ToNurbsCurve().Split(t);
            Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(), startPoint);
            Line lineB = new Line(endPoint, StartingNodes[2] - Vector3d.ZAxis * ComputeDifference());
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
            StartingNodes = GetStartingPoints(_plane, _length / 2, _length / 2, _height,
                _maxHeight, _height);

            if (_height == _maxHeight)
            {
                Line lineA = new Line(StartingNodes[0], StartingNodes[1]);
                Line lineB = new Line(StartingNodes[1], StartingNodes[2]);
                TopBeamAxisCurves = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                Arc arch = new Arc(StartingNodes[0], StartingNodes[1], StartingNodes[2]);
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                // tempCrvs[1].Reverse();
                TopBeamAxisCurves = tempCrvs.ToList();
            }
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            GenerateVerticalBottomNodes(crv);
        }

        public override double ComputeArticulatedOffsetFromTrigo(int index, double difference)
        {
            return 0;
        }

        protected override void RecomputeNodes(int index)
        {
            List<Point3d> tempTopList = new List<Point3d>();
            List<Point3d> tempBottomList = new List<Point3d>();
            for (int i = 0; i < TopNodes.Count; i++)
            {
                if (_connectionType == ConnectionType.Warren.ToString())
                {
                    if (i % 2 == 0)
                    {
                        tempTopList.Add(TopNodes[i]);
                    }
                    else if (i % 2 == 1)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_connectionType == ConnectionType.WarrenStuds.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    if (i % 2 == 1 || i == TopNodes.Count - 1 || i == 0)
                    {
                        tempBottomList.Add(BottomNodes[i]);
                    }
                }
                else if (_connectionType == ConnectionType.Howe.ToString() || _connectionType == ConnectionType.Pratt.ToString())
                {
                    tempTopList.Add(TopNodes[i]);
                    tempBottomList.Add(BottomNodes[i]);
                }
            }

            if (_connectionType == ConnectionType.Warren.ToString())
            {
                tempBottomList.Insert(0, BottomNodes[0]);
                tempBottomList.Add(BottomNodes[BottomNodes.Count - 1]);
            }
            if (_connectionType == ConnectionType.WarrenStuds.ToString())
            {
                if (!tempBottomList.Contains(BottomNodes[index]))
                {
                    tempBottomList.Insert((index / 2) + 1, BottomNodes[index]);
                }
                else if (!tempTopList.Contains(TopNodes[index]))
                {
                    tempTopList.Insert((index / 2) + 1, TopNodes[index]);
                }
            }
            TopNodes = new List<Point3d>(tempTopList);
            BottomNodes = new List<Point3d>(tempBottomList);
            if (ConnectionType.Warren.ToString() == _connectionType)
            {
                IntermediateBeamsAxisCurves.RemoveAt(index);
            }
        }

        public override void ConstructTruss(int divisions)
        {
            divisions = _divisions;
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            IntermediateBeamsAxisCurves = new List<Curve>();
            for (int i = 0; i < TopBeamAxisCurves.Count; i++)
            {
                GenerateTopNodes(TopBeamAxisCurves[i], divisions, i);
                GenerateBottomNodes(BottomBeamAxisCurves[i]);
            }

            PointCloud cloud = new PointCloud(TopNodes);
            int index = cloud.ClosestPoint(StartingNodes[1]);
            GenerateIntermediateBars(_connectionType, index);
        }
    }
}