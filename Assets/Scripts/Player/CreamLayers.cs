namespace MarioPie.Player
{
    public sealed class CreamLayers
    {
        public CreamLayers(int max)
        {
            Max = max < 1 ? 1 : max;
        }

        public int Max { get; }
        public int Count { get; private set; }

        public void Add()
        {
            if (Count < Max)
                Count++;
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}
