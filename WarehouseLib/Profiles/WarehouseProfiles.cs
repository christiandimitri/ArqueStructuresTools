
namespace WarehouseLib.Profiles
{
    public class WarehouseProfiles
    {
        public string StaticColumnProfileName;
        public string BoundaryColumnProfileName;

        public WarehouseProfiles(string staticColumnsProfileName, string boundaryColumnProfileName)
        {
            StaticColumnProfileName = staticColumnsProfileName;
            BoundaryColumnProfileName = boundaryColumnProfileName;
            var profilesCatalog = new Catalog();
        }
    }
}