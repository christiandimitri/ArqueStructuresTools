using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public class PichedTruss : Truss
    {
        protected PichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
            int divisions, string trussType, string articulationType) : base(plane, length, height, maxHeight,
            clearHeight, divisions, trussType, articulationType)
        {
        }

        public override void GenerateBeams()
        {
            throw new NotImplementedException();
        }

        public override void GenerateBottomBars()
        {
            List<Curve> bars = new List<Curve>();
            for (int i = 0; i < StartingNodes.Count; i++)
            {
                if (i < StartingNodes.Count - 1)
                {
                    Point3d ptA = StartingNodes[i] - Vector3d.ZAxis * ComputeDifference();
                    Point3d ptB = StartingNodes[i + 1] - Vector3d.ZAxis * ComputeDifference();
                    Line tempLine = new Line(ptA, ptB);
                    bars.Add(tempLine.ToNurbsCurve());
                }
            }

            BottomBars = bars;
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

        public override void GenerateBottomNodes(List<Point3d> points, double difference)
        {
            List<Point3d> nodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            foreach (var pt in points)
            {
                Point3d tempPt = pt - (Vector3d.ZAxis * difference);
                nodes.Add(tempPt);
            }

            BottomNodes.AddRange(nodes);
        }

        public override void ComputeArticulationAtColumns(string type)
        {
            if (type == "Articulated")
            {
                IsArticulatedToColumns();
            }
            else if (type == "Rigid")
            {
                IsRigidToColumns();
            }
        }

        public override void IsRigidToColumns()
        {
            BottomBars = new List<Curve>(BottomBars);
        }

        public override void IsArticulatedToColumns()
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

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }
    }
}