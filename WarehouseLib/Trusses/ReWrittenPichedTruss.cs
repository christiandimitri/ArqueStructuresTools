using WarehouseLib.Options;
using Rhino.Geometry;

namespace WarehouseLib.Trusses
{
    public abstract class ReWrittenPichedTruss : ReWrittenTruss
    {
        public ReWrittenPichedTruss(Plane plane, TrussInputs inputs) : base(plane, inputs)
        {
            ConstructBeamsSkeleton();
        }
    }
}