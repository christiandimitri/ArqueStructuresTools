using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Bracings;
using WarehouseLib.Crosses;

namespace ArqueStructuresTools
{
    public class CrossParameter : GH_Param<CrossGoo>, IGH_PreviewObject
    {
        public CrossParameter() : base("Cross", "Cross", "Contains a collection of Crosses", "Arque Structures",
            "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("D87BBB14-7F90-4E21-B7E6-EFE8BC66F0ED");

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ParamCross;
            }
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<CrossGoo> branch)
                {
                    foreach (var crossGoo in branch)
                    {
                        var cross = crossGoo.Value;
                        if (cross.Axis != null)
                        {
                            foreach (var axis in cross.Axis)
                            {
                                _box.Union(axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(axis.ToNurbsCurve(), System.Drawing.Color.Purple);
                            }
                        }
                    }
                }
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            return;
        }

        public bool Hidden { get; set; }
        public bool IsPreviewCapable => true;
        public BoundingBox ClippingBox => _box;
        private BoundingBox _box = new BoundingBox();
    }

    public class CrossGoo : GH_Goo<Cross>
    {
        public CrossGoo(Cross cross)
        {
            Value = cross;
        }

        public CrossGoo()
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
        public override string TypeName => "Cross";
        public override string TypeDescription => "This is the desc for the Cross";
    }
}