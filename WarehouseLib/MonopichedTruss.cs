using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    class MonopichedTruss : PichedTruss
    {
        public MonopichedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions) : base(plane, length, height, maxHeight,clearHeight, divisions)
        {
            GenerateUpperBars();
            TopNodes = new List<Point3d>();
            BottomNodes = new List<Point3d>();
            foreach (var bar in UpperBars)
            {
                TopNodes.AddRange(GetNodesOnCurve(bar, divisions));
                GenerateLowerNodes(GetNodesOnCurve(bar, divisions), ComputeDifference());
            }
        }

        public override void GenerateUpperBars()
        {
            StartingNodes = GetStartingPoints(Plane, Length, Height, Height + ((MaxHeight - Height) / 2), MaxHeight);
            var barA = new Line(StartingNodes[0], StartingNodes[1]);
            var barB = new Line(StartingNodes[2], StartingNodes[1]);
            UpperBars = new List<Curve> { barA.ToNurbsCurve(), barB.ToNurbsCurve() };
        }

        public override void GenerateLowerBars()
        {
            throw new NotImplementedException();
        }
        public override void GenerateBeams()
        {
            throw new NotImplementedException();
        }
    }
}
