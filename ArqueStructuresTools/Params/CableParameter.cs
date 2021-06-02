using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Cables;

namespace ArqueStructuresTools
{
    public class CableParameter : GH_Param<CableGoo>, IGH_PreviewObject
    {
        public CableParameter() : base("Cable", "Cable", "Contains a collection of Cables", "Arque Structures", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("87C72D51-89C7-4464-9914-D180C9636EBE");

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<CableGoo> branch)
                {
                    foreach (var bracingGoo in branch)
                    {
                        var cable = bracingGoo.Value;
                        if (cable != null)
                        {
                            _box.Union(cable.Axis.ToNurbsCurve().GetBoundingBox(false));
                            args.Display.DrawCurve(cable.Axis.ToNurbsCurve(), System.Drawing.Color.Firebrick);
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

    public class CableGoo : GH_Goo<Cable>
    {
        public CableGoo(Cable cable)
        {
            Value = cable;
        }

        public CableGoo()
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
        public override string TypeName => "Cable";
        public override string TypeDescription => "This is the desc for the cable";
    }
}