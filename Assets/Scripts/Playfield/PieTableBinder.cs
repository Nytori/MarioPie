using System.Collections.Generic;
using UnityEngine;

namespace MarioPie
{
    public static class PieTableBinder
    {
        public static PieSupply[] Bind(float respawnSeconds, float grabRadius)
        {
            var supplies = new List<PieSupply>();
            var pies = GameObject.FindGameObjectsWithTag("Pie");
            for (var i = 0; i < pies.Length; i++)
            {
                var pie = pies[i].transform;
                var renderers = new List<Renderer>();
                var pieRenderer = pie.GetComponent<Renderer>();
                if (pieRenderer != null)
                    renderers.Add(pieRenderer);

                var supply = pie.GetComponent<PieSupply>();
                if (supply == null)
                    supply = pie.gameObject.AddComponent<PieSupply>();

                var side = pie.position.x < 0f ? 0 : 1;
                supply.Configure(side, respawnSeconds, grabRadius, renderers.ToArray());
                supplies.Add(supply);
            }

            return supplies.ToArray();
        }
    }
}