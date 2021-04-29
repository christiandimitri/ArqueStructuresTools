using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class StrapParameter : GH_Param<StrapGoo>, IGH_PreviewObject
    {
        public StrapParameter() : base("Strap Parameter", "S", "Description", "Arque Structures", "Straps",
            GH_ParamAccess.item)
        {
        }


        public override Guid ComponentGuid => new Guid("3C3D0887-4CB2-4292-BF85-AA39DCBC0B0E");

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<WarehouseGoo> branch)
                {
                    foreach (var warehouseGoo in branch)
                    {
                        var warehouse = warehouseGoo.Value;
                        foreach (var deckStrap in warehouse.DeckStraps)
                        {
                            _box.Union(deckStrap.Axis.ToNurbsCurve().GetBoundingBox(false));
                            args.Display.DrawCurve(deckStrap.Axis.ToNurbsCurve(), System.Drawing.Color.Yellow);
                        }
                    }
                }
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }

        public bool Hidden { get; set; }
        public bool IsPreviewCapable => true;
        public BoundingBox ClippingBox => _box;
        private BoundingBox _box = new BoundingBox();
    }

    public class StrapGoo : GH_Goo<Strap>
    {
        public StrapGoo(Strap strap)
        {
            Value = strap;
        }

        public StrapGoo()
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
        public override string TypeName => "Strap";
        public override string TypeDescription => "This is the desc of the Strap";
    }
}