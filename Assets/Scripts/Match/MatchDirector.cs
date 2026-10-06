using MarioPie.Player;
using UnityEngine;

namespace MarioPie.Match
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(50)]
    public sealed class MatchDirector : MonoBehaviour
    {
        [SerializeField] MatchFlow flow;
        [SerializeField] MatchCamera cameraRig;
        [SerializeField] MatchCards cards;
        [SerializeField] MatchScoreHud hud;
        [SerializeField] Transform[] spawns;

        readonly OpeningCutscene opening = new OpeningCutscene();
        MatchRound round;
        PieFlight flight;
        PlayerPieActor[] actors;
        PlayerController[] controllers;
        PlayerCeremony[] ceremonies;
        PieSupply[] supplies;
        SplatField splats;
        bool running;
        bool rematchRequested;
        bool warnedCards;
        bool warnedCamera;

        public MatchPhase Phase => round != null ? round.Phase : MatchPhase.Intro;
        public float SecondsLeft => round != null ? round.SecondsLeft : 0f;
        public bool Hurry => round != null && round.Hurry;
        public bool AllowsPlay => round != null && round.AllowsPlay;
        public bool RematchReady => round != null && round.RematchReady;
        public int Winner { get; private set; } = MatchResult.Tie;

        public bool Begin(
            PlayerPieActor[] players,
            PlayerController[] bodies,
            PieSupply[] pieSupplies,
            SplatField splatField,
            Transform[] spawnPoints)
        {
            if (flow == null)
                flow = Resources.Load<MatchFlow>("MatchFlow");
            if (cameraRig == null)
                cameraRig = FindAnyObjectByType<MatchCamera>();
            if (cards == null)
                cards = FindAnyObjectByType<MatchCards>();
            if (hud == null)
                hud = FindAnyObjectByType<MatchScoreHud>();

            if (flow == null || players == null || players.Length < 2 || players[0] == null || players[1] == null)
            {
                Debug.LogError("A partida precisa dos dois jogadores e do asset MatchFlow.", this);
                return false;
            }

            actors = players;
            controllers = bodies;
            supplies = pieSupplies;
            splats = splatField;
            if (spawnPoints != null && spawnPoints.Length >= 2)
                spawns = spawnPoints;

            flight = new PieFlight();
            opening.Begin();
            round = new MatchRound(
                Mathf.Max(flow.introSeconds, 8f),
                flow.roundSeconds,
                flow.hurrySeconds,
                flow.resultHoldSeconds);
            Winner = MatchResult.Tie;
            ceremonies = new PlayerCeremony[2];
            for (var i = 0; i < 2; i++)
            {
                actors[i].BindMatch(flight, RequestRematch);
                var ceremony = actors[i].GetComponent<PlayerCeremony>();
                if (ceremony == null)
                    ceremony = actors[i].gameObject.AddComponent<PlayerCeremony>();
                ceremony.Setup(
                    actors[i].transform.Find("Body"),
                    flow.readyBob,
                    flow.winHop,
                    flow.tieHop,
                    flow.loseSquash);
                ceremonies[i] = ceremony;
            }

            HideSpawnMarkers();
            if (hud != null)
                hud.Bind(actors[0], actors[1], this, flow);

            running = true;
            Present();
            return true;
        }

        public void RequestRematch()
        {
            rematchRequested = true;
        }

        void Update()
        {
            if (!running || round == null)
                return;

            if (rematchRequested)
            {
                rematchRequested = false;
                if (round.ConfirmRematch())
                {
                    Winner = MatchResult.Tie;
                    ResetTable();
                    opening.Begin();
                }
            }

            if (round.Phase == MatchPhase.Intro)
                opening.Tick(Time.deltaTime, flight != null ? flight.Count : 0, actors, controllers, supplies, spawns);

            var before = round.Phase;
            round.Tick(Time.deltaTime, flight != null ? flight.Count : 0, round.Phase == MatchPhase.Intro && opening.Done);
            if (before != MatchPhase.Result && round.Phase == MatchPhase.Result)
                Winner = MatchResult.Winner(actors[0].Score, actors[1].Score);

            Present();
        }

        void Present()
        {
            ApplyActors();
            PresentCamera();
            PresentCards();
            PresentPoses();
        }

        void ApplyActors()
        {
            var play = round.AllowsPlay;
            var rematch = round.RematchReady;
            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i] != null)
                    actors[i].ApplyGate(play, rematch);
                if (controllers != null && i < controllers.Length && controllers[i] != null)
                    controllers[i].SetPlayOpen(play);
            }
        }

        void PresentCamera()
        {
            if (cameraRig == null)
            {
                if (!warnedCamera)
                {
                    warnedCamera = true;
                    Debug.LogWarning("Falta uma âncora de câmara. O enquadramento de jogo mantém-se.", this);
                }

                return;
            }

            var blend = 1f;
            if (round.Phase == MatchPhase.Intro)
            {
                var duration = flow.cameraIntroSeconds > 0.01f ? flow.cameraIntroSeconds : 1.6f;
                blend = round.PhaseElapsed / duration;
            }
            else if (round.Phase == MatchPhase.Result)
            {
                var duration = flow.resultCameraSeconds;
                blend = duration <= 0.01f ? 1f : round.PhaseElapsed / duration;
            }

            cameraRig.Present(round.Phase, blend, Winner);
        }

        void PresentCards()
        {
            if (cards == null)
            {
                if (!warnedCards)
                {
                    warnedCards = true;
                    Debug.LogWarning("Falta a âncora dos cartões.", this);
                }

                return;
            }

            switch (round.Phase)
            {
                case MatchPhase.Play when round.PhaseElapsed <= flow.startCardSeconds:
                    cards.Show(flow.cardStart);
                    break;
                case MatchPhase.Resolve:
                    cards.Show(flow.cardTimeUp);
                    break;
                case MatchPhase.Result:
                    cards.Show(ResultCard());
                    break;
                default:
                    cards.Hide();
                    break;
            }
        }

        void PresentPoses()
        {
            if (ceremonies == null)
                return;

            for (var i = 0; i < ceremonies.Length; i++)
            {
                if (ceremonies[i] == null)
                    continue;

                ceremonies[i].Show(PoseFor(i));
            }
        }

        PlayerCeremony.Pose PoseFor(int side)
        {
            if (round.Phase == MatchPhase.Intro)
                return PlayerCeremony.Pose.None;
            if (round.Phase != MatchPhase.Result)
                return PlayerCeremony.Pose.None;
            if (Winner == MatchResult.Tie)
                return PlayerCeremony.Pose.Tie;
            return Winner == side ? PlayerCeremony.Pose.Win : PlayerCeremony.Pose.Lose;
        }

        GameObject ResultCard()
        {
            if (Winner == 0)
                return flow.cardWinLeft;
            if (Winner == 1)
                return flow.cardWinRight;
            return flow.cardTie;
        }

        void HideSpawnMarkers()
        {
            if (spawns == null)
                return;

            for (var i = 0; i < spawns.Length; i++)
            {
                var spawn = spawns[i];
                if (spawn == null)
                    continue;

                for (var c = 0; c < spawn.childCount; c++)
                {
                    var child = spawn.GetChild(c);
                    if (child.name == "Marker")
                        child.gameObject.SetActive(false);
                }
            }
        }

        void ResetTable()
        {
            var leftovers = FindObjectsByType<ThrownPie>(FindObjectsSortMode.None);
            for (var i = 0; i < leftovers.Length; i++)
            {
                if (leftovers[i] != null)
                    Destroy(leftovers[i].gameObject);
            }

            flight = new PieFlight();
            for (var i = 0; i < actors.Length; i++)
            {
                if (actors[i] == null)
                    continue;

                actors[i].BindMatch(flight, RequestRematch);
                actors[i].ResetRound();
                if (spawns != null && i < spawns.Length)
                    PlayerSpawn.Place(actors[i], spawns[i]);
            }

            if (supplies != null)
            {
                for (var i = 0; i < supplies.Length; i++)
                {
                    if (supplies[i] != null)
                        supplies[i].Refill();
                }
            }

            if (splats != null)
                splats.Clear();
            if (cards != null)
                cards.Hide();
        }

    }
}
