using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry.Intersect;

namespace WarehouseLib
{
    public class CurvedTruss : Truss
    {
        public CurvedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
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
            throw new NotImplementedException();
        }

        public override void GenerateBottomNodes(Curve crv)
        {
            List<Point3d> nodes = new List<Point3d>();
            List<double> parameters = new List<double>();
            var difference = ComputeDifference();
            var points = new List<Point3d>(TopNodes);
            var intersectingLines = new List<Line>();
            for (int i = 0; i < points.Count; i++)
            {
                var tempPt = Plane.Origin - Vector3d.ZAxis * MaxHeight;
                var lineA = new Line(points[i], new Point3d(points[i].X, points[i].Y, tempPt.Z));
                intersectingLines.Add(lineA);
            }

            foreach (var line in intersectingLines)
            {
                var intersectionEvents = Intersection.CurveCurve(crv, line.ToNurbsCurve(), 0.01, 0.0);
                if (intersectionEvents != null)
                {
                    for (int i = 0; i < intersectionEvents.Count; i++)
                    {
                        var intEv = intersectionEvents[0];
                        nodes.Add(intEv.PointA);
                    }
                }
            }
            // foreach (var parameter in parameters)
            // {
            //     nodes.Add(crv.PointAt(parameter));
            // }

            // BottomNodes.AddRange(nodes);

            // List<Point3d> nodes = new List<Point3d>();
            // var points = TopNodes;
            // var difference = ComputeDifference();
            // BottomNodes = new List<Point3d>();
            // foreach (var pt in points)
            // {
            //     Point3d tempPt = pt - (Vector3d.ZAxis * difference);
            //     nodes.Add(tempPt);
            // }
            //
            BottomNodes.AddRange(nodes);
        }

        public override void ComputeArticulationAtColumns(string type)
        {
            throw new NotImplementedException();
        }

        public override void IsRigidToColumns()
        {
            throw new NotImplementedException();
        }

        public override void IsArticulatedToColumns()
        {
            throw new NotImplementedException();
        }

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }

        public Vector3d ComputeNormal(int index)
        {
            var crv = TopBars[index == 0 ? 0 : 1];
            var vertical = Vector3d.ZAxis * ComputeDifference();
            var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
            var perp = Vector3d.CrossProduct(vectorA, Plane.ZAxis);
            perp.Unitize();
            var normal = Vector3d.CrossProduct(vectorA, perp);
            return normal;
        }

        public static double Center(double offsetFactor, Curve curve, Vector3d normalVector)
        {
            // get tangent vector at end point
            Vector3d endTangent = curve.TangentAtEnd;

            // compute at center index, the move factor => offset / sin(angle)
            double angle = Vector3d.VectorAngle(endTangent, normalVector);
            double hypothenus = offsetFactor / Math.Sin(angle);
            return hypothenus;
        }

        // TODO index compute from max min height
        public double ComputeOffsetFromTrigo(int index)
        {
            var angle = Vector3d.VectorAngle(-Plane.ZAxis, ComputeNormal(index));
            var offset = Math.Cos(angle) * ComputeDifference();
            return offset;
        }

        public double ComputeOffsetFromDot(int index)
        {
            Vector3d normal = ComputeNormal(index);
            var vertical = Vector3d.ZAxis * ComputeDifference();
            var offset = Vector3d.Multiply(vertical, normal);
            return offset;
        }

        public override void ConstructTruss(int divisions)
        {
            throw new NotImplementedException();
        }
    }
}