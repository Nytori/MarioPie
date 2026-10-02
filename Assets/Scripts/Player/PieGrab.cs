using UnityEngine;

namespace MarioPie.Player
{
    public readonly struct PieSlot
    {
        public PieSlot(int side, bool available, Vector3 position)
        {
            Side = side;
            Available = available;
            Position = position;
        }

        public int Side { get; }
        public bool Available { get; }
        public Vector3 Position { get; }
    }

    public static class PieGrab
    {
        public static int Select(PieSlot[] slots, Vector3 player, int side, float radius)
        {
            if (slots == null || radius <= 0f)
                return -1;

            var best = -1;
            var bestSqr = 0f;
            var limit = radius * radius;
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (!slot.Available || slot.Side != side)
                    continue;

                var dx = player.x - slot.Position.x;
                var dz = player.z - slot.Position.z;
                var sqr = dx * dx + dz * dz;
                if (sqr > limit)
                    continue;
                if (best >= 0 && sqr >= bestSqr)
                    continue;

                best = i;
                bestSqr = sqr;
            }

            return best;
        }
    }
}
