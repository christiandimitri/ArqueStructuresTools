using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseLib
{
    public abstract class PichedTruss : Truss
    {
        protected PichedTruss(Plane plane, double length, double height, double maxHeight, int divisions) : base(plane, length, height, maxHeight, divisions)
        {

        }

        public override void GenerateLowerBars()
        {
            throw new NotImplementedException();
        }

        public override void GenerateLowerNodes()
        {
            throw new NotImplementedException();
        }

        public override void GenerateUpperBars()
        {
            throw new NotImplementedException();
        }
        public override void GenerateBeams()
        {
            throw new NotImplementedException();
        }
    }
}
