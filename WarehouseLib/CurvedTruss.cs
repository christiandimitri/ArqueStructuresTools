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
        public CurvedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight, int divisions) : base(plane, length, height, maxHeight, clearHeight, divisions)
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
            throw new NotImplementedException();
        }

        public override void GenerateUpperBars()
        {
            throw new NotImplementedException();
        }
        public double ComputeOffset(int index, double difference, Curve curve)
        {

            double offset, angle;
            //get vector at start point
            Vector3d startTangentVector = curve.TangentAtStart;

            // compute at smallest index, the move factor => sin(angle) * hypotenus = height
            //if (index == 0) 
            angle = Vector3d.VectorAngle(startTangentVector, Vector3d.XAxis);
            //else angle = Vector3d.VectorAngle(startTangentVector, -Vector3d.XAxis);
            double theta = 0.5 * (Math.PI) - angle;
            offset = Math.Sin(theta) * difference;
            return offset;
        }
        public double ComputeHypothenus(int index, double opposite, Curve curve)
        {
            // get tangent vector at start point
            Vector3d startTangent = curve.TangentAtStart;

            // compute the move factors at corners => offset / sin(angle)
            double angle;
            //if (index == 0) 
            angle = Vector3d.VectorAngle(startTangent, Vector3d.XAxis);
            //else angle = Vector3d.VectorAngle(startTangent, -Vector3d.XAxis);
            double theta = 0.5 * (Math.PI) - angle;
            double hypothenus = opposite / Math.Sin(theta);
            return hypothenus;
        }
    }
}
