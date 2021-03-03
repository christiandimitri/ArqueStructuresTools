using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Utilities.Compute
{
    class Hypothenus
    {
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
    }
}
