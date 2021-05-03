using System.Collections.Generic;
using Rhino.Geometry;

namespace WarehouseLib
{
    public abstract class Strap
    {
        public Line Axis;
        public List<Strap> FacadeStrapsX;
        public List<Strap> FacadeStrapsY;
        protected Strap(Line axis)
        {
            Axis = axis;
        }
    }
}