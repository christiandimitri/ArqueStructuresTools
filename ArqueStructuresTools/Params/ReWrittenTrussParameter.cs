using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools
{
    public class ReWrittenTrussParameter : Grasshopper.Kernel.GH_Param<ReWrittenTrussGoo>, IGH_PreviewObject
    {
        public ReWrittenTrussParameter() : base("ReTruss", "ReTruss", "Contains a collection of Trusses",
            "Arque Structures", "Params",
            GH_ParamAccess.item)
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ParamTruss;
            }
        }

        public override Guid ComponentGuid => new Guid("E88F651D-AA06-45C0-B3EF-5D672E14902D");

        public bool Hidden { get; set; }

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => _box;

        private BoundingBox _box = new BoundingBox();

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<ReWrittenTrussGoo> branch)
                {
                    foreach (var trussGoo in branch)
                    {
                        var truss = trussGoo.Value;
                        if (truss.TopBeamSkeleton != null)
                        {
                            _box.Union(truss.TopBeamSkeleton.GetBoundingBox(false));
                            args.Display.DrawCurve(truss.TopBeamSkeleton, System.Drawing.Color.Blue);
                        }

                        if (truss.BottomBeamSkeleton != null)
                        {
                            _box.Union(truss.BottomBeamSkeleton.GetBoundingBox(false));
                            args.Display.DrawCurve(truss.BottomBeamSkeleton, System.Drawing.Color.Red);
                        }

                        if (truss.IntermediateBeamSkeleton != null)
                        {
                            _box.Union(truss.IntermediateBeamSkeleton.GetBoundingBox(false));
                            args.Display.DrawCurve(truss.IntermediateBeamSkeleton, System.Drawing.Color.Green);
                        }

                        if (truss.StaticColumns != null)
                        {
                            foreach (var line in truss.StaticColumns)
                            {
                                _box.Union(line.Axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(line.Axis.ToNurbsCurve(), System.Drawing.Color.HotPink);
                            }
                        }

                        if (truss.BoundaryColumns != null)
                        {
                            foreach (var line in truss.BoundaryColumns)
                            {
                                _box.Union(line.Axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(line.Axis.ToNurbsCurve(), System.Drawing.Color.DeepPink);
                            }
                        }
                    }
                }
            }
        }
    }

    public class ReWrittenTrussGoo : GH_Goo<ReWrittenTruss>
    {
        public ReWrittenTrussGoo(ReWrittenTruss truss)
        {
            Value = truss;
        }

        public ReWrittenTrussGoo()
        {
            Value = null;
        }

        public sealed override ReWrittenTruss Value
        {
            get => base.Value;
            set => base.Value = value;
        }

        public override bool IsValid => true;

        public override string TypeName => "Truss";

        public override string TypeDescription => "This is the desc of the truss.....";

        public override bool CastFrom(object source)
        {
            return base.CastFrom(source);
        }

        public override bool CastTo<Q>(ref Q target)
        {
            return base.CastTo(ref target);
        }

        public override IGH_Goo Duplicate()
        {
            return this.Duplicate();
        }

        public override object ScriptVariable()
        {
            return base.ScriptVariable();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}