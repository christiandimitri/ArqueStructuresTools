using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WarehouseLib;

namespace ArqueStructuresTools
{
    public class DeckStrapsComponent : GH_Component
    {
        public DeckStrapsComponent() : base("DeckStrapsComponent", "Nickname", "Description", "Arque Structures",
            "Straps")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("A4DCA5FC-E06B-4F13-A17A-79D5B31EB444"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TrussParameter(), "", "", "", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StrapParameter());
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

            var deckStraps = new List<StrapGoo>();
            var tempStraps = new List<Strap>(GenerateDeckStraps(trusses));
            foreach (var strap in tempStraps)
            {
                deckStraps.Add(new StrapGoo(strap));
            }
            DA.SetData(0,new List<StrapGoo>(deckStraps));
        }

        public List<Strap> GenerateDeckStraps(List<Truss> trusses)
        {
            var deckStraps = new List<Strap>();

            for (var i = 0; i < trusses.Count; i++)
            {
                for (int j = 0; j < trusses[i].TopNodes.Count; j++)
                {
                    if (i < trusses.Count - 1)
                    {
                        Point3d ptA = trusses[i].TopNodes[j];
                        Point3d ptB = trusses[i + 1].TopNodes[j];
                        Line axis = new Line(ptA, ptB);
                        deckStraps.Add(new Strap(axis));
                    }
                }
            }

            return deckStraps;
        }
    }
}