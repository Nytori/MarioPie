using UnityEngine;

namespace MarioPie
{
    [CreateAssetMenu(fileName = "PieSfx", menuName = "MarioPie/Pie Sfx")]
    public sealed class PieSfx : ScriptableObject
    {
        [Tooltip("Nata ao apanhar a tarte. CC0, Breviceps, Freesound 445109.")]
        public AudioClip grab;

        [Tooltip("Arremesso. CC0, bouncyballblue, Freesound 534291.")]
        public AudioClip throwWhoosh;

        [Tooltip("Tarte na cara. CC0, Breviceps, Freesound 445117.")]
        public AudioClip faceHit;

        [Tooltip("Tarte no chão. CC0, Breviceps, Freesound 445109.")]
        public AudioClip floorSplat;

        [Tooltip("Atordoado. CC0, martian, Freesound 403002.")]
        public AudioClip stun;

        [Tooltip("Relógio dos últimos segundos. CC0, OwlStorm, Freesound 212181.")]
        public AudioClip hurry;
    }
}
