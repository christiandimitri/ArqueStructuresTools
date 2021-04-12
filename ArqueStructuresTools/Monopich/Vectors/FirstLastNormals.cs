using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArqueStructuresTools.Monopich.Vectors
{
    class FirstLastNormals
    {
        public static List<Vector3d> Get(List<Curve> curves)
        {
            List<Vector3d> normalVectors = new List<Vector3d>();
            Vector3d tangentAtStart = curves[0].PointAtStart - curves[0].PointAtEnd;
            tangentAtStart.Unitize();
            Vector3d tangentAtEnd = curves[1].PointAtStart - curves[1].PointAtEnd;
            tangentAtEnd.Unitize();
            tangentAtStart.Rotate(-0.5 * Math.PI, Vector3d.YAxis);
            tangentAtStart.Unitize();
            tangentAtEnd.Rotate(0.5 * Math.PI, Vector3d.YAxis);
            tangentAtEnd.Unitize();
            normalVectors.Add(tangentAtStart);
            normalVectors.Add(tangentAtStart);
            normalVectors.Add(tangentAtEnd);
            return normalVectors;
        }

    }
}
