using System;
using UnityEngine;
using UnityEngine.UI;

namespace MarioPie.Match.Pause
{
    public class Background : MonoBehaviour
    {
        Image image;
        
        private void Awake()
        {
            if (!image) image = GetComponent<Image>();
        }

        void Update()
        {
            
        }
    }
}