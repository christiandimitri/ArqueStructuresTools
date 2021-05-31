using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using WarehouseLib.Utilities;

namespace WarehouseLib.Profiles
{
    public class Catalog
    {
        public Catalog()
        {
        }

        public Dictionary<string, ProfileDescription> GetCatalog()
        {
            var profiles = new Dictionary<string, ProfileDescription>();
            var profilesCsv = GetCsvFile();
            for (int i = 1; i < profilesCsv.Count; i++)
            {
                var profile = profilesCsv[i];

                var properties = profile.Split(',');
                Debug.WriteLine(properties[0]);
                profiles.Add(properties[0],
                    ExtractProfileDescription(properties[0], double.Parse(properties[1]), double.Parse(properties[2])));
            }

            return profiles;
        }

        private ProfileDescription ExtractProfileDescription(string name, double height, double width)
        {
            if (name == String.Empty)
            {
                name = "";
            }

            if (height == Double.NaN)
            {
                height= 0;
            }

            if (width == Double.NaN)
            {
                width = 0;
            }
            var description = new ProfileDescription(name, height, width);
            return description;
        }

        private string TrimWhiteSpaceFromName(string name)
        {
            var newName = new TrimWhiteSpaceFromString(name).TrimmedString;
            return newName;
        }

        private List<string> GetCsvFile()
        {
            var profilesCsv = Properties.Resources.steel_profile_calatog.Split('\n').ToList();
            var tempList = new List<string>();
            foreach (var profile in profilesCsv)
            {
                tempList.Add(TrimWhiteSpaceFromName(profile));
            }

            profilesCsv = new List<string>(tempList);
            return profilesCsv;
        }
    }
}