using System;
using System.Collections;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools
{
    public class WarehouseParameter : Grasshopper.Kernel.GH_Param<WarehouseGoo>, IGH_PreviewObject
    {
        public WarehouseParameter() : base("Warehouse", "Warehouse", "Contains a collection of Warehouses", "Arque Structures", "Params",
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
                            if (truss.TopBeamAxisCurves != null)
                            {
                                foreach (var bar in truss.TopBeamAxisCurves)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Blue);
                                }
                            }


                            if (truss.BottomBeamAxisCurves != null)
                            {
                                foreach (var bar in truss.BottomBeamAxisCurves)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Red);
                                }
                            }

                            if (truss.IntermediateBeamsAxisCurves != null)
                            {
                                foreach (var bar in truss.IntermediateBeamsAxisCurves)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Green);
                                }
                            }

                            if (warehouse.RoofStraps != null)
                            {
                                foreach (var strap in warehouse.RoofStraps)
                                {
                                    _box.Union(strap.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(strap.Axis.ToNurbsCurve(), System.Drawing.Color.Yellow);
                                }
                            }

                            if (warehouse.FacadeStrapsX != null)
                            {
                                foreach (var strap in warehouse.FacadeStrapsX)
                                {
                                    _box.Union(strap.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(strap.Axis.ToNurbsCurve(), System.Drawing.Color.Orange);
                                }
                            }

                            if (warehouse.FacadeStrapsY != null)
                            {
                                foreach (var strap in warehouse.FacadeStrapsY)
                                {
                                    _box.Union(strap.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(strap.Axis.ToNurbsCurve(), System.Drawing.Color.Orange);
                                }
                            }

                            if (truss.StaticColumns != null)
                            {
                                foreach (var bar in truss.StaticColumns)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.HotPink);
                                }
                            }

                            if (truss.BoundaryColumns != null)
                            {
                                foreach (var bar in truss.BoundaryColumns)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.DeepPink);
                                }
                            }

                            if (warehouse.RoofBracings != null)
                            {
                                foreach (var bar in warehouse.RoofBracings)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.Indigo);
                                }
                            }

                            if (warehouse.RoofCables != null)
                            {
                                foreach (var bar in warehouse.RoofCables)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.Firebrick);
                                }
                            }

                            if (warehouse.FacadeCables != null)
                            {
                                foreach (var bar in warehouse.FacadeCables)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.Firebrick);
                                }
                            }

                            if (warehouse.ColumnsBracings != null)
                            {
                                foreach (var bar in warehouse.ColumnsBracings)
                                {
                                    _box.Union(bar.Axis.ToNurbsCurve().GetBoundingBox(false));
                                    args.Display.DrawCurve(bar.Axis.ToNurbsCurve(), System.Drawing.Color.GreenYellow);
                                }
                            }

                            if (warehouse.Crosses != null)
                            {
                                foreach (var cross in warehouse.Crosses)
                                {
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