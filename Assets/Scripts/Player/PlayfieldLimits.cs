namespace MarioPie.Player
{
    public readonly struct PlayfieldLimits
    {
        public PlayfieldLimits(float minX, float maxX, float minZ, float maxZ, float centerGap)
        {
            MinX = minX;
            MaxX = maxX;
            MinZ = minZ;
            MaxZ = maxZ;
            CenterGap = centerGap;
        }

        public float MinX { get; }
        public float MaxX { get; }
        public float MinZ { get; }
        public float MaxZ { get; }
        public float CenterGap { get; }
    }
}
