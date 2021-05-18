using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using WarehouseLib.Trusses;

namespace WarehouseLib.Bracings
{
    public class RoofBracing : Bracing
    {
        public RoofBracing()
        {
        }

        public override List<Bracing> ConstructBracings(List<Point3d> nodes, Curve beam)
        {
            var bracings = new List<Bracing>();

            foreach (var node in nodes)
            {
                beam.ClosestPoint(node, out double t);
                var axis = new Line(node, beam.PointAt(t));
                var bracing = new RoofBracing();
                bracing.Axis = axis;
                bracings.Add(bracing);
            }

            bracings.RemoveAt(0);
            bracings.RemoveAt(bracings.Count - 1);
            return bracings;
        }

        public List<Bracing> ConstructWarrenStudsBracings(List<Point3d> nodes, Curve beam)
        {
            var bracings = new List<Bracing>();
            var outerPoints = nodes;
            var innerPoints = new List<Point3d>();
            foreach (var node in outerPoints)
            {
                beam.ClosestPoint(node, out double t);
                innerPoints.Add(beam.PointAt(t));
            }

            for (var i = 0; i < innerPoints.Count; i++)
            {
                var ptA = new Point3d();
                var ptB = innerPoints[i];
                    ptA = outerPoints[i];
                    var axis = new Line(ptA, ptB);
                    var bracing = new RoofBracing();
                    bracing.Axis = axis;
                    bracings.Add(bracing);
                
                if (i % 2 == 1)
                {
                    ptA = outerPoints[i - 1];
                     axis = new Line(ptA, ptB);
                     bracing = new RoofBracing();
                    bracing.Axis = axis;
                    bracings.Add(bracing);
                    if (i < outerPoints.Count - 1)
                    {
                        ptA = outerPoints[i + 1];
                        axis = new Line(ptA, ptB);
                        bracing = new RoofBracing();
                        bracing.Axis = axis;
                        bracings.Add(bracing);
                    }
                }
            }

            return bracings;
        }
    }
}