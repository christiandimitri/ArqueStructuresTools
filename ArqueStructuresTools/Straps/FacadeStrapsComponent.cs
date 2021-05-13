using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Straps;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools
{
    public class FacadeStrapsComponent : GH_Component
    {
        public FacadeStrapsComponent() : base("Construct Facade Straps", "Nickname", "Description", "Arque Structures",
            "Straps")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("D42A93FA-3CC8-4F74-BCB2-35A80CD5CF74"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Trusses", "", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("distance", "d", "d", GH_ParamAccess.item, 0.5);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter(), "FacadeX straps", "stx", "stx", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussesGoo = new List<TrussGoo>();
            var distance = 0.5;
            if (!DA.GetDataList(0, trussesGoo)) return;
            if (!DA.GetData(1, ref distance)) return;

            var trusses = new List<Truss>();
            for (var i = 0; i < trussesGoo.Count; i++)
            {
                var trussGoo = trussesGoo[i];
                var truss = trussGoo.Value;
                trusses.Add(truss);
            }

            var facadeStrapX = new List<StrapGoo>();
            var strapsX = new FacadeStrap().ConstructStrapsAxisOnStaticColumns(trusses, distance);

            foreach (var strap in strapsX)
            {
                facadeStrapX.Add(new StrapGoo(strap));
            }


            DA.SetDataList(0, new List<StrapGoo>(facadeStrapX));
        }
    }
}