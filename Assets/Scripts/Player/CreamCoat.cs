using System;
using UnityEngine;

namespace MarioPie.Player
{
    public sealed class CreamCoat : MonoBehaviour
    {
        CreamLayers layers = new CreamLayers(1);
        GameObject[] blobs = System.Array.Empty<GameObject>();
        GameObject[] dirtyBodies = System.Array.Empty<GameObject>();

        Renderer body;
        PiePresentation presentation;
        MaterialPropertyBlock block;
        Color baseColor = Color.white;
        GameObject activeBody;

        public int Layers => layers.Count;
        public event Action<int> Changed;

        public void Setup(Renderer bodyRenderer, PiePresentation piePresentation)
        {
            body = bodyRenderer;
            presentation = piePresentation;
            if (body != null && body.sharedMaterial != null && body.sharedMaterial.HasProperty("_BaseColor"))
                baseColor = body.sharedMaterial.GetColor("_BaseColor");

            var rig = presentation != null ? PieProps.Spawn(presentation.creamSlots, transform) : null;
            if (rig != null)
            {
                var count = rig.transform.childCount;
                blobs = new GameObject[count];
                for (var i = 0; i < count; i++)
                {
                    var slot = rig.transform.GetChild(i).gameObject;
                    slot.SetActive(false);
                    blobs[i] = slot;
                }
            }

            var dirtyCount = presentation != null && presentation.dirtyBodies != null ? presentation.dirtyBodies.Length : 0;
            var max = Mathf.Max(blobs.Length, dirtyCount);
            layers = new CreamLayers(max < 1 ? 1 : max);
            dirtyBodies = new GameObject[layers.Max];
        }

        public void AddLayer()
        {
            layers.Add();
            Refresh();
            Changed?.Invoke(layers.Count);
        }

        public void Clear()
        {
            layers.Clear();
            Refresh();
            Changed?.Invoke(0);
        }

        void Refresh()
        {
            var count = layers.Count;
            var bodyPrefab = At(presentation != null ? presentation.dirtyBodies : null, count - 1);
            if (bodyPrefab != null)
            {
                if (body != null)
                {
                    body.SetPropertyBlock(null);
                    body.enabled = false;
                }

                ShowDirtyBody(bodyPrefab, count - 1);
                SetBlobCount(0);
                return;
            }

            HideDirtyBodies();
            if (body != null)
                body.enabled = true;

            Tint(count);
            SetBlobCount(count);
        }

        void ShowDirtyBody(GameObject prefab, int index)
        {
            if (index < 0 || index >= dirtyBodies.Length)
                return;

            if (activeBody != null)
                activeBody.SetActive(false);

            if (dirtyBodies[index] == null)
            {
                var instance = Instantiate(prefab, transform);
                instance.name = "DirtyBody" + (index + 1);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                dirtyBodies[index] = instance;
            }

            activeBody = dirtyBodies[index];
            activeBody.SetActive(true);
        }

        void HideDirtyBodies()
        {
            if (activeBody != null)
                activeBody.SetActive(false);
            activeBody = null;
        }

        void SetBlobCount(int count)
        {
            for (var i = 0; i < blobs.Length; i++)
            {
                if (blobs[i] != null)
                    blobs[i].SetActive(i < count);
            }
        }

        void Tint(int count)
        {
            if (body == null || presentation == null)
                return;

            block ??= new MaterialPropertyBlock();
            body.GetPropertyBlock(block);
            var t = count / (float)layers.Max;
            var color = Color.Lerp(baseColor, presentation.creamTint, t * 0.92f);
            if (body.sharedMaterial != null && body.sharedMaterial.HasProperty("_BaseColor"))
                block.SetColor("_BaseColor", color);
            if (body.sharedMaterial != null && body.sharedMaterial.HasProperty("_Color"))
                block.SetColor("_Color", color);
            body.SetPropertyBlock(block);
        }

        static GameObject At(GameObject[] list, int index)
        {
            if (list == null || index < 0 || index >= list.Length)
                return null;

            return list[index];
        }
    }
}
