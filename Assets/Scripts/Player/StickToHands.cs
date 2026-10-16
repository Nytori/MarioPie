using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class StickToHands : MonoBehaviour
    {
        public Transform thisHand;
        public bool left;

        public void OnEnable()
        {
            GameObject plr = gameObject.transform.parent.parent.gameObject;
            
            AnimationController[] anim = plr.GetComponentsInChildren<AnimationController>();

            GameObject model = null;
            
            foreach (var animationController in anim)
            {
                if (animationController.gameObject.activeSelf) model = animationController.gameObject;
            }
            
            if (!model) return;
            
            Hand[] hands = model.transform.GetComponentsInChildren<Hand>();
            
            foreach (var hand in hands)
            {
                if (left && hand.isLeftHand) thisHand = hand.transform;
                if (!left && !hand.isLeftHand) thisHand = hand.transform;
            }
        }

        private void Update()
        {
            if (thisHand) transform.position = thisHand.position;
        }
    }
}