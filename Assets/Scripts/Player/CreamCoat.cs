using System;
using UnityEngine;

namespace MarioPie.Player
{
    public sealed class CreamCoat : MonoBehaviour
    {
        const int FallbackLayers = 6;

        CreamLayers layers = new CreamLayers(1);
        GameObject[] dirtySources = System.Array.Empty<GameObject>();
        GameObject[] dirtyInstances = System.Array.Empty<GameObject>();

        Renderer body;
        PiePresentation presentation;
        MaterialPropertyBlock block;
        Color baseColor = Color.white;
        GameObject cleanSource;
        GameObject cleanInstance;
        GameObject activeBody;

        public int Layers => layers.Count;
        public event Action<int> Changed;

        public void Setup(Renderer bodyRenderer, PiePresentation piePresentation, CharacterLook look)
        {
            body = bodyRenderer;
            presentation = piePresentation;
            if (body != null && body.sharedMaterial != null && body.sharedMaterial.HasProperty("_BaseColor"))
                baseColor = body.sharedMaterial.GetColor("_BaseColor");

            cleanSource = look != null ? look.cleanBody : null;
            dirtySources = look != null && look.dirtyBodies != null ? look.dirtyBodies : System.Array.Empty<GameObject>();
            var max = dirtySources.Length > 0 ? dirtySources.Length : FallbackLayers;
            layers = new CreamLayers(max);
            dirtyInstances = new GameObject[layers.Max];
            Refresh();
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
            var dirtyPrefab = At(dirtySources, count - 1);
            if (dirtyPrefab != null)
            {
                HideCapsule();
                HideClean();
                ShowDirtyBody(dirtyPrefab, count - 1);
                return;
            }

            HideDirtyBodies();
            if (cleanSource != null)
            {
                HideCapsule();
                ShowClean();
                return;
            }

            ShowCapsule();
            Tint(count);
        }

        void ShowDirtyBody(GameObject prefab, int index)
        {
            if (index < 0 || index >= dirtyInstances.Length)
                return;

            if (activeBody != null)
                activeBody.SetActive(false);

            if (dirtyInstances[index] == null)
                dirtyInstances[index] = SpawnBody(prefab, "DirtyBody" + (index + 1));

            activeBody = dirtyInstances[index];
            activeBody.SetActive(true);
        }

        void ShowClean()
        {
            if (cleanInstance == null)
                cleanInstance = SpawnBody(cleanSource, "CleanBody");

            cleanInstance.SetActive(true);
        }

        void HideClean()
        {
            if (cleanInstance != null)
                cleanInstance.SetActive(false);
        }

        void HideDirtyBodies()
        {
            if (activeBody != null)
                activeBody.SetActive(false);
            activeBody = null;
        }

        void HideCapsule()
        {
            if (body == null)
                return;

            body.SetPropertyBlock(null);
            body.enabled = false;
        }

        void ShowCapsule()
        {
            if (body != null)
                body.enabled = true;
        }

        GameObject SpawnBody(GameObject prefab, string label)
        {
            var instance = Instantiate(prefab, transform);
            instance.name = label;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            return instance;
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
