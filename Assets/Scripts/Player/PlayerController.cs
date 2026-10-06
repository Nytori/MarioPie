using UnityEngine;
using UnityEngine.InputSystem;

namespace MarioPie.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 6f;
        [SerializeField] CharacterController body;
        [SerializeField] PlayerInput input;

        PlayfieldLimits limits;
        PlayerPieActor pies;
        int sideIndex;
        bool ready;
        bool playOpen = true;

        void Awake()
        {
            if (body == null)
                body = GetComponent<CharacterController>();
            if (input == null)
                input = GetComponent<PlayerInput>();
        }

        public void Configure(int side, PlayfieldLimits playfield)
        {
            sideIndex = side;
            limits = playfield;
            ready = true;
        }

        public void SetPlayOpen(bool open)
        {
            playOpen = open;
        }

        public bool MarchToward(Vector3 target, float dt, float arriveDistance = 0.9f)
        {
            var flat = target - transform.position;
            flat.y = 0f;
            var arrive = arriveDistance * arriveDistance;
            if (flat.sqrMagnitude <= arrive)
                return true;

            var step = Vector3.ClampMagnitude(flat, speed * dt);
            var next = PlayerMotion.ClampToSide(transform.position + step, sideIndex, limits);
            var delta = next - transform.position;
            delta.y = 0f;
            body.Move(delta);
            if (delta.sqrMagnitude > 0.0001f)
                transform.rotation = PlayerMotion.Facing(delta, transform.rotation);

            flat = target - transform.position;
            flat.y = 0f;
            return flat.sqrMagnitude <= arrive;
        }

        void Update()
        {
            if (!ready || input == null || input.actions == null)
                return;

            if (pies == null)
                pies = GetComponent<PlayerPieActor>();

            var vertical = body.isGrounded ? -0.5f * Time.deltaTime : -8f * Time.deltaTime;
            if (!playOpen || (pies != null && pies.BlocksMovement))
            {
                body.Move(new Vector3(0f, vertical, 0f));
                return;
            }

            var move = input.actions["Move"].ReadValue<Vector2>();
            var velocity = PlayerMotion.Velocity(move, speed);
            var next = PlayerMotion.ClampToSide(transform.position + velocity * Time.deltaTime, sideIndex, limits);
            var delta = next - transform.position;
            delta.y = vertical;
            body.Move(delta);

            if (velocity.sqrMagnitude > 0.0001f)
                transform.rotation = PlayerMotion.Facing(velocity, transform.rotation);
        }
    }
}
