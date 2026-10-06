using UnityEngine;
using UnityEngine.UI;

namespace MarioPie.Match
{
    public sealed class CapsulePortrait : MonoBehaviour
    {
        [SerializeField] Camera view;
        [SerializeField] Renderer leftBody;
        [SerializeField] Renderer rightBody;

        RenderTexture buffer;
        RawImage leftPhoto;
        RawImage rightPhoto;

        public void Bind(RawImage sideZero, RawImage sideOne)
        {
            leftPhoto = sideZero;
            rightPhoto = sideOne;
            Open();
        }

        public void Use(Renderer sideZero, Renderer sideOne)
        {
            Copy(leftBody, sideZero);
            Copy(rightBody, sideOne);
        }

        void OnDestroy()
        {
            if (view != null)
                view.targetTexture = null;
            if (buffer != null)
                Destroy(buffer);
        }

        void Open()
        {
            if (view == null || buffer != null)
                return;

            buffer = new RenderTexture(512, 256, 16, RenderTextureFormat.ARGB32)
            {
                name = "CapsulePortraits",
                antiAliasing = 1
            };
            view.targetTexture = buffer;
            view.aspect = buffer.width / (float)buffer.height;
            view.enabled = true;
            Apply(leftPhoto);
            Apply(rightPhoto);
        }

        void Apply(RawImage photo)
        {
            if (photo != null)
                photo.texture = buffer;
        }

        static void Copy(Renderer target, Renderer source)
        {
            if (target == null || source == null)
                return;

            var from = source.GetComponent<MeshFilter>();
            var to = target.GetComponent<MeshFilter>();
            if (from == null || to == null || from.sharedMesh == null)
                return;

            var mesh = from.sharedMesh;
            to.sharedMesh = mesh;
            target.sharedMaterial = source.sharedMaterial;
            var extent = Mathf.Max(mesh.bounds.size.x, Mathf.Max(mesh.bounds.size.y, mesh.bounds.size.z));
            var fit = 1.85f / Mathf.Max(extent, 0.001f);
            target.transform.localScale = Vector3.one * fit;
            target.transform.localPosition = -(mesh.bounds.center * fit);
        }
    }
}
