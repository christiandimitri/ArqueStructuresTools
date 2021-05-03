using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public class ArchTruss : CurvedTruss
    {
        private int BaseType;

        public ArchTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions,
            string trussType, string articulationType, int baseType, int columnsCount) : base(plane, length, height,
            maxHeight,
            clearHeight, divisions,
            trussType, articulationType, columnsCount)
        {
            BaseType = baseType;
            GenerateTopBars();
            GenerateStaticColumns();
            ChangeBaseByType(baseType);
            ConstructTruss(divisions);
            ChangeArticulationAtColumnsByType(articulationType);
        }

        protected override void IsArticulatedToColumns()
        {
            Point3d ptA = new Point3d();
            List<Curve> splitCrvs = new List<Curve>();
            for (int i = 0; i < BottomBars.Count; i++)
            {
                var bar = BottomBars[i];
                ptA = BottomNodes[i == 0 ? 1 : BottomNodes.Count - 2];
                double t;
                bar.ClosestPoint(ptA, out t);
                splitCrvs.Add(bar.Split(t)[i == 0 ? 1 : 0]);
            }

            BottomNodes.RemoveAt(0);
            BottomNodes.RemoveAt(BottomNodes.Count - 1);
            BottomBars = splitCrvs;
        }

        protected override void GenerateThickBottomBars()
        {
            if (Height == MaxHeight)
            {
                Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[1] - Vector3d.ZAxis * ComputeDifference());
                Line lineB = new Line(StartingNodes[1] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[2] - Vector3d.ZAxis * ComputeDifference());
                BottomBars = new List<Curve> {lineA.ToNurbsCurve(), lineB.ToNurbsCurve()};
            }
            else
            {
                Arc arch = new Arc(StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0),
                    StartingNodes[1] - Vector3d.ZAxis * ComputeOffsetFromTrigo(0),
                    StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1));
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                Line lineA = new Line(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[0] - ComputeNormal(0) * ComputeOffsetFromDot(0));
                Line lineB = new Line(StartingNodes[2] - ComputeNormal(1) * ComputeOffsetFromDot(1),
                    StartingNodes[2] - Vector3d.ZAxis * ComputeDifference()
                );
                List<Curve> leftCrvs = new List<Curve> {lineA.ToNurbsCurve(), tempCrvs[0]};
                List<Curve> rightCrvs = new List<Curve> {tempCrvs[1], lineB.ToNurbsCurve()};
                Curve[] joinedRight = Curve.JoinCurves(rightCrvs, 0.001);
                Curve[] joinedLeft = Curve.JoinCurves(leftCrvs, 0.001);
                leftCrvs.AddRange(rightCrvs);
                List<Curve> finalList = new List<Curve> {joinedLeft.ToList()[0], joinedRight.ToList()[0]};
                // tempCrvs[1].Reverse();
                BottomBars = finalList;
            }
        }

        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length / 2, Length / 2, Height, MaxHeight, Height);

            if (Height == MaxHeight)
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
            GenerateIntermediateBars(TrussType, index);
        }
    }
}