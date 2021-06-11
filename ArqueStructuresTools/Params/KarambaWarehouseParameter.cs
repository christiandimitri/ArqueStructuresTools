﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Utilities;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools
{
    public class KarambaWarehouseParameter : Grasshopper.Kernel.GH_Param<KarambaWarehouseGoo>, IGH_PreviewObject
    {
        public KarambaWarehouseParameter() : base("Karamba Warehouse", "Warehouse",
            "Contains a collection of Warehouses", "Arque Structures", "Karamba3D",
            GH_ParamAccess.item)
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.ParamWarehouse; }
        }

        public override Guid ComponentGuid => new Guid("6CA2248A-336C-4905-ADD2-B2BEFD58B14C");

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
                if (VolatileData.get_Branch(path) is List<KarambaWarehouseGoo> branch)
                {
                    foreach (var warehouseGoo in branch)
                    {
                        var warehouse = warehouseGoo.Value;

                        foreach (var truss in warehouse.Trusses)
                        {
                            if (truss.TopBeam.Axis != null)
                            {
                                foreach (var bar in truss.TopBeam.Axis)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Blue);
                                }
                            }


                            if (truss.BottomBeam.Axis != null)
                            {
                                foreach (var bar in truss.BottomBeam.Axis)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Red);
                                }
                            }

                            if (truss.IntermediateBeams.Axis != null)
                            {
                                foreach (var bar in truss.IntermediateBeams.Axis)
                                {
                                    _box.Union(bar.GetBoundingBox(false));
                                    args.Display.DrawCurve(bar, System.Drawing.Color.Green);
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

    public class KarambaWarehouseGoo : GH_Goo<KarambaWarehouse>
    {
        public KarambaWarehouseGoo(KarambaWarehouse warehouse)
        {
            Value = warehouse;
        }

        public KarambaWarehouseGoo()
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