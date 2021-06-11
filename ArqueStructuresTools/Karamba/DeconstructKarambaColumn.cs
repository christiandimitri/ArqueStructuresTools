using System;
using Grasshopper.Kernel;

namespace ArqueStructuresTools.Karamba
{
    public class DeconstructKarambaColumn : GH_Component
    {
        public DeconstructKarambaColumn() : base("Deconstruct Karamba3D column", "DeKaColumn",
            "Deconstruct a Column into its karamba parts inputs", "Arque Structures", "Column")
        {
        }

        public override Guid ComponentGuid => new Guid("C31CD4F3-2397-4A73-8935-D6EC835746ED");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ColumnParameter(), "Column", "Col", "Column to deconstruct",
                GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length multiplier", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length multiplier", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "A", "Column axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("Y Buckling", "YB", "Y buckling length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Z Buckling", "ZB", "Z buckling length", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var columnGoo = new ColumnGoo();
            var bucklingYMultiplier = 1.0;
            var bucklingZMultiplier = 1.0;
            // get input data
            if (!DA.GetData(0, ref columnGoo)) return;
            if (!DA.GetData(1, ref bucklingYMultiplier)) return;
            if (!DA.GetData(2, ref bucklingZMultiplier)) return;
            
            columnGoo.Value.BucklingLengths.BucklingY *= bucklingYMultiplier;
            columnGoo.Value.BucklingLengths.BucklingZ *= bucklingZMultiplier;
            
            // set output data 
            DA.SetData(0, columnGoo.Value.Axis);
            DA.SetData(1, columnGoo.Value.BucklingLengths.BucklingY);
            DA.SetData(2, columnGoo.Value.BucklingLengths.BucklingZ);
        }
    }
}