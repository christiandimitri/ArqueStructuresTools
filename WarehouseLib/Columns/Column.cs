using Rhino.Geometry;

namespace WarehouseLib
{
    public class Column
    {
        public Line Axis;

        public Column(Line axis)
        {
            Axis = axis;
        }
    }
}