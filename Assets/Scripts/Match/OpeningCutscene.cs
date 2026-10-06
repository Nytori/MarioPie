using MarioPie.Player;
using UnityEngine;

namespace MarioPie.Match
{
    public sealed class OpeningCutscene
    {
        const float Windup = 0.45f;

        enum Beat
        {
            Toss,
            WaitPie,
            Fetch,
            Return,
            Done
        }

        Beat beat;
        float tossTime;
        bool thrown;

        public bool Done => beat == Beat.Done;

        public void Begin()
        {
            beat = Beat.Toss;
            tossTime = 0f;
            thrown = false;
        }

        public void Tick(
            float dt,
            int piesInFlight,
            PlayerPieActor[] actors,
            PlayerController[] bodies,
            PieSupply[] supplies,
            Transform[] spawns)
        {
            if (Done)
                return;
            if (actors == null || actors.Length < 2 || actors[0] == null || actors[1] == null)
            {
                beat = Beat.Done;
                return;
            }

            switch (beat)
            {
                case Beat.Toss:
                    Toss(dt, actors[0]);
                    break;
                case Beat.WaitPie:
                    if (piesInFlight <= 0)
                        beat = Beat.Fetch;
                    break;
                case Beat.Fetch:
                    if (EveryoneHasPie(dt, actors, bodies, supplies))
                        beat = Beat.Return;
                    break;
                case Beat.Return:
                    if (BackAtSpawns(dt, actors, bodies, spawns))
                        beat = Beat.Done;
                    break;
            }
        }

        void Toss(float dt, PlayerPieActor left)
        {
            tossTime += dt;
            var land = GroundInFront(left);
            left.HoldDemoPie();
            left.FaceFlat(land - left.transform.position);
            if (thrown || tossTime < Windup)
                return;

            left.ThrowToGround(land);
            thrown = true;
            beat = Beat.WaitPie;
        }

        static bool EveryoneHasPie(float dt, PlayerPieActor[] actors, PlayerController[] bodies, PieSupply[] supplies)
        {
            var ready = true;
            for (var i = 0; i < actors.Length; i++)
            {
                var actor = actors[i];
                if (actor == null || actor.PiesHeld > 0)
                    continue;

                var supply = Nearest(supplies, actor);
                if (supply == null)
                    continue;

                var reach = PieDefaults.GrabRadius;
                var offset = supply.Position - actor.transform.position;
                offset.y = 0f;
                if (offset.sqrMagnitude <= reach * reach)
                {
                    if (!actor.TakeSupply(supply))
                        ready = false;
                    continue;
                }

                if (bodies != null && i < bodies.Length && bodies[i] != null)
                    bodies[i].MarchToward(supply.Position, dt);
                ready = false;
            }

            return ready;
        }

        static bool BackAtSpawns(float dt, PlayerPieActor[] actors, PlayerController[] bodies, Transform[] spawns)
        {
            if (spawns == null || spawns.Length < actors.Length)
                return true;

            var ready = true;
            for (var i = 0; i < actors.Length; i++)
            {
                var actor = actors[i];
                var spawn = spawns[i];
                if (actor == null || spawn == null)
                    continue;

                var arrived = true;
                if (bodies != null && i < bodies.Length && bodies[i] != null)
                    arrived = bodies[i].MarchToward(spawn.position, dt, 0.2f);
                if (!arrived)
                {
                    ready = false;
                    continue;
                }

                actor.FaceFlat(spawn.forward);
            }

            return ready;
        }

        static PieSupply Nearest(PieSupply[] supplies, PlayerPieActor actor)
        {
            if (supplies == null)
                return null;

            PieSupply best = null;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < supplies.Length; i++)
            {
                var supply = supplies[i];
                if (supply == null || supply.Side != actor.Side || !supply.Available)
                    continue;

                var distance = (supply.Position - actor.transform.position).sqrMagnitude;
                if (distance >= bestDistance)
                    continue;

                bestDistance = distance;
                best = supply;
            }

            return best;
        }

        static Vector3 GroundInFront(PlayerPieActor actor)
        {
            var forward = actor.Side == 0 ? Vector3.right : Vector3.left;
            var land = actor.transform.position + forward * 1.7f;
            land.y = PieDefaults.GroundHeight;
            return land;
        }
    }
}
