using System;
using Grasshopper.Kernel;

namespace ArqueStructuresTools.Bracings
{
    public class RoofBracings : GH_Component
    {
        public RoofBracings() : base("Roof Bracings", "Nickname", "Description", "Arque Structures", "Bracings")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("498401B2-CCA1-44A2-A654-D2D60AF5A6D6"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            throw new NotImplementedException();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            throw new NotImplementedException();
        }
    }
}