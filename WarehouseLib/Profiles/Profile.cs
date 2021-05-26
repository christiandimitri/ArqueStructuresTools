namespace WarehouseLib.Profiles
{
    public abstract class Profile
    {
        public string Name;

        public double Height;

        public double Width;
        
        protected Profile ()
        {
            // Name = name;
        }

        public abstract double ReturnProfileHeight(string name);

    }
}