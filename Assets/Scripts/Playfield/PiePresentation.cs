using MarioPie.Player;
using UnityEngine;

namespace MarioPie
{
    [CreateAssetMenu(fileName = "PiePresentation", menuName = "MarioPie/Pie Presentation")]
    public sealed class PiePresentation : ScriptableObject
    {
        [Header("Lançamento")]
        [Min(1f)] public float throwSpeed = PieDefaults.ThrowSpeed;
        [Range(10f, 60f)] public float lobDegrees = PieDefaults.LobDegrees;
        [Min(1f)] public float gravity = PieDefaults.Gravity;
        [Min(0f)] public float windupSeconds = PieDefaults.WindupSeconds;
        [Min(0f)] public float refireSeconds = PieDefaults.RefireSeconds;
        [Min(0f)] public float stunSeconds = PieDefaults.StunSeconds;
        [Min(0.2f)] public float grabRadius = PieDefaults.GrabRadius;
        [Min(0.05f)] public float respawnSeconds = PieDefaults.RespawnSeconds;
        [Range(0f, 75f)] public float maxAimDegrees = PieDefaults.MaxAimDegrees;

        [Header("Modelos")]
        [Tooltip("Tarte na mão e em voo. Prefab em Assets/Prefabs.")]
        public GameObject pieModel;

        [Tooltip("Mancha no chão.")]
        public GameObject splatModel;

        [Tooltip("Âncoras das tartes na mão. Cada filho é uma tarte que se pode levar.")]
        public GameObject heldPies;

        [Tooltip("Natas no corpo, uma por filho. A ordem é a das tartadas recebidas.")]
        public GameObject creamSlots;

        [Tooltip("Nata do impacto.")]
        public GameObject creamBlob;

        public Color creamTint = new Color(0.98f, 0.95f, 0.86f);

        [Tooltip("Um corpo por tartada, da primeira em diante. Pivot nos pés, frente em +Z. Quando preenchido, substitui a cápsula e as natas dessa camada.")]
        public GameObject[] dirtyBodies;
    }
}
