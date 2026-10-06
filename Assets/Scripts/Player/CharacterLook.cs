using UnityEngine;

namespace MarioPie.Player
{
    [CreateAssetMenu(fileName = "CharacterLook", menuName = "MarioPie/Character Look")]
    public sealed class CharacterLook : ScriptableObject
    {
        [Tooltip("Corpo limpo deste personagem. Pivot nos pés, frente em +Z. Vazio mantém a cápsula.")]
        public GameObject cleanBody;

        [Tooltip("Um corpo completo por tartada, da primeira em diante. Pivot nos pés, frente em +Z. Cada personagem tem a sua lista.")]
        public GameObject[] dirtyBodies;

        [Tooltip("Foto do placar, por cima do círculo. O fundo transparente deixa ver o círculo. Trocar esta imagem muda o retrato.")]
        public Sprite portrait;
    }
}
