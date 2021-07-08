using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Beams;

namespace ArqueStructuresTools.Params
{
    public class BeamParameter : GH_Param<BeamGoo>, IGH_PreviewObject
    {
        public BeamParameter() : base("Beam", "Beam", "Contains a collection of Beams", "Arque Structures", "Params",
            GH_ParamAccess.item)
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ParamBeam;
            }
        }

        public override Guid ComponentGuid => new Guid("690012B0-B731-4D92-A7CF-9B89FC517333");

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();

            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<BeamGoo> branch)
                {
                    foreach (var beamGoo in branch)
                    {
                        var beam = beamGoo.Value;
                        if (beam.SkeletonAxis != null)
                        {
                            foreach (var axis in beam.SkeletonAxis)
                            {
                                if (axis != null)
                                {
                                    _box.Union(axis.AxisCurve.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(axis.AxisCurve.ToNurbsCurve(), System.Drawing.Color.Red);
                                }
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
        private BoundingBox _box;
    }

    public class BeamGoo : GH_Goo<Beam>
    {
        public BeamGoo(Beam beam)
        {
            Value = beam;
        }

        public BeamGoo()
        {
            Value = null;
        }

        public override IGH_Goo Duplicate()
        {
            return this.Duplicate();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override bool IsValid => true;
        public override string TypeName => "Beam";
        public override string TypeDescription => "This is the desc of the beam";
    }
}