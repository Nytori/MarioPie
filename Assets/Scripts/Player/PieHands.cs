namespace MarioPie.Player
{
    public sealed class PieHands
    {
        public PieHands(int capacity)
        {
            Capacity = capacity < 1 ? 1 : capacity;
        }

        public int Capacity { get; }
        public int Held { get; private set; }
        public bool IsFull => Held >= Capacity;

        public bool TryTake()
        {
            if (IsFull)
                return false;

            Held++;
            return true;
        }

        public void Clear()
        {
            Held = 0;
        }

        public bool TrySpend()
        {
            if (Held <= 0)
                return false;

            Held--;
            return true;
        }
    }
}
