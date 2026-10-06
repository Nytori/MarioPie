using UnityEngine;

namespace MarioPie.Player
{
    public sealed class PlayerAppearance : MonoBehaviour
    {
        [SerializeField] Renderer body;
        [SerializeField] Material[] sideMaterials;

        public Renderer Body => body;

        public void ApplySide(int sideIndex)
        {
            if (body == null || sideMaterials == null || sideMaterials.Length == 0)
                return;

            var index = Mathf.Clamp(sideIndex, 0, sideMaterials.Length - 1);
            body.sharedMaterial = sideMaterials[index];
        }
    }
}
