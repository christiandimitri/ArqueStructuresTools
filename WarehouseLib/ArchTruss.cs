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
        public ArchTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions,
            string trussType) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType)
        {
            GenerateTopBars();
            GenerateColumns();
            GenerateBottomBars();
            ConstructTruss(divisions);
        }

        public override void GenerateBottomBars()
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
                Arc arch = new Arc(StartingNodes[0] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[1] - Vector3d.ZAxis * ComputeDifference(),
                    StartingNodes[2] - Vector3d.ZAxis * ComputeDifference());
                arch.ToNurbsCurve().LengthParameter(arch.ToNurbsCurve().GetLength() / 2, out double t);
                Curve[] tempCrvs = arch.ToNurbsCurve().Split(t);
                // tempCrvs[1].Reverse();
                BottomBars = tempCrvs.ToList();
            }
        }
        public override void GenerateTopBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length, Height, MaxHeight, Height);

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
        public override void ConstructTruss(int divisions)
        {
            int recomputedDivisions = RecomputeDivisions(divisions);
            TopNodes = new List<Point3d>();
            for (int j = 0; j < TopBars.Count; j++)
            {
                TopNodes.AddRange(GenerateTopNodes(TopBars[j], recomputedDivisions, j));
                GenerateBottomNodes(TopNodes, ComputeDifference());
            }

            PointCloud cloud = new PointCloud(TopNodes);
            int index = cloud.ClosestPoint(StartingNodes[1]);
            GenerateIntermediateBars(TrussType, index);
        }
    }
}