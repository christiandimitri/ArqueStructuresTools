namespace WarehouseLib.Profiles
{
    public class WarehouseProfiles
    {
        public Profile StaticColumnProfile;
        public Profile BoundaryColumnProfile;

        public WarehouseProfiles(Profile staticColumnsProfile, Profile boundaryColumnProfile)
        {
            StaticColumnProfile = staticColumnsProfile;
            BoundaryColumnProfile = boundaryColumnProfile;
        }
    }
}