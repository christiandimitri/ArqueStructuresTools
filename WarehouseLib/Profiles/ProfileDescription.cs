namespace WarehouseLib.Profiles
{
    public struct ProfileDescription
    {
        public ProfileDescription(string name, double height, double width)
        {
            Name = name;
            Height = height;
            Width = width;
        }

        public string Name { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }
    }
}