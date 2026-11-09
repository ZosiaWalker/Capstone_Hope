using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Pathfinding2D
{
    [BurstCompile]
    public struct AStarPathfindingJob : IJob
    {
        public int2 startPos;
        public int2 targetPos;
        public int width;
        public int height;
        public bool allowCornerCutting;
        public bool reachClosestWalkableNode;

        public NativePriorityQueue open;
        public NativeArray<bool> inOpen;
        public NativeArray<bool> closed;
        public NativeList<int> neighbours;

        public NativeArray<NodeStruct> grid;
        public NativeList<int2> resultPath;

        public void Execute()
        {
            int startIndex = startPos.x + startPos.y * width;
            int targetIndex = targetPos.x + targetPos.y * width;

            int closestNode = -1;
            int closestDistance = int.MaxValue;
            bool foundPath = false;

            NodeStruct startNode = grid[startIndex];
            startNode.gCost = 0;
            startNode.hCost = GetDistance(startPos, targetPos);
            grid[startIndex] = startNode;

            open.Push(new NodeScore { Index = startIndex, FCost = startNode.fCost });
            inOpen[startIndex] = true;

            while (open.Count > 0)
            {
                NodeScore currentScore = open.Pop();
                int currentIndex = currentScore.Index;
                closed[currentIndex] = true;
                inOpen[currentIndex] = false;

                NodeStruct currentNode = grid[currentIndex];

                if (currentIndex == targetIndex)
                {
                    foundPath = true;
                    break;
                }

                int distanceToTarget = GetDistance(new int2(currentNode.gridX, currentNode.gridY), targetPos);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestNode = currentNode.index;
                }

                GetNeighbours(currentIndex, neighbours);
                foreach (int neighbourIndex in neighbours)
                {
                    NodeStruct neighbour = grid[neighbourIndex];

                    if (!neighbour.isWalkable || closed[neighbour.index])
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(new int2(currentNode.gridX, currentNode.gridY), new int2(neighbour.gridX, neighbour.gridY)) + neighbour.proximityPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !inOpen[neighbour.index])
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(new int2(neighbour.gridX, neighbour.gridY), targetPos);
                        neighbour.parentIndex = currentIndex;

                        if (!inOpen[neighbour.index])
                        {
                            open.Push(new NodeScore { Index = neighbour.index, FCost = neighbour.fCost });
                            inOpen[neighbour.index] = true;
                        }
                        else
                        {
                            open.UpdatePriority(neighbour.index, neighbour.fCost);
                        }
                        grid[neighbourIndex] = neighbour;
                    }
                }
            }

            if (foundPath)
            {
                RetracePath(targetIndex);
            }
            else if (reachClosestWalkableNode && closestNode != -1)
            {
                RetracePath(closestNode);
            }
        }

        /// <summary>
        /// Estimates the movement cost between two nodes using diagonal distance.
        /// </summary>
        /// <param name="a">The first node.</param>
        /// <param name="b">The second node.</param>
        /// <returns>
        /// An integer cost, where diagonal movement costs 14 and straight movement costs 10.
        /// </returns>
        int GetDistance(int2 a, int2 b)
        {
            int dstX = math.abs(a.x - b.x);
            int dstY = math.abs(a.y - b.y);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }

        public void GetNeighbours(int index, NativeList<int> neighbours)
        {
            neighbours.Clear();

            int x = index % width;
            int y = index / width;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int checkX = x + dx;
                    int checkY = y + dy;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        int neighbourIndex = checkX + checkY * width;

                        if (!allowCornerCutting && dx != 0 && dy != 0)
                        {
                            int horizontalIndex = (x + dx) + y * width;
                            int verticalIndex = x + (y + dy) * width;

                            if (!grid[horizontalIndex].isWalkable || !grid[verticalIndex].isWalkable)
                                continue;
                        }

                        neighbours.Add(neighbourIndex);
                    }
                }
            }
        }

        private void RetracePath(int target)
        {
            if (grid[target].parentIndex == -1) return;
            int curr = target;
            while (curr != -1)
            {
                resultPath.Add(new int2(curr % width, curr / width));
                curr = grid[curr].parentIndex;
            }
        }
    }
}

