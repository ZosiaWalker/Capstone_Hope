using UnityEngine;

namespace Pathfinding2D
{
    [System.Serializable]
    public class NodeGrid
    {
        public Vector2 worldPosition;
        public bool isWalkable;
        public int proximityPenalty;
        public int gridX;
        public int gridY;

        public NodeGrid(Vector2 worldPosition, bool isWalkable, int proximityPenalty, int gridX, int gridY)
        {
            this.worldPosition = worldPosition;
            this.isWalkable = isWalkable;
            this.proximityPenalty = proximityPenalty;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}