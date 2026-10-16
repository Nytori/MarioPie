using UnityEngine;

namespace MarioPie
{
    public sealed class SfxHost : MonoBehaviour
    {
        void Update()
        {
            var paused = Time.timeScale <= 0.0001f;
            if (AudioListener.pause != paused)
                AudioListener.pause = paused;
        }

        void OnDestroy()
        {
            if (AudioListener.pause)
                AudioListener.pause = false;
        }
    }
}
