using System.Collections.Generic;
using UnityEngine;

namespace MarioPie
{
    public static class PieTableBinder
    {
        public static PieSupply[] Bind(Transform playfield, float respawnSeconds, float grabRadius)
        {
            if (playfield == null)
                return System.Array.Empty<PieSupply>();

            var supplies = new List<PieSupply>();
            for (var i = 0; i < playfield.childCount; i++)
            {
                var table = playfield.GetChild(i);
                if (table.name != "TableLeft" && table.name != "TableRight")
                    continue;

                Collect(table, respawnSeconds, grabRadius, supplies);
            }

            return supplies.ToArray();
        }

        static void Collect(Transform table, float respawnSeconds, float grabRadius, List<PieSupply> supplies)
        {
            var creams = new List<Transform>();
            var pies = new List<Transform>();
            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);
                if (child.name == "Pie")
                    pies.Add(child);
                else if (child.name == "Cream")
                    creams.Add(child);
            }

            for (var i = 0; i < pies.Count; i++)
            {
                var pie = pies[i];
                var renderers = new List<Renderer>();
                var pieRenderer = pie.GetComponent<Renderer>();
                if (pieRenderer != null)
                    renderers.Add(pieRenderer);

                var dollop = TakeCream(creams, pie.localPosition.z);
                if (dollop != null)
                {
                    var dollopRenderer = dollop.GetComponent<Renderer>();
                    if (dollopRenderer != null)
                        renderers.Add(dollopRenderer);
                }

                var supply = pie.GetComponent<PieSupply>();
                if (supply == null)
                    supply = pie.gameObject.AddComponent<PieSupply>();

                var side = pie.position.x < 0f ? 0 : 1;
                supply.Configure(side, respawnSeconds, grabRadius, renderers.ToArray());
                supplies.Add(supply);
            }
        }

        static Transform TakeCream(List<Transform> creams, float z)
        {
            var bestIndex = -1;
            var bestDelta = 0.25f;
            for (var i = 0; i < creams.Count; i++)
            {
                var delta = Mathf.Abs(creams[i].localPosition.z - z);
                if (delta > bestDelta)
                    continue;

                bestDelta = delta;
                bestIndex = i;
            }

            if (bestIndex < 0)
                return null;

            var found = creams[bestIndex];
            creams.RemoveAt(bestIndex);
            return found;
        }
    }
}
