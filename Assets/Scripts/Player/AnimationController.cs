using MarioPie.Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class AnimationController : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int ThrowT1 = Animator.StringToHash("ThrowT1");
        private static readonly int ThrowT2 = Animator.StringToHash("ThrowT2");
        private static readonly int Pies = Animator.StringToHash("Pies");
        private static readonly int HitF = Animator.StringToHash("HitF");
        private static readonly int HitB = Animator.StringToHash("HitB");

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

        private void Throw()
        {
            topAnimator.SetTrigger(ThrowT1);
            topAnimator.SetTrigger(ThrowT2);
        }
        
        public void Hurt(bool fromFront)
        {
            if (fromFront)
            {
                topAnimator.SetTrigger(HitF);
                bottomAnimator.SetTrigger(HitF);
            }
            else
            {
                topAnimator.SetTrigger(HitB);
                bottomAnimator.SetTrigger(HitB);
            }
        }
    }
}