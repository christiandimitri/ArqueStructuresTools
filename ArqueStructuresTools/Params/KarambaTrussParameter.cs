using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib.Utilities;

namespace ArqueStructuresTools
{
    public class KarambaTrussParameter : GH_Param<KarambaTrussGoo>, IGH_PreviewObject
    {
        public KarambaTrussParameter() : base("Karamba3D Truss", "KaTruss", "Karamba3D truss parameter",
            "Arque Structures", "Params", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("020C9350-8B91-4C2A-B92A-2F48C08B3958");

        public bool Hidden { get; set; }
        public bool IsPreviewCapable => true;

        private BoundingBox _box = new BoundingBox();
        public BoundingBox ClippingBox => _box;

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            _box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                if (VolatileData.get_Branch(path) is List<KarambaTrussGoo> branch)
                {
                    foreach (var karambaTrussGoo in branch)
                    {
                        var truss = karambaTrussGoo.Value;
                        if (truss.Karamba3DStaticColumns != null)
                        {
                            foreach (var column in truss.Karamba3DStaticColumns)
                            {
                                var line = column;
                                _box.Union(line.Axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(line.Axis.ToNurbsCurve(), System.Drawing.Color.HotPink);
                            }
                        }

                        if (truss.GetKaramba3DTopBeams != null)
                        {
                            foreach (var beam in truss.GetKaramba3DTopBeams)
                            {
                                var line = beam;
                                foreach (var axis in line.Axis)
                                {
                                    _box.Union(axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(axis.ToNurbsCurve(), System.Drawing.Color.Blue);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class KarambaTrussGoo : GH_Goo<KarambaTruss>
    {
        public KarambaTrussGoo(KarambaTruss kaTruss)
        {
            Value = kaTruss;
        }

        public KarambaTrussGoo()
        {
            Value = null;
        }

        public override KarambaTruss Value
        {
            get => base.Value;
            set => base.Value = value;
        }

        public override bool IsValid => true;

        public override string TypeName => "Karamba3D Truss";

        public override string TypeDescription => "This is the desc of the karamba3D truss.....";

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