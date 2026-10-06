using UnityEngine;

namespace MarioPie.Match
{
    [DisallowMultipleComponent]
    public sealed class MatchCamera : MonoBehaviour
    {
        [SerializeField] Camera view;
        [SerializeField] CameraAnchor intro;
        [SerializeField] CameraAnchor play;
        [SerializeField] CameraAnchor winLeft;
        [SerializeField] CameraAnchor winRight;

        bool warned;

        void Awake()
        {
            if (view == null)
                view = GetComponent<Camera>();
        }

        public void Present(MatchPhase phase, float blend, int winner)
        {
            if (phase == MatchPhase.Intro)
            {
                Blend(intro != null ? intro : play, play, blend);
                return;
            }

            if (phase == MatchPhase.Result)
            {
                var target = winner == 0 ? winLeft : winner == 1 ? winRight : play;
                Blend(play, target != null ? target : play, blend);
                return;
            }

            Blend(play, play, 1f);
        }

        void Blend(CameraAnchor from, CameraAnchor to, float t)
        {
            if (view == null)
                view = GetComponent<Camera>();

            var start = from != null ? from : play;
            var end = to != null ? to : start;
            if (view == null || start == null || end == null)
            {
                Warn();
                return;
            }

            var weight = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            view.transform.SetPositionAndRotation(
                Vector3.Lerp(start.transform.position, end.transform.position, weight),
                Quaternion.Slerp(start.transform.rotation, end.transform.rotation, weight));
            view.fieldOfView = Mathf.Lerp(start.fieldOfView, end.fieldOfView, weight);
        }

        void Warn()
        {
            if (warned)
                return;

            warned = true;
            Debug.LogWarning("Falta uma âncora de câmara. O enquadramento de jogo mantém-se.", this);
        }
    }
}
