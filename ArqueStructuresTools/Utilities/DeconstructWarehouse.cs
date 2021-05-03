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
            pManager.AddParameter(new StrapParameter(), "XFacade Straps", "yfs", "xfs", GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "YFacade Straps", "xfs", "yfs", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            WarehouseGoo warehouseGoo = new WarehouseGoo();

            if (!DA.GetData(0, ref warehouseGoo)) return;

            var warehouse = warehouseGoo.Value;

            var trusses = new List<TrussGoo>();

            foreach (var truss in warehouse.Trusses)
            {
                trusses.Add(new TrussGoo(truss));
            }

            var roofStraps = new List<StrapGoo>();

            foreach (var roofStrap in warehouse.RoofStraps)
            {
                roofStraps.Add(new StrapGoo(roofStrap));
            }

            var facadeStraps = new List<StrapGoo>();

            foreach (var facadeStrap in warehouse.FacadeStraps)
            {
                facadeStraps.Add(new StrapGoo(facadeStrap));
            }

            DA.SetDataList(0, new List<TrussGoo>(trusses));
            DA.SetDataList(1, new List<StrapGoo>(roofStraps));
            DA.SetDataList(2, new List<StrapGoo>(facadeStraps));
        }
    }
}