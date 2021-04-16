using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class TrussParameter : Grasshopper.Kernel.GH_Param<TrussGoo>, IGH_PreviewObject
    {

        public TrussParameter() : base("Truss Parameter", "TP", "Truss...", "Arque Structures", "test", GH_ParamAccess.item)
        {
        }


        public override Guid ComponentGuid => new Guid("23DE33C1-E75A-47E4-8280-5D2A62883BF2");

        public bool Hidden { get; set; }

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => box;

        private BoundingBox box = new BoundingBox();
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
 
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            box = new BoundingBox();
            foreach (var path in VolatileData.Paths)
            {
                var branch = VolatileData.get_Branch(path) as List<TrussGoo>;
                foreach (var trussGoo in branch)
                {
                    var truss = trussGoo.Value;
                    foreach (var line in truss.TopBars)
                    {
                        box.Union(line.GetBoundingBox(false));
                        args.Display.DrawCurve(line, System.Drawing.Color.Blue);
                    }
                }
                foreach (var trussGoo in branch)
                {
                    var truss = trussGoo.Value;
                    foreach (var line in truss.BottomBars)
                    {
                        box.Union(line.GetBoundingBox(false));
                        args.Display.DrawCurve(line, System.Drawing.Color.Blue);
                    }
                }
                foreach (var trussGoo in branch)
                {
                    var truss = trussGoo.Value;
                    foreach (var line in truss.Columns)
                    {
                        box.Union(line.Axis.ToNurbsCurve().GetBoundingBox(false));
                        args.Display.DrawCurve(line.Axis.ToNurbsCurve(), System.Drawing.Color.Blue);
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

        public override Truss Value { get => base.Value; set => base.Value = value; }

        public override bool IsValid => true;

        public override string TypeName => "Truss";

        public override string TypeDescription =>"This is the desc of the truss.....";

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
