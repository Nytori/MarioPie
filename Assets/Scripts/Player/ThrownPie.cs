using UnityEngine;

namespace MarioPie.Player
{
    [DefaultExecutionOrder(-10)]
    public sealed class ThrownPie : MonoBehaviour
    {
        PlayerPieActor thrower;
        PlayerPieActor target;
        SplatField splats;
        Vector3 velocity;
        float gravity;
        float hitRadius;
        bool resolved;
        PieFlight flight;
        bool counted;

        public void Launch(
            Vector3 origin,
            Vector3 initialVelocity,
            float flightGravity,
            float radius,
            PlayerPieActor owner,
            PlayerPieActor opponent,
            GameObject piePrefab,
            SplatField splatField,
            PieFlight pies)
        {
            thrower = owner;
            target = opponent;
            splats = splatField;
            velocity = initialVelocity;
            gravity = flightGravity;
            hitRadius = radius;
            flight = pies;
            transform.position = origin;
            if (flight != null)
            {
                flight.Begin();
                counted = true;
            }
            var visual = PieProps.Spawn(piePrefab, transform);
            if (visual != null)
                visual.name = "Visual";
        }

        void Update()
        {
            if (resolved)
                return;

            var previous = transform.position;
            var position = previous;
            PieBallistics.Step(ref position, ref velocity, gravity, Time.deltaTime);
            transform.position = position;
            transform.Rotate(0f, 240f * Time.deltaTime, 0f, Space.World);

            if (HitsOpponent(previous, position))
            {
                Resolve();
                thrower.AwardPoint();
                target.ReceiveHit();
                Destroy(gameObject);
                return;
            }

            if (PieHit.ReachedGround(position.y, velocity.y, PieDefaults.GroundHeight) || LeftTheArena(position))
            {
                Resolve();
                if (splats != null)
                    splats.Drop(position);
                Destroy(gameObject);
            }
        }

        bool HitsOpponent(Vector3 from, Vector3 to)
        {
            if (target == null || thrower == null || target.Side == thrower.Side)
                return false;

            var controller = target.GetComponent<CharacterController>();
            var center = target.transform.position + (controller != null ? controller.center : new Vector3(0f, PieDefaults.BodyCenterY, 0f));
            var radius = controller != null ? controller.radius : PieDefaults.BodyRadius;
            var height = controller != null ? controller.height : PieDefaults.BodyHeight;
            return PieHit.SegmentHitsBody(from, to, hitRadius, center, radius, height);
        }

        static bool LeftTheArena(Vector3 position)
        {
            return Mathf.Abs(position.x) > 8f || Mathf.Abs(position.z) > 8f;
        }

        void Resolve()
        {
            resolved = true;
        }

        void OnDestroy()
        {
            if (!counted || flight == null)
                return;

            counted = false;
            flight.End();
        }
    }
}
