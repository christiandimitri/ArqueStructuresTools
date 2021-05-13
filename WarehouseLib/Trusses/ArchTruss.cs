using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry.Intersect;
using WarehouseLib.Columns;
using WarehouseLib.Options;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class ArchTruss : CurvedTruss
    {
        private TrussOptions _options;

        public ArchTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
            _options = options;
            GenerateTopBars();
            StaticColumns = new List<Column>(new StaticColumn().GenerateColumns(StartingNodes, plane));
            ChangeBaseByType(_options.BaseType);
            ConstructTruss(options.Divisions);
            ChangeArticulationAtColumnsByType(options.ArticulationType);
        }

        protected override void IsArticulatedToColumns()
        {
            var ptA = new Point3d();
            List<Curve> splitCurves = new List<Curve>();
            for (int i = 0; i < BottomBars.Count; i++)
            {
                var bar = BottomBars[i];
                ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
                double t;
                bar.ClosestPoint(ptA, out t);
                splitCurves.Add(bar.Split(t)[i == 0 ? 1 : 0]);
            }

            BottomNodes.RemoveAt(0);
            BottomNodes.RemoveAt(BottomNodes.Count - 1);
            BottomBars = splitCurves;
        }

        protected override void GenerateThickBottomBars()
        {
            if (_height == _maxHeight)
            {
                var lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[1] - Vector3d.ZAxis * ComputeDifference());
                var lineB = new Line(StartingNodes[1] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[2] - Vector3d.ZAxis * ComputeDifference());
                BottomBars = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                List<Curve> finalList;

                if (_options.ArticulationType == "Articulated" && _options.BaseType == 0)
                {
                    finalList = ComputeBottomBarsArticulatedToColumns(null);
                }
                else
                {
                    var startPoint = StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0);
                    var centerPoint = StartingNodes[1] - Vector3d.ZAxis * ComputeOffsetFromTrigo(0);
                    var endPoint = StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1);
                    var arch = new Arc(startPoint, centerPoint, endPoint);
                    arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                    Curve[] tempCurves = arch.ToNurbsCurve().Split(t);
                    Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                        StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0));
                    Line lineB = new Line(StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1),
                        StartingNodes[2] - Vector3d.ZAxis * ComputeDifference()
                    );
                    List<Curve> leftCurves = new List<Curve> {lineA.ToNurbsCurve(), tempCurves[0]};
                    List<Curve> rightCurves = new List<Curve> {tempCurves[1], lineB.ToNurbsCurve()};
                    Curve[] joinedRight = Curve.JoinCurves(rightCurves, 0.001);
                    Curve[] joinedLeft = Curve.JoinCurves(leftCurves, 0.001);
                    leftCurves.AddRange(rightCurves);
                    finalList = new List<Curve> {joinedLeft.ToList()[0], joinedRight.ToList()[0]};
                    // tempCrvs[1].Reverse();
                }

                BottomBars = finalList;
            }
        }

        protected override List<Curve> ComputeBottomBarsArticulatedToColumns(List<Curve> bars)
        {
            var finalList = new List<Curve>();
            var startPoint = StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0);
            var centerPoint = StartingNodes[1] - Vector3d.ZAxis * ComputeOffsetFromTrigo(0);
            var endPoint = StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1);
            // Arc arch = new Arc(startPoint,centerPoint,endPoint);
            // arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
            // Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
            // Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
            //     StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0, true));
            // Line lineB = new Line(StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1, true),
            //     StartingNodes[2] - Vector3d.ZAxis * ComputeDifference()
            // );
            // List<Curve> leftCrvs = new List<Curve> {lineA.ToNurbsCurve(), tempCrvs[0]};
            // List<Curve> rightCrvs = new List<Curve> {tempCrvs[1], lineB.ToNurbsCurve()};
            // Curve[] joinedRight = Curve.JoinCurves(rightCrvs, 0.001);
            // Curve[] joinedLeft = Curve.JoinCurves(leftCrvs, 0.001);
            // leftCrvs.AddRange(rightCrvs);
            // finalList = new List<Curve> {joinedLeft.ToList()[0], joinedRight.ToList()[0]};
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
                TopBars = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                Arc arch = new Arc(StartingNodes[0], StartingNodes[1], StartingNodes[2]);
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                // tempCrvs[1].Reverse();
                TopBars = tempCrvs.ToList();
            }
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            GenerateVerticalBottomNodes(crv);
        }

        public override void ConstructTruss(int divisions)
        {
            int recomputedDivisions = RecomputeDivisions(divisions);
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            IntermediateBars = new List<Curve>();
            for (int i = 0; i < TopBars.Count; i++)
            {
                GenerateTopNodes(TopBars[i], recomputedDivisions, i);
                GenerateBottomNodes(BottomBars[i]);
            }

            PointCloud cloud = new PointCloud(TopNodes);
            int index = cloud.ClosestPoint(StartingNodes[1]);
            GenerateIntermediateBars(_trussType, index);
        }
    }
}