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
        public StrapParameter() : base("Strap", "Strap", "Contains a collection of Straps", "Arque Structures", "Params",
            GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("3C3D0887-4CB2-4292-BF85-AA39DCBC0B0E");
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ParamStrap;
            }
        }
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<StrapGoo> branch)
                {
                    foreach (var strapGoo in branch)
                    {
                        var strap = strapGoo.Value;
                        if (strap != null)
                        {
                            _box.Union(strap.Axis.ToNurbsCurve().GetBoundingBox(false));
                            args.Display.DrawCurve(strap.Axis.ToNurbsCurve(), System.Drawing.Color.Orange);
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