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

        protected PichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions) : base(plane, length, height, maxHeight, clearHeight, divisions)
        {

        }

        public override void GenerateBeams()
        {
            throw new NotImplementedException();
        }

        public override void GenerateLowerBars()
        {
            throw new NotImplementedException();
        }

        public override void GenerateLowerNodes(List<Point3d> points, double difference)
        {
            List<Point3d> nodes = new List<Point3d>();
            foreach (var pt in points)
            {
                Point3d tempPt = pt - (Vector3d.ZAxis * difference);
                nodes.Add(tempPt);
            }
            BottomNodes.AddRange(nodes);
        }

        public override void GenerateNodes(int divisions)
        {
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            foreach (var bar in TopBars)
            {
                TopNodes.AddRange(GetNodesOnCurve(bar, divisions));
                GenerateLowerNodes(TopNodes, ComputeDifference());
            }
        }

        public override void GenerateUpperBars()
        {
            throw new NotImplementedException();
        }
    }
}
