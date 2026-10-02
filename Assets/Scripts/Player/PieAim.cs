using UnityEngine;

namespace MarioPie.Player
{
    public static class PieAim
    {
        public static Vector3 Direction(int sideIndex, Vector2 move, float maxAngleDegrees)
        {
            var lateral = Mathf.Clamp(move.y, -1f, 1f);
            var across = sideIndex == 0 ? Vector3.right : Vector3.left;
            var yaw = (sideIndex == 0 ? -lateral : lateral) * maxAngleDegrees;
            return Quaternion.Euler(0f, yaw, 0f) * across;
        }
    }
}
