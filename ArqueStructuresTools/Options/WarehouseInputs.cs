﻿using System;
using Grasshopper.Kernel;
using WarehouseLib.Options;

namespace ArqueStructuresTools.Options
{
    public class WarehouseInputs : GH_Component
    {
        public WarehouseInputs() : base("Warehouse Inputs", "Nickname", "Description", "Arque Structures",
            "Inputs")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7C11BF31-DF26-4F1F-8233-1558162F2C89"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Typology", "t", "t", GH_ParamAccess.item, "Flat");
            pManager.AddNumberParameter("Length", "l", "l", GH_ParamAccess.item, 50.0);
            pManager.AddIntegerParameter("Portico count", "pc", "pc", GH_ParamAccess.item, 5);
            pManager.AddBooleanParameter("Portico at boundary", "pb", "pb", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Roof Bracing type", "bt", "bt", GH_ParamAccess.item, "Bracing");
            pManager.AddNumberParameter("Facade cables threshold", "fct", "fct", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("St Andre count", "sac", "sac", GH_ParamAccess.item, 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Warehouse options", "wp", "wp", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var typology = "";
            var length = 0.0;
            var porticoCount = 0;
            var hasBoundary = true;
            var roofBracingType = "";
            var facadeCablesThreshold = 0.0;
            var stAndreCrossCount = 0;
            if (!DA.GetData(0, ref typology)) return;
            if (!DA.GetData(1, ref length)) return;
            if (!DA.GetData(2, ref porticoCount)) return;
            if (!DA.GetData(3, ref hasBoundary)) return;
            if (!DA.GetData(4, ref roofBracingType)) return;
            if (!DA.GetData(5, ref facadeCablesThreshold)) return;
            if (!DA.GetData(6, ref stAndreCrossCount)) return;


            WarehouseLib.Options.WarehouseOptions warehouseInputs;
            try
            {
                warehouseInputs = new WarehouseLib.Options.WarehouseOptions(typology, length, porticoCount, hasBoundary,
                    roofBracingType,
                    facadeCablesThreshold, stAndreCrossCount);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }

            DA.SetData(0, warehouseInputs);
        }
    }
}