using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib
{
    public abstract class Strap
    {
        public Line Axis;

        protected Strap()
        {
            
        }

        public abstract List<Strap> ConstructStraps(List<Strap> strapMethod);
    }
}