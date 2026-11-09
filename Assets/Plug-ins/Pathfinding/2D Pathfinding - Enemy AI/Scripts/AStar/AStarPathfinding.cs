using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Pathfinding2D
{
    /// <summary>
    /// Implements the A* pathfinding algorithm for navigating a 2D grid.
    /// Uses <see cref="AStarGrid"/> nodes to calculate the shortest path
    /// between two world positions.
    /// </summary>
    [RequireComponent(typeof(AStarGrid))]
    public class AStarPathfinding : MonoBehaviour
    {
        private AStarGrid grid;

        private void Awake()
        {
            grid = GetComponent<AStarGrid>();
        }

        public void FindPath(PathRequest request, Action<PathResult> callback)
        {
            if (grid.Grid == null)
            {
                Debug.LogWarning("A* grid was not created! Make sure to create it before requesting a path.");
                return;
            }

            int2 startPosition = grid.WorldToGrid(request.pathStart);
            int2 targetPosition = grid.WorldToGrid(request.pathEnd);

            NativePriorityQueue open = new NativePriorityQueue(grid.Area, Allocator.TempJob);
            NativeArray<bool> inOpen = new NativeArray<bool>(grid.Area, Allocator.TempJob);
            NativeArray<bool> closed = new NativeArray<bool>(grid.Area, Allocator.TempJob);
            NativeList<int> neighbours = new NativeList<int>(8, Allocator.TempJob);

            NativeArray<NodeStruct> runtimeGrid = new NativeArray<NodeStruct>(grid.Area, Allocator.TempJob);
            NativeList<int2> resultPath = new NativeList<int2>(Allocator.TempJob);

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    runtimeGrid[x + y * grid.Width] = new NodeStruct
                    {
                        index = x + y * grid.Width,
                        parentIndex = -1,
                        isWalkable = grid.Grid[x, y].isWalkable,
                        proximityPenalty = grid.Grid[x, y].proximityPenalty,
                        gridX = x,
                        gridY = y,
                        gCost = 0,
                        hCost = 0
                    };
                }
            }

            AStarPathfindingJob job = new AStarPathfindingJob
            {
                startPos = startPosition,
                targetPos = targetPosition,
                width = grid.Width,
                height = grid.Height,
                allowCornerCutting = grid.AllowCornerCutting,
                reachClosestWalkableNode = request.reachClosestWalkableNode,
                open = open,
                inOpen = inOpen,
                closed = closed,
                neighbours = neighbours,
                grid = runtimeGrid,
                resultPath = resultPath
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            Vector3[] waypoints = new Vector3[resultPath.Length];
            for (int i = 0; i < resultPath.Length; i++)
            {
                waypoints[i] = grid.GridToWorld(resultPath[i]);
            }

            Vector3[] simplifiedWaypoints = SimplifyPath(waypoints);
            Array.Reverse(simplifiedWaypoints);

            callback?.Invoke(new PathResult(simplifiedWaypoints, simplifiedWaypoints.Length > 0, request.callback));

            open.Dispose();
            inOpen.Dispose();
            closed.Dispose();
            neighbours.Dispose();

            runtimeGrid.Dispose();
            resultPath.Dispose();
        }

        /// <summary>
        /// Simplifies a path by removing unnecessary nodes that lie 
        /// in the same direction.
        /// </summary>
        /// <param name="path">The full list of nodes along the path.</param>
        /// <returns>
        /// An array of simplified world-space waypoints.
        /// </returns>
        Vector3[] SimplifyPath(Vector3[] path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector3 directionOld = Vector2.zero;

            for (int i = 1; i < path.Length; i++)
            {
                Vector3 directionNew = new Vector3(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i - 1]);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }
    }
}