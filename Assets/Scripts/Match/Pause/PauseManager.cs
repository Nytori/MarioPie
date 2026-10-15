using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MarioPie.Match.Pause
{
    public class PauseManager : MonoBehaviour
    {

        public static PauseManager Instance;
        
        public bool isPaused = false;
        
        [SerializeField]
        private Image background;

        private void Pause()
        {
            isPaused = true;
            var color = background.color;
            color.a = isPaused ? color.a + Time.fixedDeltaTime : color.a - Time.fixedDeltaTime;
            background.color = color;
            Time.timeScale = 0;
        }

        private bool HasTransitioned()
        {
            return background.color.a == 0 || background.color.a == 1;
        }

        private void Unpause()
        {
            isPaused = false;
            var color = background.color;
            color.a = isPaused ? color.a + Time.fixedDeltaTime : color.a - Time.fixedDeltaTime;
            background.color = color;
            Time.timeScale = 1;
        }

        public void TogglePause()
        {
            if (!HasTransitioned()) return;
            if (isPaused) Unpause();
            else Pause();
        }

        private void Update()
        {
            if (!HasTransitioned())
            {
                var color = background.color;
                color.a = isPaused ? Mathf.Clamp(color.a + Time.fixedDeltaTime * 10, 0, 1) : Mathf.Clamp(color.a - Time.fixedDeltaTime * 10, 0, 1);
                background.color = color;
                print(background.color.a);
            }
        }

        public void Awake()
        {
            Instance = this;
        }
    }
}