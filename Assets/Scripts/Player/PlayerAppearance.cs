using Player;
using UnityEngine;

namespace MarioPie.Player
{
    public sealed class PlayerAppearance : MonoBehaviour
    {
        [SerializeField] Renderer body;
        [SerializeField] Material[] sideMaterials;
        
        [SerializeField] private GameObject rat;
        [SerializeField] private GameObject cat;
        [SerializeField] GameObject[] ratDirtLayers;
        [SerializeField] GameObject[] catDirtLayers;

        public Renderer Body => body;

        public GameObject[] ActiveDirtLayers =>
            cat != null && cat.activeSelf ? catDirtLayers : ratDirtLayers;

        public AnimationController getModelController()
        {
            if (cat.activeSelf) return cat.GetComponent<AnimationController>();
            return rat.GetComponent<AnimationController>();
        }

        public void ApplySide(int sideIndex)
        {
            if (body == null || sideMaterials == null || sideMaterials.Length == 0)
                return;

            var index = Mathf.Clamp(sideIndex, 0, sideMaterials.Length - 1);
            body.sharedMaterial = sideMaterials[index];

            if ((sideIndex + 1) % 2 != 0) rat.SetActive(true);
            else cat.SetActive(true);
        }
    }
}
