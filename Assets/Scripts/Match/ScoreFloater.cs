using UnityEngine;
using UnityEngine.UI;

namespace MarioPie.Match
{
    public sealed class ScoreFloater : MonoBehaviour
    {
        [SerializeField] float rise = 124f;
        [SerializeField] float life = 0.8f;

        RectTransform rect;
        Text label;
        Vector2 origin;
        float age;
        float tilt;
        bool armed;

        void Awake()
        {
            rect = transform as RectTransform;
            label = GetComponent<Text>();
            tilt = Random.Range(-8f, 8f);
            if (rect != null)
                rect.localScale = new Vector3(0.2f, 0.2f, 1f);
        }

        void Update()
        {
            if (rect == null)
            {
                Destroy(gameObject);
                return;
            }

            if (!armed)
            {
                origin = rect.anchoredPosition;
                armed = true;
            }

            age += Time.deltaTime;
            var t = life <= 0.01f ? 1f : Mathf.Clamp01(age / life);
            var pop = Mathf.Clamp01(age / 0.16f);
            var scale = pop < 1f
                ? Mathf.LerpUnclamped(0.15f, 1.35f, EaseOutBack(pop))
                : Mathf.Lerp(1.35f, 1f, Mathf.Clamp01((age - 0.16f) / 0.18f));
            var lift = rise * (1f - (1f - t) * (1f - t));
            rect.anchoredPosition = origin + new Vector2(0f, lift);
            rect.localScale = new Vector3(scale, scale, 1f);
            rect.localEulerAngles = new Vector3(0f, 0f, tilt * (1f - t));

            if (label != null)
            {
                var color = label.color;
                color.a = t < 0.55f ? 1f : 1f - (t - 0.55f) / 0.45f;
                label.color = color;
            }

            if (t >= 1f)
                Destroy(gameObject);
        }

        static float EaseOutBack(float u)
        {
            const float c1 = 1.75f;
            var p = u - 1f;
            return 1f + (c1 + 1f) * p * p * p + c1 * p * p;
        }
    }
}
