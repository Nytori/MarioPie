using MarioPie.Player;
using UnityEngine;

namespace MarioPie
{
    public sealed class Playfield : MonoBehaviour
    {
        [SerializeField] float minX = -4.9f;
        [SerializeField] float maxX = 4.9f;
        [SerializeField] float minZ = -6.1f;
        [SerializeField] float maxZ = 6.1f;
        [SerializeField] float centerGap = 0.7f;

        public PlayfieldLimits Limits => new PlayfieldLimits(minX, maxX, minZ, maxZ, centerGap);

        void OnDrawGizmos()
        {
            var limits = Limits;
            DrawSide(limits.MinX, -limits.CenterGap, limits.MinZ, limits.MaxZ, new Color(0.2f, 0.45f, 0.9f, 0.35f));
            DrawSide(limits.CenterGap, limits.MaxX, limits.MinZ, limits.MaxZ, new Color(0.9f, 0.25f, 0.2f, 0.35f));
        }

        static void DrawSide(float minX, float maxX, float minZ, float maxZ, Color color)
        {
            var center = new Vector3((minX + maxX) * 0.5f, 0.05f, (minZ + maxZ) * 0.5f);
            var size = new Vector3(maxX - minX, 0.05f, maxZ - minZ);
            Gizmos.color = color;
            Gizmos.DrawCube(center, size);
        }
    }
}
