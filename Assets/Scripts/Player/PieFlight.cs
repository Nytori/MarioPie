namespace MarioPie.Player
{
    public sealed class PieFlight
    {
        public int Count { get; private set; }

        public void Begin()
        {
            Count++;
        }

        public void End()
        {
            if (Count > 0)
                Count--;
        }
    }
}
