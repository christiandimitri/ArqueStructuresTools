using System;
using Grasshopper.Kernel;
using WarehouseLib.Connections;
using WarehouseLib.Options;


namespace ArqueStructuresTools.Options
{
    public class TrussInputs : GH_Component
    {
        public TrussInputs() : base("Truss Inputs", "Nickname", "description", "Arque Structures",
            "Inputs")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("633109A0-8166-4C00-BDDE-21021F410E2D"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Truss type", "Truss type",
                "The input is a text, the available types are: 0 for Pratt, 1 for Howe, 2 for Warren, 3 for WarrenStuds",
                GH_ParamAccess.item,
                0);
            pManager.AddNumberParameter("Width", "w", "w", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Height", "h", "h", GH_ParamAccess.item, 3.0);
            pManager.AddNumberParameter("Max height", "mh", "mh", GH_ParamAccess.item, 4.0);
            pManager.AddNumberParameter("Clear height", "ch", "ch", GH_ParamAccess.item, 2.5);
            pManager.AddIntegerParameter("Base type", "bt", "bt", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Articulation type", "at", "at", GH_ParamAccess.item, "Rigid");
            pManager.AddIntegerParameter("Divisions", "d", "d", GH_ParamAccess.item, 5);
            pManager.AddIntegerParameter("Portico type", "pt", "0 for Portico, 1 for Truss", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Columns count", "cc", "cc", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Facade straps distance", "fsd", "fsd", GH_ParamAccess.item, 1.2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Truss options", "n", "n", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trussType = 0;
            double width = 0;
            double height = 0;
            double maxHeight = 0;
            double clearHeight = 0;
            var baseType = 0;
            var articulationType = "";
            var divisions = 0;
            var porticoType = 1;
            var columnsCount = 0;
            var facadeStrapsDistance = 0.0;
            if (!DA.GetData(0, ref trussType)) return;
            if (!DA.GetData(1, ref width)) return;
            if (!DA.GetData(2, ref height)) return;
            if (!DA.GetData(3, ref maxHeight)) return;
            if (!DA.GetData(4, ref clearHeight)) return;
            if (!DA.GetData(5, ref baseType)) return;
            if (!DA.GetData(6, ref articulationType)) return;
            if (!DA.GetData(7, ref divisions)) return;
            if (!DA.GetData(8, ref porticoType)) return;
            if (!DA.GetData(9, ref columnsCount)) return;
            if (!DA.GetData(10, ref facadeStrapsDistance)) return;


            WarehouseLib.Options.TrussInputs inputs;
            try
            {
                inputs = new WarehouseLib.Options.TrussInputs((ConnectionType) trussType, width, height, maxHeight,
                    clearHeight, baseType,
                    articulationType, divisions, (PorticoType) porticoType, columnsCount, facadeStrapsDistance);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
                return;
            }


            DA.SetData(0, inputs);
        }
    }
}