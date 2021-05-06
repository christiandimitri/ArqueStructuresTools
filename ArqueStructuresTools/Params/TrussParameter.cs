using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools
{
    public class TrussParameter : Grasshopper.Kernel.GH_Param<TrussGoo>, IGH_PreviewObject
    {
        public TrussParameter() : base("Truss Parameter", "TP", "Truss...", "Arque Structures", "Params",
            GH_ParamAccess.item)
        {
        }


        public override Guid ComponentGuid => new Guid("23DE33C1-E75A-47E4-8280-5D2A62883BF2");

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
                if (VolatileData.get_Branch(path) is List<TrussGoo> branch)
                {
                    foreach (var trussGoo in branch)
                    {
                        var truss = trussGoo.Value;
                        if (truss.TopBars != null)
                        {
                            foreach (var line in truss.TopBars)
                            {
                                _box.Union(line.GetBoundingBox(false));
                                args.Display.DrawCurve(line, System.Drawing.Color.Blue);
                            }
                        }

                        if (truss.BottomBars != null)
                        {
                            foreach (var line in truss.BottomBars)
                            {
                                _box.Union(line.GetBoundingBox(false));
                                args.Display.DrawCurve(line, System.Drawing.Color.Red);
                            }
                        }

                        if (truss.IntermediateBars != null)
                        {
                            foreach (var line in truss.IntermediateBars)
                            {
                                _box.Union(line.GetBoundingBox(false));
                                args.Display.DrawCurve(line, System.Drawing.Color.Green);
                            }
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

    public class TrussGoo : GH_Goo<Truss>
    {
        public TrussGoo(Truss truss)
        {
            Value = truss;
        }

        public TrussGoo()
        {
            Value = null;
        }

        public sealed override Truss Value
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