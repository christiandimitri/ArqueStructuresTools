namespace WarehouseLib.BucklingLengths
{
    public struct BucklingLengths
    {
        public double BucklingY { get; set; }
        public double BucklingZ { get; set; }
        
        public  BucklingLengths (double bucklingY, double bucklingZ)
        {
            BucklingY = bucklingY;
            BucklingZ = bucklingZ;
        }
    }
}