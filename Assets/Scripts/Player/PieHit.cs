using UnityEngine;

namespace MarioPie.Player
{
    public static class PieHit
    {
        public static bool OverlapsBody(Vector3 pie, float pieRadius, Vector3 center, float bodyRadius, float bodyHeight)
        {
            var dx = pie.x - center.x;
            var dz = pie.z - center.z;
            var reach = pieRadius + bodyRadius;
            if (dx * dx + dz * dz > reach * reach)
                return false;

            var half = bodyHeight * 0.5f;
            return pie.y >= center.y - half - pieRadius && pie.y <= center.y + half + pieRadius;
        }

        public static bool SegmentHitsBody(Vector3 from, Vector3 to, float pieRadius, Vector3 center, float bodyRadius, float bodyHeight)
        {
            if (OverlapsBody(from, pieRadius, center, bodyRadius, bodyHeight))
                return true;
            if (OverlapsBody(to, pieRadius, center, bodyRadius, bodyHeight))
                return true;

            return OverlapsBody(Vector3.Lerp(from, to, 0.5f), pieRadius, center, bodyRadius, bodyHeight);
        }

        public static bool ReachedGround(float height, float verticalSpeed, float ground)
        {
            return height <= ground && verticalSpeed <= 0f;
        }
    }
}
