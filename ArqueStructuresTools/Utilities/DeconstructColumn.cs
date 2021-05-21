using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace ArqueStructuresTools
{
    public class DeconstructColumn : GH_Component
    {
        public DeconstructColumn() : base("Deconstruct Column", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid => new Guid("ACD0033D-26F3-4018-A8A9-8027EBDD6CEF");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ColumnParameter(), "Column", "c", "column to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "a", "Axis of the column", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "p", "The columns' profile orientation's plane", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var columnGoo = new ColumnGoo();

            if (!DA.GetData(0, ref columnGoo)) return;

            DA.SetData(0, columnGoo.Value.Axis);
            DA.SetData(1, columnGoo.Value.ProfileOrientitionPlane);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
    }
}