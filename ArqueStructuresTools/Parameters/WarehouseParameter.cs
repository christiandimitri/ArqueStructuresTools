using System;
using System.Collections;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class WarehouseParameter : Grasshopper.Kernel.GH_Param<WarehouseGoo>, IGH_PreviewObject
    {
        public WarehouseParameter() : base("Warehouse Parameter", "W", "Warehouse...", "Arque Structures", "Parameters",
            GH_ParamAccess.item)
        {
        }


        public override Guid ComponentGuid => new Guid("F70844D7-D165-4AD2-B854-E3A925342F0B");

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
                if (VolatileData.get_Branch(path) is List<WarehouseGoo> branch)
                {
                    foreach (var warehouseGoo in branch)
                    {
                        var warehouse = warehouseGoo.Value;
                        foreach (var truss in warehouse.Trusses)
                        {
                            foreach (var column in truss.Columns)
                            {
                                _box.Union(column.Axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(column.Axis.ToNurbsCurve(), System.Drawing.Color.Purple);
                            }

                            foreach (var topBars in truss.TopBars)
                            {
                                _box.Union(topBars.GetBoundingBox(false));
                                args.Display.DrawCurve(topBars, System.Drawing.Color.Blue);
                            }

                            foreach (var bottomBars in truss.BottomBars)
                            {
                                _box.Union(bottomBars.GetBoundingBox(false));
                                args.Display.DrawCurve(bottomBars, System.Drawing.Color.Red);
                            }

                            foreach (var interBars in truss.IntermediateBars)
                            {
                                _box.Union(interBars.GetBoundingBox(false));
                                args.Display.DrawCurve(interBars, System.Drawing.Color.Green);
                            }

                            foreach (var deck in warehouse.DeckStraps)
                            {
                                _box.Union(deck.Axis.ToNurbsCurve().GetBoundingBox(false));
                                args.Display.DrawCurve(deck.Axis.ToNurbsCurve(), System.Drawing.Color.Yellow);
                            }
                        }
                    }
                }
            }
        }
    }

    public class WarehouseGoo : GH_Goo<Warehouse>
    {
        public WarehouseGoo(Warehouse warehouse)
        {
            Value = warehouse;
        }

        public WarehouseGoo()
        {
            Value = null;
        }

        // public sealed override Warehouse Value
        // {
        //     get => base.Value;
        //     set => base.Value = value;
        // }

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