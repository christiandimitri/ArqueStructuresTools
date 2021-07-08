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

        protected override void GenerateIntermediateBeamAxis()
        {
            Connections.Connections connections = null;

            var bars = new List<Curve>();
            if (_inputs.TrussType == ConnectionType.Warren)
            {
                connections = new WarrenConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }

            else if (_inputs.TrussType == ConnectionType.WarrenStuds)
            {
                connections = new WarrenStudsConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Pratt)
            {
                connections = new PrattConnection(TopPoints, BottomPoints);
                bars = connections.ConstructConnections();
            }
            else if (_inputs.TrussType == ConnectionType.Howe)
            {
                connections = new HoweConnection(TopPoints, BottomPoints, _articulationType);
                bars = connections.ConstructConnections();
            }

            IntermediateBeamSkeleton = bars;
        }

        protected override void ConstructTruss()
        {
            base.ConstructTruss();
        }
    }
}