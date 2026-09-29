using UnityEngine;

namespace MarioPie.Player
{
    public static class PlayerMotion
    {
        public static Vector3 Velocity(Vector2 move, float speed)
        {
            var planar = Vector3.ClampMagnitude(new Vector3(move.x, 0f, move.y), 1f);
            return planar * speed;
        }

        public static Quaternion Facing(Vector3 velocity, Quaternion current)
        {
            var flat = new Vector3(velocity.x, 0f, velocity.z);
            if (flat.sqrMagnitude < 0.0001f)
                return current;

            return Quaternion.LookRotation(flat.normalized, Vector3.up);
        }

        public static Vector3 ClampToSide(Vector3 position, int sideIndex, PlayfieldLimits limits)
        {
            var z = Mathf.Clamp(position.z, limits.MinZ, limits.MaxZ);
            var minX = sideIndex == 0 ? limits.MinX : limits.CenterGap;
            var maxX = sideIndex == 0 ? -limits.CenterGap : limits.MaxX;
            var x = Mathf.Clamp(position.x, minX, maxX);
            return new Vector3(x, position.y, z);
        }
    }
}
