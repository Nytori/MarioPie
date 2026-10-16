using MarioPie.Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class AnimationController : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Pies = Animator.StringToHash("Pies");

        [SerializeField]
        private Animator topAnimator;
        [SerializeField]
        private Animator bottomAnimator;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private CharacterController bodyController;

        [SerializeField] private Rig rightArm;
        [SerializeField] private Rig leftArm;

        public bool isCat = false;

        private void Update()
        {
            if (bodyController.velocity == Vector3.zero)
            {
                topAnimator.SetBool(IsWalking, false);
                bottomAnimator.SetBool(IsWalking, false);
            }
            else
            {
                topAnimator.SetBool(IsWalking, true);
                bottomAnimator.SetBool(IsWalking, true);
            }

            topAnimator.SetInteger(Pies, playerController.pies.hands.Held);
            
            if (playerController.pies.hands.Held != 0) leftArm.weight = 1;
            else leftArm.weight = 0;
            if (playerController.pies.hands.Held == 2) rightArm.weight = 1;
            else rightArm.weight = 0;
        }

        public void Throw()
        {
            if (playerController.pies.hands.Held == 0)
            {
                topAnimator.Play("ArmatureTOP|ThrowPieTop1");
                bottomAnimator.Play("ArmatureBOTTOM|ThrowPieBottom1");
            }
            else if (playerController.pies.hands.Held == 1)
            {
                topAnimator.Play("ArmatureTOP|ThrowPieTop2");
                bottomAnimator.Play("ArmatureBOTTOM|ThrowPieBottom2");
            }
        }
        
        public void Hurt(bool fromFront)
        {
            if (fromFront)
            {
                topAnimator.Play("ArmatureTOP|GetHitFrontTop");
                bottomAnimator.Play("ArmatureBOTTOM|GetHitFrontBottom");
            }
            else
            {
                topAnimator.Play("ArmatureTOP|GetHitBackTop");
                bottomAnimator.Play("ArmatureBOTTOM|GetHitBackBottom");
            }
        }

        public void Win()
        {
            if (isCat)
            {
                topAnimator.Play("ArmatureTOP|WinCatTop");
                bottomAnimator.Play("ArmatureBOTTOM|WinCatBottom");
            }
            else
            {
                topAnimator.Play("ArmatureTOP|WinRatTop");
                bottomAnimator.Play("ArmatureBOTTOM|WinRatBottom");
            }
        }
        
        public void Lose()
        {
            topAnimator.Play("ArmatureTOP|LoseTop");
            bottomAnimator.Play("ArmatureBOTTOM|LoseBottom");
        }

        public void ResetAnimations()
        {
            topAnimator.Play("ArmatureTOP|IdleTop");
            bottomAnimator.Play("ArmatureBOTTOM|IdleBottom");
        }
    }
}