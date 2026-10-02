using UnityEngine;

namespace MarioPie
{
    public static class PieProps
    {
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogError("Falta um prefab no Pie Presentation.");
                return null;
            }

            var instance = parent != null ? Object.Instantiate(prefab, parent) : Object.Instantiate(prefab);
            if (parent != null)
            {
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
            }

            return instance;
        }
    }
}
