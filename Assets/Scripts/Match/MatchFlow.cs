using UnityEngine;

namespace MarioPie.Match
{
    [CreateAssetMenu(fileName = "MatchFlow", menuName = "MarioPie/Match Flow")]
    public sealed class MatchFlow : ScriptableObject
    {
        [Header("Tempos")]
        [Min(0.1f)] public float introSeconds = 8f;
        [Min(0.1f)] public float cameraIntroSeconds = 1.6f;
        [Min(0.05f)] public float startCardSeconds = 0.9f;
        [Min(1f)] public float roundSeconds = 30f;
        [Min(0f)] public float hurrySeconds = 10f;
        [Min(0f)] public float resultHoldSeconds = 1.5f;
        [Min(0.1f)] public float resultCameraSeconds = 1.2f;
        [Min(0.05f)] public float scorePunchSeconds = 0.28f;

        [Header("Placar")]
        public Color hurryColor = new Color(1f, 0.32f, 0.22f, 1f);
        public Color timeColor = Color.white;
        [Min(1f)] public float winScoreScale = 1.35f;

        [Header("Poses")]
        [Min(0f)] public float readyBob = 0.08f;
        [Min(0f)] public float winHop = 0.35f;
        [Min(0f)] public float tieHop = 0.16f;
        [Range(0.4f, 1f)] public float loseSquash = 0.72f;

        [Header("Cartões")]
        public GameObject cardStart;
        public GameObject cardTimeUp;
        public GameObject cardWinLeft;
        public GameObject cardWinRight;
        public GameObject cardTie;
        public GameObject scorePopup;
    }
}
