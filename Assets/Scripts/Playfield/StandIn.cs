using UnityEngine;

namespace MarioPie
{
    public sealed class StandIn : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
