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
        int sideIndex;
        bool ready;

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

        void Update()
        {
            if (!ready || input == null || input.actions == null)
                return;

            var move = input.actions["Move"].ReadValue<Vector2>();
            var velocity = PlayerMotion.Velocity(move, speed);
            var next = PlayerMotion.ClampToSide(transform.position + velocity * Time.deltaTime, sideIndex, limits);
            var delta = next - transform.position;
            delta.y = body.isGrounded ? -0.5f * Time.deltaTime : -8f * Time.deltaTime;
            body.Move(delta);

            if (velocity.sqrMagnitude > 0.0001f)
                transform.rotation = PlayerMotion.Facing(velocity, transform.rotation);
        }
    }
}
