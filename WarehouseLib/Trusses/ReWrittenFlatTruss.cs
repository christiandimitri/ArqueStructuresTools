using System.Collections.Generic;
using Rhino.Geometry;
using WarehouseLib.Beams;
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

            TopBeamSkeleton = new List<Curve> {new Line(StartingPoints[0], StartingPoints[2]).ToNurbsCurve()};
            double moveFactor = _height - _clearHeight;
            BottomBeamSkeleton = new List<Curve>
            {
                new Line(StartingPoints[0] - (_plane.ZAxis * (_height - _clearHeight)),
                    StartingPoints[2] - (_plane.ZAxis * moveFactor)).ToNurbsCurve()
            };
        }

        protected override List<BeamAxis> GenerateIntermediateBeamAxis()
        {
            return base.GenerateIntermediateBeamAxis();
        }

        protected override void ConstructTruss()
        {
            base.ConstructTruss();
        }
    }
}