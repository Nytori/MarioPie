using UnityEngine;

namespace MarioPie.Player
{
    public static class PieBallistics
    {
        public static Vector3 LaunchVelocity(Vector3 direction, float speed, float lobDegrees)
        {
            var flat = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (flat.sqrMagnitude < 0.0001f)
                flat = Vector3.forward;

            var facing = Quaternion.LookRotation(flat.normalized, Vector3.up);
            var pitched = facing * Quaternion.Euler(-lobDegrees, 0f, 0f);
            return pitched * Vector3.forward * Mathf.Max(0f, speed);
        }

        public static void Step(ref Vector3 position, ref Vector3 velocity, float gravity, float dt)
        {
            velocity.y -= gravity * dt;
            position += velocity * dt;
        }
    }
}
