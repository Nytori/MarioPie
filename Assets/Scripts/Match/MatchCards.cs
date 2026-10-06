using System.Collections.Generic;
using UnityEngine;

namespace MarioPie.Match
{
    [DisallowMultipleComponent]
    public sealed class MatchCards : MonoBehaviour
    {
        [SerializeField] RectTransform mount;
        readonly HashSet<int> warned = new HashSet<int>();
        GameObject current;
        GameObject currentPrefab;

        public void Show(GameObject prefab)
        {
            if (prefab == currentPrefab && current != null)
                return;

            Hide();
            currentPrefab = prefab;
            if (prefab == null)
            {
                Warn(0, "Falta um cartão da partida.");
                return;
            }

            if (mount == null)
            {
                Warn(1, "Falta a âncora dos cartões.");
                return;
            }

            current = Instantiate(prefab, mount);
            current.transform.localScale = Vector3.one;
        }

        public void Hide()
        {
            if (current != null)
                Destroy(current);

            current = null;
            currentPrefab = null;
        }

        void Warn(int id, string message)
        {
            if (!warned.Add(id))
                return;

            Debug.LogWarning(message, this);
        }
    }
}
