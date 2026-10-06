using UnityEngine;

namespace MarioPie.Match
{
    public sealed class MatchRound
    {
        readonly float introSeconds;
        readonly float roundSeconds;
        readonly float hurrySeconds;
        readonly float resultHoldSeconds;

        public MatchRound(
            float introSeconds,
            float roundSeconds,
            float hurrySeconds,
            float resultHoldSeconds)
        {
            this.introSeconds = AtLeast(introSeconds, 1.6f);
            this.roundSeconds = AtLeast(roundSeconds, 30f);
            this.hurrySeconds = hurrySeconds < 0f ? 10f : hurrySeconds;
            this.resultHoldSeconds = resultHoldSeconds < 0f ? 1.5f : resultHoldSeconds;
            BeginIntro();
        }

        public MatchPhase Phase { get; private set; }
        public float SecondsLeft { get; private set; }
        public float PhaseElapsed { get; private set; }
        public bool Hurry { get; private set; }
        public bool AllowsPlay { get; private set; }
        public bool RematchReady { get; private set; }
        public float IntroDuration => introSeconds;

        public static int DisplaySeconds(float secondsLeft)
        {
            if (secondsLeft <= 0.0001f)
                return 0;

            return Mathf.CeilToInt(secondsLeft - 0.0001f);
        }

        public void Tick(float dt, int piesInFlight, bool introComplete = false)
        {
            if (dt < 0f)
                dt = 0f;
            if (piesInFlight < 0)
                piesInFlight = 0;

            switch (Phase)
            {
                case MatchPhase.Intro:
                    AdvanceIntro(dt, piesInFlight, introComplete);
                    break;
                case MatchPhase.Play:
                    AdvancePlay(dt, piesInFlight);
                    break;
                case MatchPhase.Resolve:
                    if (piesInFlight <= 0)
                        BeginResult();
                    else
                        PhaseElapsed += dt;
                    break;
                case MatchPhase.Result:
                    PhaseElapsed += dt;
                    RematchReady = PhaseElapsed + 0.00001f >= resultHoldSeconds;
                    break;
            }
        }

        public bool ConfirmRematch()
        {
            if (!RematchReady)
                return false;

            BeginIntro();
            return true;
        }

        void AdvanceIntro(float dt, int piesInFlight, bool introComplete)
        {
            PhaseElapsed += dt;
            var timedOut = PhaseElapsed + 0.00001f >= introSeconds;
            if (!introComplete && !timedOut)
                return;

            var extra = PhaseElapsed - introSeconds;
            BeginPlay();
            if (extra > 0.0001f)
                Tick(extra, piesInFlight);
        }

        void AdvancePlay(float dt, int piesInFlight)
        {
            PhaseElapsed += dt;
            SecondsLeft -= dt;
            if (SecondsLeft > 0.00001f)
            {
                Hurry = SecondsLeft <= hurrySeconds + 0.00001f;
                return;
            }

            var extra = -SecondsLeft;
            SecondsLeft = 0f;
            Hurry = false;
            AllowsPlay = false;
            if (piesInFlight <= 0)
            {
                BeginResult();
                if (extra > 0.0001f)
                    Tick(extra, 0);
                return;
            }

            Phase = MatchPhase.Resolve;
            PhaseElapsed = 0f;
            RematchReady = false;
        }

        void BeginIntro()
        {
            Phase = MatchPhase.Intro;
            PhaseElapsed = 0f;
            SecondsLeft = roundSeconds;
            Hurry = false;
            AllowsPlay = false;
            RematchReady = false;
        }

        void BeginPlay()
        {
            Phase = MatchPhase.Play;
            PhaseElapsed = 0f;
            SecondsLeft = roundSeconds;
            Hurry = false;
            AllowsPlay = true;
            RematchReady = false;
        }

        void BeginResult()
        {
            Phase = MatchPhase.Result;
            PhaseElapsed = 0f;
            SecondsLeft = 0f;
            Hurry = false;
            AllowsPlay = false;
            RematchReady = false;
        }

        static float AtLeast(float value, float fallback)
        {
            return value > 0.01f ? value : fallback;
        }
    }
}
