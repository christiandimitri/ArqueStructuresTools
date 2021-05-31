using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace ArqueStructuresTools
{
    public class DeconstructBeam : GH_Component
    {
        public DeconstructBeam() : base("Deconstruct Beam", "Nickname", "Description", "Arque Structures",
            "Utilities")
        {
        }

        public override Guid ComponentGuid => new Guid("1D7C237D-8CEE-4D93-A57D-CD1216FACCBF");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BeamParameter(), "Beam", "b", "column to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Axis", "a", "Axis of the column", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "p", "The columns' profile orientation's plane", GH_ParamAccess.item);
            pManager.AddTextParameter("Profile", "p", "The columns' profile reference name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            BeamGoo beamGoo = new BeamGoo();

            if (!DA.GetData(0, ref beamGoo)) return;

            DA.SetDataList(0, beamGoo.Value.Axis);
            DA.SetData(1, beamGoo.Value.ProfileOrientationPlane);
            DA.SetData(2, beamGoo.Value.Profile.Name);
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