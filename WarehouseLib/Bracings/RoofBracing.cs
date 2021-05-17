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

            foreach (var node in nodes)
            {
                beam.ClosestPoint(node, out double t);
                var axis = new Line(node, beam.PointAt(t));
                var bracing = new RoofBracing();
                bracing.Axis = axis;
                bracings.Add(bracing);
            }

            return bracings;
        }
    }
}