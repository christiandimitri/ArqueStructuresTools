using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;
using WarehouseLib.Options;

namespace ArqueStructuresTools.Options
{
    public class TrussOptions : GH_Component
    {
        public TrussOptions() : base("Truss options", "Nickname", "description", "Arque Structures", "Utilities")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("633109A0-8166-4C00-BDDE-21021F410E2D"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Truss type", "tt", "tt", GH_ParamAccess.item, "Warren");
            pManager.AddIntegerParameter("Typology", "t", "t", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Width", "w", "w", GH_ParamAccess.item, 11);
            pManager.AddNumberParameter("Height", "h", "h", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("Max height", "mh", "mh", GH_ParamAccess.item, 4);
            pManager.AddNumberParameter("Clear height", "ch", "ch", GH_ParamAccess.item, 2.5);
            pManager.AddIntegerParameter("Base type", "bt", "bt", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Articulation type", "at", "at", GH_ParamAccess.item, "Rigid");
            pManager.AddIntegerParameter("Divisions", "d", "d", GH_ParamAccess.item, 5);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("n", "n", "n", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussType = "";
            var typology = 0;
            var width = 0;
            var height = 0;
            var maxHeight = 0;
            var clearHeight = 0;
            var baseType = 0;
            var articulationType = "";
            var divisions = 0;
            if (!DA.GetData(0, ref trussType)) return;
            if (!DA.GetData(1, ref typology)) return;
            if (!DA.GetData(2, ref width)) return;
            if (!DA.GetData(3, ref height)) return;
            if (!DA.GetData(4, ref maxHeight)) return;
            if (!DA.GetData(5, ref clearHeight)) return;
            if (!DA.GetData(6, ref baseType)) return;
            if (!DA.GetData(7, ref articulationType)) return;
            if (!DA.GetData(8, ref divisions)) return;
            var truss = new ArchTruss(Plane.WorldXY, width, height, maxHeight, clearHeight, divisions, trussType,
                articulationType, baseType, 2);
            var options = new TrussOptions();
            DA.SetData(0, 0);
        }
    }
}