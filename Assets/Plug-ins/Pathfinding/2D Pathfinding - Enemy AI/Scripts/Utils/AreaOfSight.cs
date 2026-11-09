using UnityEngine;

namespace Pathfinding2D
{
    [System.Serializable]
    public class AreaOfSight
    {
        [SerializeField, Range(-1f, 1f)]
        private float alignment = 0.5f;

        public float Alignment => alignment;
    }
}