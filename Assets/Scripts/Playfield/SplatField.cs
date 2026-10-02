using System.Collections.Generic;
using UnityEngine;

namespace MarioPie
{
    public sealed class SplatField : MonoBehaviour
    {
        const int Capacity = 40;

        readonly List<GameObject> marks = new List<GameObject>();
        GameObject prefab;
        int cursor;

        public void Setup(GameObject splatPrefab)
        {
            prefab = splatPrefab;
        }

        public void Drop(Vector3 point)
        {
            if (prefab == null)
                return;

            point.y = 0.025f;
            GameObject mark;
            if (marks.Count < Capacity)
            {
                mark = PieProps.Spawn(prefab, transform);
                if (mark == null)
                    return;
                marks.Add(mark);
            }
            else
            {
                mark = marks[cursor];
                cursor = (cursor + 1) % marks.Count;
            }

            var size = Random.Range(0.85f, 1.15f);
            mark.transform.localScale = new Vector3(size, 1f, size);
            mark.transform.SetPositionAndRotation(point, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            mark.SetActive(true);
        }
    }
}
