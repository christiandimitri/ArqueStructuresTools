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
            int divisions, string trussType) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType)
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

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }
    }
}