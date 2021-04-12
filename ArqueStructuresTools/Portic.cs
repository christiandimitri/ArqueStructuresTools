using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ArqueStructuresTools
{
    public class Portic : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Portic()
          : base("Portic/Truss", "Pr",
              "Description",
              "Arque Structures", "Portics")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddPlaneParameter("plane", "pl", "Starting base plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("typology", "ty", "(0) Straight/Flat (1) Arc (2) Single inclination (3) Double inclination", GH_ParamAccess.item, 3);
            pManager.AddIntegerParameter("base type", "bT", "(0) Straight base (1) Offset base", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("support type", "sT", "(0) Articulated supports (1) Rigid", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("span left", "sL", "description", GH_ParamAccess.item, 5000);
            pManager.AddIntegerParameter("span right", "sR", "description", GH_ParamAccess.item, 5000);
            pManager.AddIntegerParameter("max height", "mH", "description", GH_ParamAccess.item, 3500);
            pManager.AddIntegerParameter("clear height", "cH", "description", GH_ParamAccess.item, 2700);
            pManager.AddIntegerParameter("left height", "lH", "description", GH_ParamAccess.item, 3000);
            pManager.AddIntegerParameter("right height", "rH", "description", GH_ParamAccess.item, 3000);
            pManager.AddIntegerParameter("subdivision count", "sC", "description", GH_ParamAccess.item, 4);
            pManager.AddIntegerParameter("min length", "miL", "description", GH_ParamAccess.item, 1500);
            pManager.AddIntegerParameter("max length", "maL", "description", GH_ParamAccess.item, 2000);
            pManager.AddIntegerParameter("portic type", "pT", "description", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("truss type", "tT", "description", GH_ParamAccess.item, 0);
            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddPointParameter("upper points", "uP", "upper base points", GH_ParamAccess.list);
            pManager.AddCurveParameter("upper curves", "uC", "upper curves points", GH_ParamAccess.list);
            pManager.AddPointParameter("lower points", "lP", "lower base points", GH_ParamAccess.list);
            pManager.AddVectorParameter("lower vectors", "lV", "lower normal vectors", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            int typology = 3;
            int baseType = 0;
            int supportType = 1;
            int leftSpan = 5000;
            int rightSpan = 5000;
            int maximumHeight = 3500;
            int clearHeight = 2700;
            int leftHeight = 3000;
            int rightHeight = 3000;
            int subdivisionCount = 4;
            int minimumLength = 1500;
            int maximumLength = 2000;
            int porticType = 0;
            int trussType = 0;
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref typology)) return;
            if (!DA.GetData(2, ref baseType)) return;
            if (!DA.GetData(3, ref supportType)) return;
            if (!DA.GetData(4, ref leftSpan)) return;
            if (!DA.GetData(5, ref rightSpan)) return;
            if (!DA.GetData(6, ref maximumHeight)) return;
            if (!DA.GetData(7, ref clearHeight)) return;
            if (!DA.GetData(8, ref leftHeight)) return;
            if (!DA.GetData(9, ref rightHeight)) return;
            if (!DA.GetData(10, ref subdivisionCount)) return;
            if (!DA.GetData(11, ref minimumLength)) return;
            if (!DA.GetData(12, ref maximumLength)) return;
            if (!DA.GetData(13, ref porticType)) return;
            if (!DA.GetData(14, ref trussType)) return;
            // upper elements
            List<Point3d> upperBasePoints = new List<Point3d>();
            List<Curve> upperBaseCurves = new List<Curve>();

            // lower elements
            List<Point3d> lowerBasePoints = new List<Point3d>();
            List<Curve> lowerBaseCurves = new List<Curve>();

            // get heights interval
            int[] heights = Utilities.Heights.HeightsInterval.Interval(leftHeight, rightHeight);
            // min height and index for selected corner
            int min = Utilities.Heights.HeightsInterval.Min(heights);
            int indexOfMin = Utilities.Heights.HeightsInterval.IndexOfMin(heights, min);

            // max height and index for not selected corner
            int max = Utilities.Heights.HeightsInterval.Max(heights);
            int indexOfMax = Utilities.Heights.HeightsInterval.IndexOfMax(heights, max);

            // initilize difference
            int difference= new int();
            List<Vector3d> firstLastNormalVectors = new List<Vector3d>();
            // if typology range skipped, throw warning
            if(typology > 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Topology type selection should be between 0 and 3");
                //Restrict maximum height to columns maximum height
                Utilities.Restrictions.MaxHeight.Recompute(ref maximumHeight, max, 0);

            }

            // straight typology
            if (typology == 0)
            {

                // compute difference between clear height and maximum
                int tempDifference = Straight.Heights.ClearHeight.ComputeDifference(maximumHeight,ref clearHeight, min);
                difference = tempDifference;
                
                //draw upper base points 
                List<Point3d> straightPoints = Straight.StraightPoints.UpperBasePoints(plane, leftSpan, maximumHeight);
                upperBasePoints = straightPoints;

                //draw upper base curves
                List<Curve> straightBaseCurves = Straight.StraightCurves.UpperBaseCurves(straightPoints);
                upperBaseCurves = straightBaseCurves;

                // compute first last normal vectors 
                List<Vector3d> tempNormals = Straight.Vectors.FirstLastNormals.Get(straightBaseCurves);
                firstLastNormalVectors = tempNormals;

            }

            // arch typology
            else if (typology == 1)
            {
                // warning maximum height >= columns maximum height
                if (maximumHeight <= max)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Maximum height should be > left/right heights ");
                    //Restrict maximum height to columns maximum height
                    Utilities.Restrictions.MaxHeight.Recompute(ref maximumHeight, max, 200);

                }

                else if (maximumHeight > leftSpan / 2)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Maximum height should be <= (leftSpan/2) ");
                    maximumHeight = leftSpan / 2;
                }


                // compute difference between clear height and maximum
                int tempDifference = Arch.Heights.ClearHeight.ComputeDifference(ref clearHeight, min);
                difference = tempDifference;

                //draw upper base points 
                List<Point3d> archPoints = Arch.ArchPoints.UpperBasePoints(plane, leftSpan, maximumHeight, ref leftHeight, ref rightHeight);
                upperBasePoints = archPoints;

                //draw upper base curves
                List<Curve> archBaseCurves = Arch.ArchCurves.UpperBaseCurves(archPoints);
                upperBaseCurves = archBaseCurves;

                // compute first last normal vectors 
                List<Vector3d> tempNormals = Arch.Vectors.FirstLastNormals.Get(archBaseCurves);
                firstLastNormalVectors = tempNormals;

            }

            // monopich typology
            else if(typology == 2)
            {
                // compute difference between clear height and maximum
                int tempDifference = Monopich.Heights.ClearHeight.ComputeDifference(ref clearHeight, min);
                difference = tempDifference;
                //draw upper base points 
                List<Point3d> monopichPoints = Monopich.MonopichPoints.UpperBasePoints(plane, leftSpan, ref leftHeight,ref rightHeight);
                upperBasePoints = monopichPoints;

                //draw upper base curves
                List<Curve> monopichBaseCurves = Monopich.MonopichCurves.UpperBaseCurves(monopichPoints);
                upperBaseCurves = monopichBaseCurves;

                // compute first last normal vectors 
                List<Vector3d> tempNormals = Monopich.Vectors.FirstLastNormals.Get(monopichBaseCurves);
                firstLastNormalVectors = tempNormals;


            }

            // duopich typology
            else if(typology == 3)
            {
                // warning maximum height >= columns maximum height
                if (maximumHeight <= max)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Maximum height should be > left/right heights ");
                    //Restrict maximum height to columns maximum height
                    Utilities.Restrictions.MaxHeight.Recompute(ref maximumHeight, max, 0);

                }


                // compute difference between clear height and maximum
                int tempDifference = Duopich.Heights.ClearHeight.ComputeDifference(ref clearHeight, min);
                difference = tempDifference;

                //draw upper base points 
                List<Point3d> duopichPoints = Duopich.DuopichPoints.UpperBasePoints(plane, leftSpan, rightSpan, maximumHeight, ref leftHeight, ref rightHeight);
                upperBasePoints = duopichPoints;

                //draw upper base curves
                List<Curve> duopichBaseCurves = Duopich.DuopichCurves.UpperBaseCurves(duopichPoints);
                upperBaseCurves = duopichBaseCurves;

                // compute first last normal vectors 
                List<Vector3d> tempNormals = Duopich.Vectors.FirstLastNormals.Get(duopichBaseCurves);
                firstLastNormalVectors = tempNormals;

            }

            // ====== generate inferior truss points & curves ======= //

            // compute the offset variable
            double offsetFactor = Utilities.Compute.Offset.Compute(indexOfMin, difference, upperBaseCurves[indexOfMin]);

            // compute and store the normal vectors at each point and store globally
            List<Vector3d> sideVectors = new List<Vector3d>();
            sideVectors.Add(firstLastNormalVectors[0]);
            sideVectors.Add(firstLastNormalVectors[2]);
            firstLastNormalVectors[1].Rotate(-0.5 * Math.PI, Vector3d.YAxis);

            List<Vector3d> tempN = firstLastNormalVectors;
            // compute highest corner and middle hypothenus
            double middleHypothenus = Utilities.Compute.Hypothenus.Center(offsetFactor, upperBaseCurves[indexOfMin], firstLastNormalVectors[1]);
            double cornerHypothenus = Utilities.Compute.Hypothenus.Corner(indexOfMax, offsetFactor, upperBaseCurves[indexOfMax]);

            // store points at normal vectors at each point
            if (typology == 0) lowerBasePoints = Straight.StraightPoints.ThickBasePoints(firstLastNormalVectors, offsetFactor, upperBasePoints);
            else if (typology == 1) lowerBasePoints = Arch.ArchPoints.ThickBasePoints(firstLastNormalVectors, offsetFactor, upperBasePoints, middleHypothenus);
            else if (typology == 2) lowerBasePoints = Monopich.MonopichPoints.ThickBasePoints(firstLastNormalVectors, offsetFactor, upperBasePoints, middleHypothenus);
            else if (typology == 3) lowerBasePoints = Duopich.DuopichPoints.ThickBasePoints(firstLastNormalVectors, offsetFactor, upperBasePoints, middleHypothenus);

            // on each side create the clear point at its corresponding clear height
            //for (int i = 0; i < sideVectors.Count; i++)
            //{
            //    Point3d tempPt = new Point3d(i != indexOfMin ? -cornerHypothenus * Vector3d.ZAxis + upperBasePoints[i == 1 ? 2 : 0] : -difference * Vector3d.ZAxis + upperBasePoints[i == 1 ? 2 : 0]);
            //    lowerBasePoints.Insert(i == 0 ? 0 : 4, tempPt);
            //}



            // output to grasshopper

            DA.SetDataList(0, upperBasePoints);
            DA.SetDataList(1, upperBaseCurves);
            DA.SetDataList(2, lowerBasePoints);

            DA.SetDataList(3, tempN);
        }

        

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("967e44ae-d6a3-4d84-b116-3e592554f49e"); }
        }
    }
}
