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
        [SerializeField] PiePresentation presentation;

        void Awake()
        {
            if (manager == null)
                manager = GetComponent<PlayerInputManager>();
        }

        void Start()
        {
            if (presentation == null)
                presentation = Resources.Load<PiePresentation>("PiePresentation");
            if (presentation == null)
            {
                Debug.LogError("Falta o asset PiePresentation.", this);
                return;
            }

            var supplies = PieTableBinder.Bind(
                playfield.transform,
                Mathf.Max(0.05f, presentation.respawnSeconds),
                Mathf.Max(0.2f, presentation.grabRadius));
            if (supplies.Length == 0)
                Debug.LogWarning("Nenhuma tarte encontrada nas bancadas.", this);

            var splatsObject = new GameObject("Splats");
            splatsObject.transform.SetParent(playfield.transform, false);
            var splats = splatsObject.AddComponent<SplatField>();
            splats.Setup(presentation.splatModel);

            var actors = new PlayerPieActor[SeatAssignment.SeatCount];
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

                PlayerSpawn.Place(player, spawns[i]);
                player.GetComponent<PlayerController>().Configure(i, playfield.Limits);
                player.GetComponent<PlayerAppearance>().ApplySide(i);
                var actor = player.gameObject.AddComponent<PlayerPieActor>();
                actor.Configure(i, supplies, splats, presentation);
                actors[i] = actor;
            }

            if (actors[0] != null)
                actors[0].SetOpponent(actors[1]);
            if (actors[1] != null)
                actors[1].SetOpponent(actors[0]);

            var controllers = new PlayerController[actors.Length];
            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i] != null)
                    controllers[i] = actors[i].GetComponent<PlayerController>();
            }

            var director = GetComponent<MatchDirector>();
            if (director == null)
                director = gameObject.AddComponent<MatchDirector>();
            if (director.Begin(actors, controllers, supplies, splats, spawns))
                return;

            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i] != null)
                    actors[i].ApplyGate(false, false);
                if (controllers[i] != null)
                    controllers[i].SetPlayOpen(false);
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
    }
}
