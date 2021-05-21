using System;
using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public abstract class CurvedTruss : Truss
    {
        public CurvedTruss(Plane plane, TrussOptions options) : base(plane, options)
        {
        }

        protected override void GenerateThickBottomBars()
        {
            throw new NotImplementedException();
        }

        protected override List<Curve> ComputeBottomBarsArticulatedToColumns(List<Curve> bars)
        {
            throw new NotImplementedException();
        }

        protected override void GenerateBottomNodes(Curve crv)
        {
            throw new NotImplementedException();
        }

        protected override void IsArticulatedToColumns()
        {
            throw new NotImplementedException();
        }

        public override void GenerateTopBars()
        {
            throw new NotImplementedException();
        }

        public Vector3d ComputeNormalAtStartEnd(int index)
        {
            var crv = TopBeamAxisCurves[index == 0 ? 0 : 1];
            var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
            var perp = Vector3d.CrossProduct(vectorA, _plane.ZAxis);
            perp.Unitize();
            var normal = Vector3d.CrossProduct(vectorA, perp);
            return normal;
        }

        // TODO index compute from max min height
        public double ComputeOffsetFromTrigo(int index)
        {
            var angle = Vector3d.VectorAngle(-_plane.ZAxis, ComputeNormalAtStartEnd(index));
            var offset = Math.Cos(angle) * ComputeDifference();
            return offset;
        }

        public abstract double ComputeArticulatedOffsetFromTrigo(int index, double difference);
        public double ComputeOffsetFromDot(int index)
        {
            var normal = ComputeNormalAtStartEnd(index);
            var vertical = Vector3d.ZAxis * ComputeDifference();
            double offset = 0;
            offset = Vector3d.Multiply(normal, vertical);
            return offset;
        }

        public override void ConstructTruss(int divisions)
        {
            throw new NotImplementedException();
        }

        public override List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index)
        {
            throw new NotImplementedException();
        }
    }
}