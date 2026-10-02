using UnityEngine;

namespace MarioPie
{
    public sealed class PieSupply : MonoBehaviour
    {
        public int Side { get; private set; }
        public bool Available => remaining <= 0f;
        public Vector3 Position => transform.position;

        Renderer[] visuals;
        float respawnSeconds = 2.4f;
        float grabRadius = 1.5f;
        float remaining;

        public void Configure(int side, float respawn, float radius, Renderer[] renderers)
        {
            Side = side;
            respawnSeconds = respawn < 0.05f ? 0.05f : respawn;
            grabRadius = radius;
            visuals = renderers;
            remaining = 0f;
            SetVisible(true);
        }

        public bool TryTake()
        {
            if (!Available)
                return false;

            remaining = respawnSeconds;
            SetVisible(false);
            return true;
        }

        void Update()
        {
            if (remaining <= 0f)
                return;

            remaining -= Time.deltaTime;
            if (remaining > 0f)
                return;

            remaining = 0f;
            SetVisible(true);
        }

        void SetVisible(bool visible)
        {
            if (visuals == null)
                return;

            for (var i = 0; i < visuals.Length; i++)
            {
                if (visuals[i] != null)
                    visuals[i].enabled = visible;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Available
                ? new Color(1f, 0.95f, 0.75f, 0.9f)
                : new Color(0.45f, 0.45f, 0.45f, 0.45f);
            var center = transform.position;
            center.y = 0.15f;
            Gizmos.DrawWireSphere(center, grabRadius);
        }
    }
}
