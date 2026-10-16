using System;
using UnityEngine;

namespace MarioPie.Player
{
    public sealed class CreamCoat : MonoBehaviour
    {
        GameObject[] layers = System.Array.Empty<GameObject>();
        CreamLayers coat = new CreamLayers(1);

        public int Layers => coat.Count;
        public event Action<int> Changed;

        public void Setup(GameObject[] dirtLayers)
        {
            layers = dirtLayers ?? System.Array.Empty<GameObject>();
            coat = new CreamLayers(layers.Length < 1 ? 1 : layers.Length);
            if (layers.Length < 1)
                Debug.LogWarning("Faltam as camadas de nata neste jogador.", this);

            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i] == null)
                    Debug.LogWarning($"Falta a camada de nata {i + 1}.", this);
            }

            Refresh();
        }

        public void AddLayer()
        {
            if (coat.Count >= layers.Length)
                return;

            coat.Add();
            Refresh();
            Changed?.Invoke(coat.Count);
        }

        public void Clear()
        {
            coat.Clear();
            Refresh();
            Changed?.Invoke(0);
        }

        void Refresh()
        {
            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i] == null)
                    continue;

                layers[i].SetActive(i < coat.Count);
            }
        }
    }
}
