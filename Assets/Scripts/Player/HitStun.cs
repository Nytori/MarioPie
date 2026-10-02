namespace MarioPie.Player
{
    public sealed class HitStun
    {
        public bool Active => remaining > 0f;
        public float Remaining => remaining;

        float remaining;

        public void Apply(float seconds)
        {
            if (seconds > remaining)
                remaining = seconds;
        }

        public void Tick(float dt)
        {
            if (dt <= 0f || remaining <= 0f)
                return;

            remaining -= dt;
            if (remaining < 0f)
                remaining = 0f;
        }
    }
}
