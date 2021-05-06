﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using WarehouseLib.Trusses;

namespace WarehouseLib
{
    public class CurvedTruss : Truss
    {
        public CurvedTruss(Plane plane, double length, double height, double maxHeight, double clearHeight,
            int divisions, string trussType, string articulationType, int columnsCount) : base(plane, length, height,
            maxHeight,
            clearHeight, divisions, trussType, articulationType, columnsCount)
        {
        }

        protected override void GenerateThickBottomBars()
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

        public Vector3d ComputeNormal(int index)
        {
            var crv = TopBars[index == 0 ? 0 : 1];
            var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
            var perp = Vector3d.CrossProduct(vectorA, Plane.ZAxis);
            perp.Unitize();
            var normal = Vector3d.CrossProduct(vectorA, perp);
            return normal;
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

        public override List<Vector3d> ComputeNormals(Curve crv, List<Point3d> points, int index)
        {
            List<Vector3d> normals = new List<Vector3d>();
            for (int i = 0; i < points.Count; i++)
            {
                var vectorA = index == 0 ? crv.TangentAtStart : crv.TangentAtEnd;
                var perp = Vector3d.CrossProduct(vectorA, Plane.ZAxis);
                perp.Unitize();
                var normal = Vector3d.CrossProduct(vectorA, perp);
                normals.Add(normal);
            }

            return normals;
        }
    }
}