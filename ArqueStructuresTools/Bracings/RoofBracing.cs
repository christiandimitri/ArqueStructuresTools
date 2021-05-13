using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Trusses;

namespace ArqueStructuresTools.Bracings
{
    public class RoofBracing : GH_Component
    {
        public RoofBracing() : base("Roof Bracing", "Nickname", "Description", "Arque Structures", "Bracing")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("498401B2-CCA1-44A2-A654-D2D60AF5A6D6"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "Trusses", "t", "t", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter(), "Roof bracing", "rb", "rb", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussesGoo = new List<TrussGoo>();

            if (!DA.GetDataList(0, trussesGoo)) return;
            var trusses = new List<Truss>();
            for (var i = 0; i < trussesGoo.Count; i++)
            {
                var trussGoo = trussesGoo[i];
                var truss = trussGoo.Value;
                trusses.Add(truss);
            }

            var roofStraps = new List<StrapGoo>();
            var tempStraps = new RoofStrap().ConstructRoofStraps(trusses, 0);
            foreach (var strap in tempStraps)
            {
                roofStraps.Add(new StrapGoo(strap));
            }

            DA.SetDataList(0, new List<StrapGoo>(roofStraps));
        }
    }
}