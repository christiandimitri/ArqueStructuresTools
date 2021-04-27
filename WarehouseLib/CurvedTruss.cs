using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public class CurvedTruss : Truss
    {
        public CurvedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions, string trussType, string articulationType) : base(plane, length, height, maxHeight, clearHeight, divisions, trussType, articulationType)
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
            throw new NotImplementedException();

        }
        public override void IsRigidToColumns()
        {
            throw new NotImplementedException();
        }

        public override void IsArticualtedToColumns()
        {
            throw new NotImplementedException();
        }

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }
        public Vector3d ComputeOffset()
        {
            var crv = TopBars[0];
            var ptA = crv.PointAtStart;
            var vertical = Vector3d.ZAxis * ComputeDifference();
            var vectorA= crv.TangentAtStart;
            var perp = Vector3d.CrossProduct(vectorA, Plane.ZAxis);
            var normal = Vector3d.CrossProduct(vectorA, perp);
            var offset = Vector3d.Multiply(vertical, normal);
            normal *= offset;
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
        public static double Corner(int index, double opposite, Curve curve)
        {
            // get tangent vector at start point
            Vector3d startTangent = curve.TangentAtStart;

            // compute the move factors at corners => offset / sin(angle)
            double angle;
            if (index == 0) angle = Vector3d.VectorAngle(startTangent, Vector3d.XAxis);
            else angle = Vector3d.VectorAngle(startTangent, -Vector3d.XAxis);
            double theta = 0.5 * (Math.PI) - angle;
            double hypothenus = opposite / Math.Sin(theta);
            return hypothenus;
        }

        public override void ConstructTruss(int divisions)
        {
            throw new NotImplementedException();
        }
    }
}
