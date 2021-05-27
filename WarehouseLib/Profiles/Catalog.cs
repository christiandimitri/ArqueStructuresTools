using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseLib.Profiles
{
    public class Catalog
    {
        public List<(string, double, double, double)> IPE;
        public List<(string, double, double, double)> IPN;
        public List<(string, double, double, double)> HEA;
        public List<(string, double, double, double)> HEB;
        public List<(string, double, double, double)> HEM;
        public List<(string, double, double, double)> HEAA;
        public List<(string, double, double, double)> UPA;
        public List<(string, double, double, double)> UPN;
        public List<(string, double, double, double)> UPE;
        public List<(string, double, double, double)> EqualL;
        public List<(string, double, double, double)> UnequalL;
        public List<(string, double, double, double)> T;
        public List<(string, double, double, double)> TB;

        public Catalog()
        {
            FetchProfilesCatalog();
        }

        private void FetchProfilesCatalog()
        {
            foreach (var section in Enum.GetValues(typeof(ProfileTypes)))
            {
                GetProfileCatalog(section.ToString());
            }
        }

        private void GetProfileCatalog(string section)
        {
            var profilesCsv = GetCsvFile(section).ToList();

            var catalogTuplesList = new List<(string, double, double, double)>();


            for (int i = 1; i < profilesCsv.Count; i++)
            {
                var profile = profilesCsv[i];
                var sectionProperties = profile.Split(',');
                catalogTuplesList.Add((sectionProperties[0], double.Parse(sectionProperties[1]),
                    double.Parse(sectionProperties[2]),
                    double.Parse(sectionProperties[3])));
            }

            SetProfileProperties(section, catalogTuplesList);
        }

        private void SetProfileProperties(string sectionFamily,
            List<(string, double, double, double)> catalogTuplesList)
        {
            switch (sectionFamily)
            {
                case "IPE":
                    IPE = catalogTuplesList;
                    break;
                case "IPN":
                    IPN = catalogTuplesList;
                    break;
                case "L_equal":
                    EqualL = catalogTuplesList;
                    break;
                case "L_unequal":
                    UnequalL = catalogTuplesList;
                    break;
                case "HEA":
                    HEA = catalogTuplesList;
                    break;
                case "HEAA":
                    HEAA = catalogTuplesList;
                    break;
                case "HEB":
                    HEB = catalogTuplesList;
                    break;
                case "HEM":
                    HEM = catalogTuplesList;
                    break;
                case "T":
                    T = catalogTuplesList;
                    break;
                case "TB":
                    TB = catalogTuplesList;
                    break;
                case "UPA":
                    UPA = catalogTuplesList;
                    break;
                case "UPE":
                    UPE = catalogTuplesList;
                    break;
                case "UPN":
                    UPN = catalogTuplesList;
                    break;
            }
        }

        private string[] GetCsvFile(string sectionFamily)
        {
            var profilesCsv = new string[] { };
            switch (sectionFamily)
            {
                case "IPE":
                    profilesCsv = Properties.Resources.steel_profile_calatog_IPE.Split('\n');
                    break;
                case "IPN":
                    profilesCsv = Properties.Resources.steel_profile_calatog_IPN.Split('\n');
                    break;
                case "L_equal":
                    profilesCsv = Properties.Resources.steel_profile_calatog_L_equal.Split('\n');
                    break;
                case "L_unequal":
                    profilesCsv = Properties.Resources.steel_profile_calatog_L_unequal.Split('\n');
                    break;
                case "HEA":
                    profilesCsv = Properties.Resources.steel_profile_calatog_HEA.Split('\n');
                    break;
                case "HEAA":
                    profilesCsv = Properties.Resources.steel_profile_calatog_HEAA.Split('\n');
                    break;
                case "HEB":
                    profilesCsv = Properties.Resources.steel_profile_calatog_HEB.Split('\n');
                    break;
                case "HEM":
                    profilesCsv = Properties.Resources.steel_profile_calatog_HEM.Split('\n');
                    break;
                case "T":
                    profilesCsv = Properties.Resources.steel_profile_calatog_T.Split('\n');
                    break;
                case "TB":
                    profilesCsv = Properties.Resources.steel_profile_calatog_TB.Split('\n');
                    break;
                case "UPA":
                    profilesCsv = Properties.Resources.steel_profile_calatog_UPA.Split('\n');
                    break;
                case "UPE":
                    profilesCsv = Properties.Resources.steel_profile_calatog_UPE.Split('\n');
                    break;
                case "UPN":
                    profilesCsv = Properties.Resources.steel_profile_calatog_UPN.Split('\n');
                    break;
            }

            return profilesCsv;
        }
    }
}