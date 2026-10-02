using UnityEngine;

namespace MarioPie.Player
{
    public sealed class CreamBurst : MonoBehaviour
    {
        const float Life = 0.28f;

        float remaining = Life;
        Vector3 fromScale;
        Vector3 toScale;

        public static void Spawn(Vector3 position, GameObject prefab)
        {
            var blob = PieProps.Spawn(prefab, null);
            if (blob == null)
                return;

            blob.name = "CreamBurst";
            blob.transform.position = position;
            var burst = blob.AddComponent<CreamBurst>();
            burst.toScale = blob.transform.localScale;
            burst.fromScale = burst.toScale * 0.4f;
            blob.transform.localScale = burst.fromScale;
        }

        void Update()
        {
            remaining -= Time.deltaTime;
            var t = 1f - Mathf.Clamp01(remaining / Life);
            transform.localScale = Vector3.Lerp(fromScale, toScale, t);
            if (remaining <= 0f)
                Destroy(gameObject);
        }
    }
}
