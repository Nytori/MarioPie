using UnityEngine;
using UnityEngine.InputSystem;

namespace MarioPie.Player
{
    [DefaultExecutionOrder(-20)]
    public sealed class PlayerPieActor : MonoBehaviour
    {
        readonly HitStun stun = new HitStun();
        PieHands hands;

        PlayerInput input;
        InputAction throwAction;
        PieSupply[] supplies;
        PieSlot[] slots;
        SplatField splats;
        CreamCoat creamCoat;
        PiePresentation presentation;
        GameObject[] heldVisuals = System.Array.Empty<GameObject>();
        Vector3[] heldRest = System.Array.Empty<Vector3>();
        PlayerPieActor opponent;
        Vector3 lockedAim = Vector3.right;
        float windup;
        float refire;
        int side;
        bool ready;

        public int Side => side;
        public int Score { get; private set; }
        public int PiesHeld => hands != null ? hands.Held : 0;
        public bool BlocksMovement => stun.Active || windup > 0f;

        public void Configure(
            int sideIndex,
            PieSupply[] pieSupplies,
            SplatField splatField,
            PiePresentation piePresentation)
        {
            side = sideIndex;
            supplies = pieSupplies ?? System.Array.Empty<PieSupply>();
            slots = new PieSlot[supplies.Length];
            splats = splatField;
            presentation = piePresentation;
            input = GetComponent<PlayerInput>();
            throwAction = input != null && input.actions != null
                ? input.actions.FindAction("Throw", false)
                : null;
            if (throwAction == null)
                Debug.LogWarning("A acção Throw não está no mapa de input.", this);

            creamCoat = GetComponent<CreamCoat>();
            if (creamCoat == null)
                creamCoat = gameObject.AddComponent<CreamCoat>();

            var body = transform.Find("Body");
            creamCoat.Setup(body != null ? body.GetComponent<Renderer>() : null, presentation);
            BuildHands();
            lockedAim = side == 0 ? Vector3.right : Vector3.left;
            ready = true;
        }

        void BuildHands()
        {
            var rig = presentation != null ? PieProps.Spawn(presentation.heldPies, transform) : null;
            if (rig == null)
            {
                hands = new PieHands(1);
                return;
            }

            var count = rig.transform.childCount;
            heldVisuals = new GameObject[count];
            heldRest = new Vector3[count];
            for (var i = 0; i < count; i++)
            {
                var anchor = rig.transform.GetChild(i);
                heldRest[i] = anchor.localPosition;
                anchor.gameObject.SetActive(false);
                heldVisuals[i] = anchor.gameObject;
            }

            hands = new PieHands(count < 1 ? 1 : count);
        }

        public void SetOpponent(PlayerPieActor other)
        {
            opponent = other;
        }

        public void AwardPoint()
        {
            Score++;
        }

        public void ReceiveHit()
        {
            var stunSeconds = presentation != null ? presentation.stunSeconds : PieDefaults.StunSeconds;
            stun.Apply(stunSeconds < 0f ? PieDefaults.StunSeconds : stunSeconds);
            windup = 0f;
            if (creamCoat != null)
                creamCoat.AddLayer();
            CreamBurst.Spawn(transform.position + Vector3.up * 1.2f, presentation != null ? presentation.creamBlob : null);
        }

        void Update()
        {
            if (!ready)
                return;

            var dt = Time.deltaTime;
            stun.Tick(dt);
            if (refire > 0f)
                refire -= dt;

            if (stun.Active)
                windup = 0f;
            else if (windup > 0f)
            {
                windup -= dt;
                if (windup <= 0f)
                    Release();
            }

            if (!stun.Active && windup <= 0f && refire <= 0f && PressedThrow())
                BeginThrow();
        }

        void LateUpdate()
        {
            if (!ready)
                return;

            if (!stun.Active && windup <= 0f)
                TryPickup();

            RefreshHeld();
        }

        void BeginThrow()
        {
            if (hands == null || hands.Held <= 0)
                return;

            var move = ReadMove();
            var maxAngle = presentation != null ? presentation.maxAimDegrees : PieDefaults.MaxAimDegrees;
            lockedAim = PieAim.Direction(side, move, maxAngle);
            transform.rotation = Quaternion.LookRotation(lockedAim, Vector3.up);
            var windupSeconds = presentation != null ? presentation.windupSeconds : PieDefaults.WindupSeconds;
            if (windupSeconds <= 0f)
            {
                Release();
                return;
            }

            windup = windupSeconds;
        }

        void Release()
        {
            windup = 0f;
            if (!hands.TrySpend())
                return;

            refire = Mathf.Max(0f, presentation != null ? presentation.refireSeconds : PieDefaults.RefireSeconds);
            var speed = Positive(presentation != null ? presentation.throwSpeed : PieDefaults.ThrowSpeed, PieDefaults.ThrowSpeed);
            var lob = presentation != null ? presentation.lobDegrees : PieDefaults.LobDegrees;
            var gravity = Positive(presentation != null ? presentation.gravity : PieDefaults.Gravity, PieDefaults.Gravity);
            var origin = transform.position + Vector3.up * PieDefaults.ReleaseHeight + lockedAim * 0.8f;
            var pie = new GameObject("Thrown Pie");
            var thrown = pie.AddComponent<ThrownPie>();
            var model = presentation != null ? presentation.pieModel : null;
            thrown.Launch(origin, PieBallistics.LaunchVelocity(lockedAim, speed, lob), gravity, PieDefaults.HitRadius, this, opponent, model, splats);
        }

        void TryPickup()
        {
            if (hands == null || hands.IsFull || supplies.Length == 0)
                return;

            var radius = Positive(presentation != null ? presentation.grabRadius : PieDefaults.GrabRadius, PieDefaults.GrabRadius);
            for (var i = 0; i < supplies.Length; i++)
            {
                var supply = supplies[i];
                slots[i] = new PieSlot(supply.Side, supply.Available, supply.Position);
            }

            var index = PieGrab.Select(slots, transform.position, side, radius);
            if (index < 0 || !hands.TryTake())
                return;

            if (!supplies[index].TryTake())
                hands.TrySpend();
        }

        void RefreshHeld()
        {
            if (hands == null)
                return;

            var windupSeconds = presentation != null ? presentation.windupSeconds : PieDefaults.WindupSeconds;
            var throwingIndex = hands.Held - 1;
            for (var i = 0; i < heldVisuals.Length; i++)
            {
                var visual = heldVisuals[i];
                if (visual == null)
                    continue;

                var active = i < hands.Held;
                if (visual.activeSelf != active)
                    visual.SetActive(active);
                if (!active)
                    continue;

                if (i == throwingIndex && windup > 0f && windupSeconds > 0f)
                {
                    var t = 1f - windup / windupSeconds;
                    visual.transform.position = transform.position + Vector3.up * PieDefaults.ReleaseHeight + lockedAim * Mathf.Lerp(0.35f, 0.8f, t);
                    visual.transform.rotation = Quaternion.identity;
                }
                else
                {
                    visual.transform.localPosition = heldRest[i];
                    visual.transform.localRotation = Quaternion.identity;
                }
            }
        }

        bool PressedThrow()
        {
            return throwAction != null && throwAction.WasPressedThisFrame();
        }

        Vector2 ReadMove()
        {
            if (input == null || input.actions == null)
                return Vector2.zero;

            var move = input.actions.FindAction("Move", false);
            return move != null ? move.ReadValue<Vector2>() : Vector2.zero;
        }

        static float Positive(float value, float fallback)
        {
            return value > 0.01f ? value : fallback;
        }
    }
}
