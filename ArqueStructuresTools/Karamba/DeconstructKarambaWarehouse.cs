using System;
using System.Collections.Generic;
using System.Linq;
using ArqueStructuresTools.Params;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ArqueStructuresTools.Karamba
{
    public class DeconstructKarambaWarehouse : GH_Component
    {
        public DeconstructKarambaWarehouse() : base("Deconstruct Karamba3D Warehouse", "DeKaWarehouse",
            "Deconstruct a Warehouse into its karamba parts inputs", "Arque Structures", "Karamba3D")
        {
        }

        public override Guid ComponentGuid => new Guid("97481C7C-7FCB-4F6B-96AD-D1C6BDB8EA11");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new KarambaWarehouseParameter(), "Karamba warehouse", "KaWarehouse",
                "Karamba warehouse to deconstruct",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new KarambaTrussParameter(), "Trusses", "T", "Warehouse trusses",
                GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "Roof straps", "RS", "Warehouse roof straps",
                GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "X Facade straps", "X-FS",
                "Warehouse facade straps X-direction", GH_ParamAccess.list);
            pManager.AddParameter(new StrapParameter(), "Y Facade straps", "Y-FS",
                "Warehouse facade straps Y-direction", GH_ParamAccess.list);
            pManager.AddParameter(new BracingParameter(), "Roof bracing", "RB", "Warehouse roof bracing",
                GH_ParamAccess.list);
            pManager.AddParameter(new BracingParameter(), "Columns bracing", "CB", "Warehouse columns bracing",
                GH_ParamAccess.list);
            pManager.AddParameter(new CableParameter(), "Roof cables", "RC", "Warehouse roof cables",
                GH_ParamAccess.list);
            pManager.AddParameter(new CableParameter(), "Facade cables", "FC", "Warehouse facade cables",
                GH_ParamAccess.list);
            pManager.AddParameter(new CrossParameter(), "St-And cross", "SAC", "Warehouse st andre cross",
                GH_ParamAccess.list);
            pManager.AddPointParameter("St-And cross bottom nodes", "SCN", "St andre's bottom nodes",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            KarambaWarehouseGoo karambaWarehouseGoo = new KarambaWarehouseGoo();

            if (!DA.GetData(0, ref karambaWarehouseGoo)) return;

            var warehouse = karambaWarehouseGoo.Value;

            var trusses = new List<KarambaTrussGoo>();
            if (warehouse.KarambaTrusses != null)
            {
                foreach (var truss in warehouse.KarambaTrusses)
                {
                    trusses.Add(new KarambaTrussGoo(truss));
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

            var crossStAndre = new List<CrossGoo>();
            if (warehouse.Crosses != null)
            {
                foreach (var cross in warehouse.Crosses)
                {
                    crossStAndre.Add(new CrossGoo(cross));
                }
            }

            var stAndresBottomNodes = new List<Point3d>();
            if (warehouse.StAndresBottomNodes != null)
            {
                stAndresBottomNodes = warehouse.StAndresBottomNodes;
            }

            DA.SetDataList(0, new List<KarambaTrussGoo>(trusses));
            DA.SetDataList(1, new List<StrapGoo>(roofStraps));
            DA.SetDataList(2, new List<StrapGoo>(facadeStrapsX));
            DA.SetDataList(3, new List<StrapGoo>(facadeStrapsY));
            DA.SetDataList(4, new List<BracingGoo>(roofBracings));
            DA.SetDataList(5, new List<BracingGoo>(columnsBracings));
            DA.SetDataList(6, new List<CableGoo>(roofCables));
            DA.SetDataList(7, new List<CableGoo>(facadeCables));
            DA.SetDataList(8, new List<CrossGoo>(crossStAndre));
            DA.SetDataList(9, stAndresBottomNodes);
        }
    }
}