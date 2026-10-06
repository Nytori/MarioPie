using UnityEngine;

namespace MarioPie.Player
{
    public sealed class PlayerCeremony : MonoBehaviour
    {
        public enum Pose
        {
            None,
            Ready,
            Win,
            Lose,
            Tie
        }

        Transform body;
        Animator animator;
        Vector3 restPosition;
        Vector3 restScale;
        Pose pose;
        bool animatorOwns;
        float bob = 0.08f;
        float winHop = 0.35f;
        float tieHop = 0.16f;
        float squash = 0.72f;
        float time;

        public void Setup(Transform visual, float readyBob, float winHopHeight, float tieHopHeight, float loseSquash)
        {
            body = visual;
            animator = GetComponent<Animator>();
            bob = readyBob;
            winHop = winHopHeight;
            tieHop = tieHopHeight;
            squash = loseSquash;
            if (body == null)
                return;

            restPosition = body.localPosition;
            restScale = body.localScale;
        }

        public void Show(Pose next)
        {
            if (pose == next)
                return;

            pose = next;
            time = 0f;
            animatorOwns = Fire(next);
            Apply();
        }

        void LateUpdate()
        {
            if (pose == Pose.None || animatorOwns)
                return;

            time += Time.deltaTime;
            Apply();
        }

        bool Fire(Pose next)
        {
            if (animator == null || animator.runtimeAnimatorController == null || next == Pose.None)
                return false;

            var trigger = next.ToString();
            if (!HasTrigger(trigger))
                return false;

            animator.SetTrigger(trigger);
            return true;
        }

        bool HasTrigger(string trigger)
        {
            var parameters = animator.parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == trigger)
                    return true;
            }

            return false;
        }

        void Apply()
        {
            if (body == null || animatorOwns)
                return;

            switch (pose)
            {
                case Pose.Ready:
                    body.localPosition = restPosition + Vector3.up * (Mathf.Sin(time * 3.5f) * bob);
                    body.localScale = restScale;
                    break;
                case Pose.Win:
                    body.localPosition = restPosition + Vector3.up * Hop(winHop);
                    body.localScale = restScale;
                    break;
                case Pose.Tie:
                    body.localPosition = restPosition + Vector3.up * Hop(tieHop);
                    body.localScale = restScale;
                    break;
                case Pose.Lose:
                    body.localPosition = restPosition;
                    var settle = Mathf.Clamp01(time / 0.2f);
                    var squeeze = Mathf.Lerp(1f, squash, settle);
                    body.localScale = new Vector3(restScale.x, restScale.y * squeeze, restScale.z);
                    break;
                default:
                    body.localPosition = restPosition;
                    body.localScale = restScale;
                    break;
            }
        }

        float Hop(float height)
        {
            const float cycle = 0.42f;
            var u = (time % cycle) / cycle;
            return Mathf.Sin(u * Mathf.PI) * height;
        }
    }
}
