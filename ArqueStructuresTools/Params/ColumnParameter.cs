using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Columns;

namespace ArqueStructuresTools
{
    public class ColumnParameter : GH_Param<ColumnGoo>, IGH_PreviewObject
    {
        public ColumnParameter() : base("Column parameter", "Column", "This is the columns parameter component",
            "Arque Structures", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("E65BD2B5-98B4-4788-A2A7-9A7AB2915B53");

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();

            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<ColumnGoo> branch)
                {
                    foreach (var columnGoo in branch)
                    {
                        var column = columnGoo.Value;
                        if (column != null)
                        {
                            _box.Union(column.Axis.ToNurbsCurve().GetBoundingBox(false));
                            args.Display.DrawCurve(column.Axis.ToNurbsCurve(), System.Drawing.Color.Aqua);
                        }
                    }
                }
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            throw new NotImplementedException();
        }

        public bool Hidden { get; set; }
        public bool IsPreviewCapable => true;
        public BoundingBox ClippingBox => _box;
        private BoundingBox _box = new BoundingBox();
    }

    public class ColumnGoo : GH_Goo<Column>
    {
        public ColumnGoo(Column column)
        {
            Value = column;
        }

        public ColumnGoo()
        {
            Value = null;
        }

        public override IGH_Goo Duplicate()
        {
            return this.Duplicate();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool IsValid => true;
        public override string TypeName => "Column";
        public override string TypeDescription => "This is the Columns description";
    }
}