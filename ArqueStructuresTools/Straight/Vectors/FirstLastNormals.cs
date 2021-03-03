using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Straight.Vectors
{
    class FirstLastNormals
    {
        public static List<Vector3d> Get(List<Curve> curves)
        {
            List<Vector3d> normalVectors = new List<Vector3d>();
            Curve[] joinCurves = Curve.JoinCurves(curves);
            Curve tempCurve = joinCurves[0];
            Vector3d tangentAtStart = curves[0].PointAtEnd - curves[0].PointAtStart;
            tangentAtStart.Unitize();
            Vector3d tangentAtEnd = curves[1].PointAtEnd - curves[1].PointAtStart;
            tangentAtEnd.Unitize();
            Vector3d tangent1 = new Vector3d(curves[0].PointAtStart - curves[0].PointAtEnd);
            tangent1.Unitize();
            Vector3d tangent2 = new Vector3d(curves[1].PointAtStart - curves[1].PointAtEnd);
            tangent2.Unitize();
            double centerAngle = Vector3d.VectorAngle(tangent1, tangent2);
            tangent1.Rotate(-centerAngle / 2, Vector3d.YAxis);
            tangent2.Rotate(centerAngle / 2, Vector3d.YAxis);
            Vector3d tangentCenter = new Vector3d(tangent1 + tangent2 / 2);
            tangentCenter.Unitize();
            tangentAtStart.Rotate(0.5 * Math.PI, Vector3d.YAxis);
            tangentAtEnd.Rotate(-0.5 * Math.PI, Vector3d.YAxis);
            normalVectors.Add(tangentAtStart);
            normalVectors.Add(tangentCenter);
            normalVectors.Add(tangentAtEnd);
            return normalVectors;
        }

    }
}
