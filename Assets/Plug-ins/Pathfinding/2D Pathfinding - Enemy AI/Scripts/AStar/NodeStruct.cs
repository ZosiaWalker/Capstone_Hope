namespace Pathfinding2D
{
    public struct NodeStruct
    {
        public int index;
        public int parentIndex;

        public bool isWalkable;
        public int proximityPenalty;
        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
    }
}
