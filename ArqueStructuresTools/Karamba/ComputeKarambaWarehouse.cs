using System;
using Grasshopper.Kernel;
using WarehouseLib.Utilities;
using WarehouseLib.Warehouses;

namespace ArqueStructuresTools.Karamba
{
    public class ComputeKarambaWarehouse : GH_Component
    {
        public ComputeKarambaWarehouse() : base("Compute Karamba3D Warehouse", "ConKaWarehouse",
            "Construct a Warehouse into its karamba parts inputs", "Arque Structures", "Karamba3D")
        {
        }

        public override Guid ComponentGuid => new Guid("1394D5EF-9D8C-4DAD-93BB-CFD6A88B7B8C");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WarehouseParameter(), "Warehouse", "W",
                "Warehouse to compute for a structural analysis", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new KarambaWarehouseParameter(), "Karamba warehouse", "KaWarehouse",
                "Karamba warehouse ready to analyse", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var warehouseGoo = new WarehouseGoo();

            // get input data
            if (!DA.GetData(0, ref warehouseGoo)) return;
            
            Warehouse warehouse = warehouseGoo.Value;

            KarambaWarehouse karambaWarehouse = new KarambaWarehouse(warehouse);

            // set output data 
            DA.SetData(0, new KarambaWarehouseGoo(karambaWarehouse));
        }
    }
}