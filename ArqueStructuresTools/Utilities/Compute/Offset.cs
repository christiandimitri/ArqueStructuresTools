using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Utilities.Compute
{
    class Offset
    {
        public static double Compute(int index, int difference, Curve curve)
        {

            double offset, angle;
            //get vector at start point
            Vector3d startTangentVector = curve.TangentAtStart;

            // compute at smallest index, the move factor => sin(angle) * hypotenus = height
            if (index == 0) angle = Vector3d.VectorAngle(startTangentVector, Vector3d.XAxis);
            else angle = Vector3d.VectorAngle(startTangentVector, -Vector3d.XAxis);
            double theta = 0.5 * (Math.PI) - angle;
            offset = Math.Sin(theta) * difference;
            return offset;
        }
    }
}
