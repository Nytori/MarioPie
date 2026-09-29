using MarioPie.Input;
using MarioPie.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MarioPie.Match
{
    public sealed class MatchRoster : MonoBehaviour
    {
        [SerializeField] PlayerInputManager manager;
        [SerializeField] Playfield playfield;
        [SerializeField] Transform[] spawns;
        [SerializeField] Transform playerRoot;

        void Awake()
        {
            if (manager == null)
                manager = GetComponent<PlayerInputManager>();
        }

        void Start()
        {
            var seats = SeatAssignment.Assign(Gamepad.all.Count);
            for (var i = 0; i < seats.Length; i++)
            {
                var device = ResolveDevice(seats[i]);
                if (device == null)
                {
                    Debug.LogError($"Sem dispositivo para o lugar {i} ({seats[i].Scheme}).", this);
                    continue;
                }

                var player = manager.JoinPlayer(i, controlScheme: seats[i].Scheme, pairWithDevice: device);
                if (player == null)
                {
                    Debug.LogError($"Não foi possível juntar o jogador {i}.", this);
                    continue;
                }

                player.gameObject.name = $"Player {i + 1}";
                if (playerRoot != null)
                    player.transform.SetParent(playerRoot, true);

                Place(player, spawns[i]);
                player.GetComponent<PlayerController>().Configure(i, playfield.Limits);
                player.GetComponent<PlayerAppearance>().ApplySide(i);
            }
        }

        static InputDevice ResolveDevice(ControlSeat seat)
        {
            if (!seat.UsesGamepad)
                return Keyboard.current;

            if (seat.GamepadIndex < 0 || seat.GamepadIndex >= Gamepad.all.Count)
                return null;

            return Gamepad.all[seat.GamepadIndex];
        }

        static void Place(PlayerInput player, Transform spawn)
        {
            var body = player.GetComponent<CharacterController>();
            body.enabled = false;
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            body.enabled = true;
        }
    }
}
