using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Bracings;

namespace ArqueStructuresTools
{
    public class BracingParameter : GH_Param<BracingGoo>, IGH_PreviewObject
    {
        public BracingParameter() : base("Bracing", "Bracing", "Contains a collection of Bracing", "Arque Structures", "Params", GH_ParamAccess.item)
        {
            
        }

        public override Guid ComponentGuid => new Guid("446BCC6A-E9A9-4B0C-AD9F-E432AC83B0A9");

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<BracingGoo> branch)
                {
                    foreach (var bracingGoo in branch)
                    {
                        var bracing = bracingGoo.Value;
                        if (bracing != null)
                        {
                            _box.Union(bracing.Axis.ToNurbsCurve().GetBoundingBox(false));
                            args.Display.DrawCurve(bracing.Axis.ToNurbsCurve(), System.Drawing.Color.Purple);
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

    public class BracingGoo : GH_Goo<Bracing>
    {
        public BracingGoo(Bracing bracing)
        {
            Value = bracing;
        }

        public BracingGoo()
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
        public override string TypeName => "Bracing";
        public override string TypeDescription => "This is the desc for the bracing";
    }

}