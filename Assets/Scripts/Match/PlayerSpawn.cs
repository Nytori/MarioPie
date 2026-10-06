using UnityEngine;

namespace MarioPie.Match
{
    public static class PlayerSpawn
    {
        public static void Place(Component player, Transform spawn)
        {
            if (player == null || spawn == null)
                return;

            var body = player.GetComponent<CharacterController>();
            if (body != null)
                body.enabled = false;

            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            if (body != null)
                body.enabled = true;
        }
    }
}
