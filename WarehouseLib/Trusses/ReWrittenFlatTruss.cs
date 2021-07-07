using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Connections;
using WarehouseLib.Options;

namespace WarehouseLib.Trusses
{
    public class ReWrittenFlatTruss : ReWrittenPichedTruss
    {
        public ReWrittenFlatTruss(Plane plane, TrussInputs inputs) : base(plane, inputs)
        {
        }

        protected override void ConstructBeamsSkeleton()
        {
            // construct starting points
            StartingPoints = ConstructStartingPoints(_plane, _width / 2, _width / 2, _height, _height, _height);

            // construct top skeleton curve

            TopBeamSkeleton = new Line(StartingPoints[0], StartingPoints[2]).ToNurbsCurve();
            double moveFactor = _height - _clearHeight;
            BottomBeamSkeleton = new Line(StartingPoints[0] - (_plane.ZAxis * (_height - _clearHeight)),
                StartingPoints[2] - (_plane.ZAxis * moveFactor)).ToNurbsCurve();
        }

        protected override void ConstructTruss()
        {
            var divisionParams =
                TopBeamSkeleton.DivideByCount(
                    _connectionType == ConnectionType.Warren ? _divisions * 2 : _divisions, true);
            var topElements = GenerateTopBeamDivisions(TopBeamSkeleton, divisionParams);

            TopPoints.AddRange(topElements.nodes);
            TopBeamAxis.AddRange(topElements.axis);

            var bottomElements = GenerateBottomBeamDivisions(BottomBeamSkeleton, topElements.nodes);
            BottomPoints.AddRange(bottomElements.nodes);
            BottomBeamAxis.AddRange(bottomElements.axis);

            var cloud = new PointCloud(TopPoints);
            var index = cloud.ClosestPoint(StartingPoints[1]);
            GenerateIntermediateBeamAxis();        }
    }
}