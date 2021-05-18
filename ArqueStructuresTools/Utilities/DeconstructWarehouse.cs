using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeconstructWarehouse : GH_Component
    {
        public DeconstructWarehouse() : base("Deconstruct Warehouse", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("45D2DC47-A714-4DFD-A19A-B281CFF1F870"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "w", "w", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Trusses", "t", "t", GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "Roof straps", "rs", "rs", GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "X Facade straps", "yfs", "xfs", GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "Y Facade straps", "xfs", "yfs", GH_ParamAccess.list);
            pManager.AddParameter(new BracingParameter(), "Roof bracings", "rb", "rb", GH_ParamAccess.list);
            pManager.AddParameter(new BracingParameter(), "Columns bracings", "cb", "cb", GH_ParamAccess.list);
            pManager.AddParameter(new CableParameter(), "Roof cables", "rc", "rc", GH_ParamAccess.list);
            pManager.AddParameter(new CableParameter(), "Facade cables", "fc", "fc", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            WarehouseGoo warehouseGoo = new WarehouseGoo();

            if (!DA.GetData(0, ref warehouseGoo)) return;

            var warehouse = warehouseGoo.Value;

            var trusses = new List<TrussGoo>();
            if (warehouse.Trusses != null)
            {
                foreach (var truss in warehouse.Trusses)
                {
                    trusses.Add(new TrussGoo(truss));
                }
            }

            var roofStraps = new List<StrapGoo>();
            if (warehouse.RoofStraps != null)
            {
                foreach (var roofStrap in warehouse.RoofStraps)
                {
                    roofStraps.Add(new StrapGoo(roofStrap));
                }
            }

            var facadeStrapsX = new List<StrapGoo>();
            if (warehouse.FacadeStrapsX != null)
            {
                foreach (var facadeStrap in warehouse.FacadeStrapsX)
                {
                    facadeStrapsX.Add(new StrapGoo(facadeStrap));
                }
            }

            var facadeStrapsY = new List<StrapGoo>();
            if (warehouse.FacadeStrapsY != null)
            {
                foreach (var facadeStrap in warehouse.FacadeStrapsY)
                {
                    facadeStrapsY.Add(new StrapGoo(facadeStrap));
                }
            }

            var roofBracings = new List<BracingGoo>();
            if (warehouse.RoofBracings != null)
            {
                foreach (var bracing in warehouse.RoofBracings)
                {
                    roofBracings.Add(new BracingGoo(bracing));
                }
            }

            var roofCables = new List<CableGoo>();
            if (warehouse.RoofBracings != null)
            {
                foreach (var cable in warehouse.RoofCables)
                {
                    roofCables.Add(new CableGoo(cable));
                }
            }

            var columnsBracings = new List<BracingGoo>();
            if (warehouse.ColumnsBracings != null)
            {
                foreach (var bracing in warehouse.ColumnsBracings)
                {
                    columnsBracings.Add(new BracingGoo(bracing));
                }
            }

            var facadeCables = new List<CableGoo>();
            if (warehouse.FacadeCables != null)
            {
                foreach (var cable in warehouse.FacadeCables)
                {
                    facadeCables.Add(new CableGoo(cable));
                }
            }

            DA.SetDataList(0, new List<TrussGoo>(trusses));
            DA.SetDataList(1, new List<StrapGoo>(roofStraps));
            DA.SetDataList(2, new List<StrapGoo>(facadeStrapsX));
            DA.SetDataList(3, new List<StrapGoo>(facadeStrapsY));
            DA.SetDataList(4, new List<BracingGoo>(roofBracings));
            DA.SetDataList(5, new List<BracingGoo>(columnsBracings));
            DA.SetDataList(6, new List<CableGoo>(roofCables));
            DA.SetDataList(7, new List<CableGoo>(facadeCables));
        }
    }
}