using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace MarioPie
{
    public class PlayerAnim : MonoBehaviour
    {
        [SerializeField] private Animator topAnimator;
        [SerializeField] private Animator bottomAnimator;

        [SerializeField] private Rig rigLeft;
        [SerializeField] private Rig rigRight;

        [Header("Animation Toggles")]
        [SerializeField] private bool isWalking;
        [SerializeField] private bool throw1;
        [SerializeField] private bool throw2;
        [SerializeField] private bool grab1;
        [SerializeField] private bool grab2;
        [SerializeField] private bool hitFront;
        [SerializeField] private bool hitBack;
        [SerializeField] private bool won;
        [SerializeField] private bool lose;

        
        public bool leftHandHasPie = false;
        public bool rightHandHasPie = false;
        
        private void SetBoolBoth(string parameter, bool value)
        {
            if (topAnimator != null)
                topAnimator.SetBool(parameter, value);

            if (bottomAnimator != null)
                bottomAnimator.SetBool(parameter, value);
        }

        private void SetAction(string action)
        {
            string[] actions = { "Throw1", "Throw2", "Grab1", "Grab2", "HitFront", "HitBack", "Won", "Lose" };

            foreach (string parameter in actions)
            {
                SetBoolBoth(parameter, false);
            }

            SetBoolBoth(action, true);
            
            // Adjust rig weights based on the action and pie status
            if (action is "Won" or "Lose")
            {
                if (rigLeft != null) rigLeft.weight = 0;
                if (rigRight != null) rigRight.weight = 0;
            }
            else
            {
                if (rigLeft != null) rigLeft.weight = leftHandHasPie ? 1 : 0;
                if (rigRight != null) rigRight.weight = rightHandHasPie ? 1 : 0;
            }
            
            
        }

        public void SetWalking(bool isWalking)
        {
            SetBoolBoth("IsWalking", isWalking);
        }

        public void Throw1()
        {
            SetAction("Throw1");
        }

        public void Throw2()
        {
            SetAction("Throw2");
        }

        public void Grab1()
        {
            SetAction("Grab1");
        }

        public void Grab2()
        {
            SetAction("Grab2");
        }

        public void HitFront()
        {
            SetAction("HitFront");
        }

        public void HitBack()
        {
            SetAction("HitBack");
        }

        public void Win()
        {
            SetAction("Won");
        }

        public void Lose()
        {
            SetAction("Lose");
        }

        private void OnValidate()
        {
            SetWalking(isWalking);

            if (throw1) SetAction("Throw1");
            else if (throw2) SetAction("Throw2");
            else if (grab1) SetAction("Grab1");
            else if (grab2) SetAction("Grab2");
            else if (hitFront) SetAction("HitFront");
            else if (hitBack) SetAction("HitBack");
            else if (won) SetAction("Won");
            else if (lose) SetAction("Lose");
            else
            {
                // Turn off all actions if none are selected
                string[] actions = { "Throw1", "Throw2", "Grab1", "Grab2", "HitFront", "HitBack", "Won", "Lose" };
                foreach (string action in actions)
                {
                    SetBoolBoth(action, false);
                }
            }
        }
    }
}
